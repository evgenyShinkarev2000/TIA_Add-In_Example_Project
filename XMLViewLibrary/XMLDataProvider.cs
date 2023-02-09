using Infastructure;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLViewLibrary
{
    internal class XmlDataProvider : IDataProvider
    {
        public bool IsDisposed { get; private set; } = false;
        public bool IsOutdated { get; private set; } = true;
        public event Action<IEnumerable<Data>> DataUpdated;
        public event Action<object, LogEventStatus> LogEvents;
        public bool IsRisingUpdates
        {
            get => isRisingUpdates;
            set
            {
                isRisingUpdates = value;
                if (isRisingUpdates)
                {
                    if (CanProvideUpdate)
                    {
                        LogEvents?.Invoke("Starting rise updates", LogEventStatus.Action);
                    }
                    else
                    {
                        LogEvents?.Invoke("Cant starting rise updates.", LogEventStatus.Warning);
                    }
                    
                    if (IsOutdated && CanProvideUpdate)
                    {
                        LogEvents($"Outdated data detected. Use force update.", LogEventStatus.CriticalWarning);
                    }
                }
                else
                {
                    LogEvents?.Invoke("Stop rise updates", LogEventStatus.Warning);
                }
            }
        }
        public bool CanProvideUpdate { get; private set; } = false;
        public event Action<bool> CanProvideUpdateChanged;
        public string PathToXmlFile { get; private set; }

        private readonly FileSystemWatcher fileSystemWatcher;
        private bool isRisingUpdates = false;
        private DateTime lastModifiedTime;
        public XmlDataProvider()
        {
            fileSystemWatcher = new FileSystemWatcher();

            CanProvideUpdateChanged += (canProvideUpdate) => CanProvideUpdate = canProvideUpdate;

            fileSystemWatcher.Changed += (s, e) =>
            {
                try
                {
                    if (IsRisingUpdates)
                    {
                        LogEvents($"File {PathToXmlFile} changed. Starting update...", LogEventStatus.ControlPointAction);
                        Update();
                    }
                    else
                    {
                        LogEvents($"File {PathToXmlFile} changed. Current data outdated.", LogEventStatus.CriticalWarning);
                        IsOutdated = true;
                    }
                }
                catch (Exception ex)
                {
                    LogEvents(ex, LogEventStatus.Error);
                }
            };

            fileSystemWatcher.Renamed += (s, e) =>
            {
                LogEvents($"File {e.OldFullPath} was renamed to {e.FullPath}. Updating watch target...", LogEventStatus.Warning);
                WatchXmlFile(e.FullPath);
                LogEvents($"Target {e.OldFullPath} was updated to {e.FullPath}", LogEventStatus.Warning);
            };

            fileSystemWatcher.Deleted += (s, e) =>
            {
                LogEvents($"File {e.FullPath} was removed. Must update watch target or retrieve file", LogEventStatus.CriticalWarning);
                CanProvideUpdateChanged(false);
            };

            fileSystemWatcher.Created += (s, e) =>
            {
                LogEvents($"File {e.FullPath} was retived. Continue watch", LogEventStatus.Warning);
                if (lastModifiedTime != File.GetLastWriteTimeUtc(PathToXmlFile))
                {
                    LogEvents($"File was modified without watcher. Current data probably outdated. Use force update", LogEventStatus.CriticalWarning);
                }
                CanProvideUpdateChanged(true);
            };
        }

        ~XmlDataProvider()
        {
            Dispose(false);
        }

        public void WatchXmlFile(string pathToXmlFile)
        {
            LogEvents($"Looking for {pathToXmlFile}", LogEventStatus.Action);
            if (!File.Exists(pathToXmlFile))
            {
                throw new FileNotFoundException();
            }
            if (Path.GetExtension(pathToXmlFile) != ".xml")
            {
                throw new FileFormatException("Expected *.xml file");
            }

            this.PathToXmlFile = pathToXmlFile;
            fileSystemWatcher.Path = Path.GetDirectoryName(pathToXmlFile);
            fileSystemWatcher.Filter = Path.GetFileName(pathToXmlFile);
            LogEvents($"Starting watch {pathToXmlFile}", LogEventStatus.Action);
            fileSystemWatcher.EnableRaisingEvents = true;
            if (IsRisingUpdates)
            {
                LogEvents.Invoke("Starting rise updates", LogEventStatus.Action);
            }
            CanProvideUpdateChanged(true);
        }

        public void ForceUpdate()
        {
            if (PathToXmlFile == null)
            {
                throw new InvalidOperationException($"{nameof(PathToXmlFile)} is null. Call {nameof(WatchXmlFile)} first");
            }
            Update();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isFromDispose)
        {
            if (!IsDisposed)
            {
                if (isFromDispose)
                {
                    DataUpdated = null;
                    LogEvents = null;
                    CanProvideUpdateChanged = null;
                    CanProvideUpdate = false;
                    fileSystemWatcher.Dispose();
                }

                IsDisposed = true;
            }
        }

        private void Update()
        {
            try
            {
                lastModifiedTime = File.GetLastWriteTimeUtc(PathToXmlFile);
                var data = ParseXmlFile();
                IsOutdated = false;
                DataUpdated(data);
            }
            catch (Exception ex)
            {
                throw new XmlException($"Error read, parse XmlFile or {nameof(DataUpdated)} handler throw exception", ex);
            }
        }

        private IEnumerable<Data> ParseXmlFile()
        {
            Exception innerException = null;
            foreach (var tryCount in Enumerable.Range(0, 5))
            {
                try
                {
                    using (var fileStream = File.OpenRead(PathToXmlFile))
                    {

                        var document = XDocument.Load(fileStream);

                        return Parser.Parse(document);
                    }
                }
                catch (IOException ex)
                {
                    innerException = ex;
                    LogEvents("File busy", LogEventStatus.Warning);
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
                //Some programs save files few times
                Thread.Sleep(100);
            }

            throw innerException;
        }
    }
}
