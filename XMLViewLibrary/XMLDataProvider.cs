using Infastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public event Action<IEnumerable<Data>> DataUpdated;
        public event Action<object> Failed;
        public string PathToXmlFile { get; private set; }

        private readonly FileSystemWatcher fileSystemWatcher;
        public XmlDataProvider()
        {
            fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Changed += (s, e) => Update();

            fileSystemWatcher.Deleted += (s, e) =>
            {
                Failed("File removed");

                throw new FileNotFoundException("File removed");
            };

            fileSystemWatcher.Renamed += (s, e) =>
            {
                Failed("File renamed");

                throw new FileNotFoundException("File renamed");
            };
        }

        ~XmlDataProvider()
        {
            Dispose(false);
        }

        public void WatchXmlFile(string pathToXmlFile)
        {
            if (!File.Exists(pathToXmlFile))
            {
                throw new FileNotFoundException();
            }
            if (Path.GetExtension(pathToXmlFile) != ".xml")
            {
                throw new FileFormatException();
            }

            this.PathToXmlFile = pathToXmlFile;
            fileSystemWatcher.Path = Path.GetDirectoryName(pathToXmlFile);
            fileSystemWatcher.Filter = Path.GetFileName(pathToXmlFile);
            fileSystemWatcher.EnableRaisingEvents = true;
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
                    Failed = null;
                    fileSystemWatcher.Dispose();
                }

                IsDisposed = true;
            }   
        }

        private void Update()
        {
            try
            {
                var data = ParseXmlFile();
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
            foreach(var tryCount in Enumerable.Range(0, 5))
            {
                try
                {
                    using (var fileStream = File.OpenRead(PathToXmlFile))
                    {
                        var document = XDocument.Load(fileStream);

                        return Parser.Parse(document);
                    }
                }
                catch(Exception ex)
                {
                    Failed(ex);
                    innerException = ex;
                }
                //Some programs save files few times
                Thread.Sleep(100);
            }

            throw innerException;
        }
    }
}
