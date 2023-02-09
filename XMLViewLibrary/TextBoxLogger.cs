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
    internal class TextBoxLogger : AbstractLogger
    {
        private readonly RichTextBox logArea;
        private readonly Color darkYellow = Color.FromRgb(0x9C, 0x81, 0x00);

        public TextBoxLogger(RichTextBox logArea)
        {
            this.logArea = logArea;
        }

        public override void LogAction(object stringConvertable) =>
            AppendLine(stringConvertable, "info", Brushes.Black);

        public override void LogControlPointAction(object stringConvertabe) =>
            AppendLine(stringConvertabe, "control info", Brushes.DarkCyan);

        public override void LogWarning(object stringConvertable) =>
            AppendLine(stringConvertable, "warn", new SolidColorBrush(darkYellow));

        public override void LogCritical(object stringConvertable) =>
            AppendLine(stringConvertable, "critical", Brushes.DarkOrange);

        public override void LogError(object stringConvertable) =>
            AppendLine(stringConvertable, "error", Brushes.DarkRed);

        private void AppendLine(object stringConvertable, string prefix, SolidColorBrush brush)
        {
            var infoRun = new Run($"{DateTime.Now.ToLongTimeString()} {prefix}: ");
            infoRun.Foreground = brush;
            var messageRun = new Run(stringConvertable.ToString());
            logArea.Dispatcher.Invoke(() =>
            {
                var paragraph = logArea.Document.Blocks.FirstBlock as Paragraph;
                paragraph.Inlines.Add("\n");
                paragraph.Inlines.Add(infoRun);
                paragraph.Inlines.Add(messageRun);
            });
        }
    }
}
