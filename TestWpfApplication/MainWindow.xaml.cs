using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TestWpfApplication
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Regex _logRegex = new Regex(@"(?<title>(INFO|WARN|DEBUG|ERROR|FATAL) +\d{4}-\d{2}-\d{2}T.*?(\r\n|\n))(?<header>.*?(\r\n|\n))(?<body>.*?)(?=(INFO |WARN |DEBUG |ERROR |FATAL ))", RegexOptions.Singleline);

        private static string _logFile;
        private readonly List<DataItem> _originaItems = new List<DataItem>();

        public MainWindow()
        {
            InitializeComponent();
            SevirityItems = new PickerContext
            {
                PropertyDescription = "Sevirity Level"
            };

            ThreadItems = new PickerContext
            {
                PropertyDescription = "Thread #"
            };

            HeaderItems = new PickerContext
            {
                PropertyDescription = "Message Header"
            };

            SourceItems = new PickerContext
            {
                PropertyDescription = "Message Source"
            };

            Thread.DataContext = ThreadItems;
            Sevirity.DataContext = SevirityItems;
            Source.DataContext = SourceItems;
            Title.DataContext = HeaderItems;
        }


        public PickerContext SevirityItems { get; set; }

        public PickerContext ThreadItems { get; set; }

        public PickerContext HeaderItems { get; set; }

        public PickerContext SourceItems { get; set; }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            SevirityItems.Items.Clear();
            ThreadItems.Items.Clear();
            HeaderItems.Items.Clear();
            SourceItems.Items.Clear();
            _originaItems.Clear();

            _logFile = File.ReadAllText(Path.Text.Replace("\"", string.Empty));
            var matches = _logRegex.Matches(_logFile);
            Parallel.ForEach(matches.Cast<Match>(), match =>
            {
                if (!string.IsNullOrEmpty(match.Groups["title"].Value) &&
                    !string.IsNullOrEmpty(match.Groups["header"].Value))
                {
                    var item = new DataItem
                    {
                        Severity = match.Groups["title"].Value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0],
                        Date = DateTime.Parse(match.Groups["title"].Value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1]),
                        Thread = int.Parse(match.Groups["title"].Value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[2].Replace("[", string.Empty).Replace("]", string.Empty)),
                        Source = match.Groups["title"].Value.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries)[1].Split('(')[0],
                        Header = match.Groups["header"].Value,
                        Message = match.Groups["body"].Value
                    };
                    _originaItems.Add(item);
                }
            });

            foreach (var group in _originaItems.GroupBy(x => x.Thread).OrderByDescending(x => x.Count()))
            {
                ThreadItems.Items.Add(new PickerItem
                {
                    Hide = false,
                    Text = group.Key.ToString(),
                    Num = group.Count()
                });
            }

            foreach (var group in _originaItems.GroupBy(x => x.Header).OrderByDescending(x => x.Count()))
            {
                HeaderItems.Items.Add(new PickerItem
                {
                    Hide = false,
                    Text = group.Key,
                    Num = group.Count()
                });
            }

            foreach (var group in _originaItems.GroupBy(x => x.Source).OrderByDescending(x => x.Count()))
            {
                SourceItems.Items.Add(new PickerItem
                {
                    Hide = false,
                    Text = group.Key,
                    Num = group.Count()
                });
            }

            foreach (var group in _originaItems.GroupBy(x => x.Severity).OrderByDescending(x => x.Count()))
            {
                SevirityItems.Items.Add(new PickerItem
                {
                    Hide = false,
                    Text = group.Key,
                    Num = group.Count()
                });
            }
        }

        private void ShowResult(object sender, RoutedEventArgs e)
        {
            var selected = _originaItems
                .Where(x => HeaderItems.Items.Any(y => string.Equals(y.Text, x.Header) && !y.Hide))
                .Where(x => SevirityItems.Items.Any(y => string.Equals(y.Text, x.Severity) && !y.Hide))
                .Where(x => SourceItems.Items.Any(y => string.Equals(y.Text, x.Source) && !y.Hide))
                .Where(x => ThreadItems.Items.Any(y => int.Parse(y.Text) == x.Thread && !y.Hide))
                .OrderBy(x => x.Date);

            Result.Document.Blocks.Clear();
            foreach (var x in selected)
            {
                var message = string.Format("{0} {1} [{2}] - {3}", x.Severity, x.Date, x.Thread, x.Source);
                if (!string.IsNullOrEmpty(x.Header) && x.Header != Environment.NewLine && x.Header != "\n")
                {
                    message += Environment.NewLine;
                    message += x.Header;
                }

                if (!string.IsNullOrEmpty(x.Message) && x.Message != Environment.NewLine && x.Message != "\n")
                {
                    message += Environment.NewLine;
                    message += x.Message;
                }

                switch (x.Severity)
                {
                    case "DEBUG":
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.Gray),
                            Text = message
                        }));
                        break;

                    case "INFO":
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.Green),
                            Text = message
                        }));
                        break;

                    case "WARN":
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.Orange),
                            Text = message
                        }));
                        break;

                    case "ERROR":
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.PaleVioletRed),
                            Text = message
                        }));
                        break;

                    case "FATAL":
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.Red),
                            Text = message
                        }));
                        break;

                    default:
                        Result.Document.Blocks.Add(new Paragraph(new Run
                        {
                            Foreground = new SolidColorBrush(Colors.Black),
                            Text = message
                        }));
                        break;
                }
            }
        }
    }
}