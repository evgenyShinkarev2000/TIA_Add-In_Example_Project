using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace XMLViewLibrary
{
    internal class TextBoxLogger : ILogger
    {
        private readonly RichTextBox logArea;

        public TextBoxLogger(RichTextBox logArea)
        {
            this.logArea = logArea;
        }
        public void LogAction(object stringConvertable) =>
            AppendLine(stringConvertable, "info", Brushes.Black);

        public void LogWarning(object stringConvertable) =>
            AppendLine(stringConvertable, "warn", Brushes.Yellow);

        public void LogCritical(object stringConvertable) =>
            AppendLine(stringConvertable, "critical", Brushes.Orange);

        public void LogError(object stringConvertable) =>
            AppendLine(stringConvertable, "error", Brushes.Red);

        private void AppendLine(object stringConvertable, string prefix, SolidColorBrush brush)
        {
            var template = $"{DateTime.Now.ToShortTimeString()} {prefix}: {stringConvertable};";
            var run = new Run(template);
            run.Foreground = brush;
            var paragraph = new Paragraph(run);
            this.logArea.Dispatcher.Invoke(() =>
            {
                this.logArea.Document.Blocks.Add(paragraph);
            });
        }
    }
}
