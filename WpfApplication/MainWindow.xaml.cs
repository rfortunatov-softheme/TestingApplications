using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Replay.Core.Web.Helpers;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task _generationTask;
        private CancellationTokenSource _cancellationTokenSource;

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Start.SelectedDate == null || End.SelectedDate == null)
            {
                MessageBox.Show("You have to select Start and End date");
                return;
            }

            if (Start.SelectedDate > End.SelectedDate || Start.SelectedDate == End.SelectedDate)
            {
                MessageBox.Show("Start date should be less then End date.");
                return;
            }

            if (string.IsNullOrEmpty(Agent.Text) || string.IsNullOrEmpty(Core.Text) || string.IsNullOrEmpty(User.Text) || string.IsNullOrEmpty(Password.Text))
            {
                MessageBox.Show("Please fill agent, core, user and password fields.");
                return;
            }

            int iterval;
            if (string.IsNullOrEmpty(Interval.Text) || !int.TryParse(Interval.Text, out iterval))
            {
                MessageBox.Show("Interval filed should be not blank and should contain only numbers.");
                return;
            }

            if (!RunCmdUtil(string.Format("-list repositories -core {0} -user {1} -password {2}", Core.Text, User.Text, Password.Text),
                p =>
                {
                    var error = p.StandardError.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    if (error.Length > 0)
                    {
                        MessageBox.Show(error);
                        return false;
                    }

                    return true;
                }))
            {
                return;
            }

            if (!RunCmdUtil(string.Format("-list protectedservers -core {0} -user {1} -password {2}", Core.Text, User.Text, Password.Text),
                p =>
                {
                    var error = p.StandardError.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    if (error.Length > 0)
                    {
                        MessageBox.Show(error);
                        return false;
                    }

                    var output = p.StandardOutput.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    if (!output.Contains(Agent.Text))
                    {
                        MessageBox.Show(string.Format("Agent with name {0} not found.", Agent.Text));
                        return false;
                    }
                    
                    return true;
                }))
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _generationTask = Task.Factory.StartNew(GenerateData, _cancellationTokenSource.Token);
        }

        private void GenerateData()
        {
            DateTime start = DateTime.Now;
            Dispatcher.Invoke(() => start = Start.SelectedDate.Value);
            DateTime end = DateTime.Now;
            Dispatcher.Invoke(() => end = End.SelectedDate.Value);
            var user = string.Empty;
            var password = string.Empty;
            var agent = string.Empty;
            var core = string.Empty;
            var intervalString = string.Empty;
            int interval;
            Dispatcher.Invoke(() => intervalString = Interval.Text);
            Dispatcher.Invoke(() => user = User.Text);
            Dispatcher.Invoke(() => password = Password.Text);
            Dispatcher.Invoke(() => agent = Agent.Text);
            Dispatcher.Invoke(() => core = Core.Text);
            interval = int.Parse(intervalString);
            while (start <= end)
            {
                var sw = new Stopwatch();
                sw.Start();
                Dispatcher.Invoke(() => Output.Text += string.Format("Generating recovery point for date {0}", start));
                Dispatcher.Invoke(() => Output.Text += Environment.NewLine);

                var newTime = new SYSTEMTIME
                {
                    wYear = (short)start.Year,
                    wMonth = (short)start.Month,
                    wDay = (short)start.Day,
                    wHour = (short)start.Hour,
                    wMinute = (short)start.Minute,
                    wSecond = (short)start.Second
                };
                // must be short

                SetSystemTime(ref newTime); // invoke this method.

                Thread.Sleep(5000);

                Dispatcher.Invoke(() => Output.Text += "Forcing transfer");
                Dispatcher.Invoke(() => Output.Text += Environment.NewLine);

                if (!RunCmdUtil(string.Format("-force -core {0} -user {1} -password {2} -protectedserver {3}", core, user, password, agent),
                p =>
                {
                    var error = p.StandardError.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    if (error.Length > 0)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show(error));
                        return false;
                    }

                    return true;
                }))
                {
                    return;
                }

                Thread.Sleep(5000);

                Dispatcher.Invoke(() => Output.Text += "Waiting for transfer to end");
                Dispatcher.Invoke(() => Output.Text += Environment.NewLine);

                while (!RunCmdUtil(string.Format("-list activejobs -core {0} -user {1} -password {2} -protectedserver {3}", core, user, password, agent),
                p =>
                {
                    var output = p.StandardOutput.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    return output.Contains("No jobs of the specified type were found on the core");
                }))
                {
                    Thread.Sleep(100);
                }

                sw.Stop();

                Dispatcher.Invoke(() => Output.Text += string.Format("Generating recovery point took {0}", sw.Elapsed));
                Dispatcher.Invoke(() => Output.Text += Environment.NewLine);

                start = start.AddHours(interval);
                if (!RunCmdUtil(string.Format("-forcerollup -core {0} -user {1} -password {2} -protectedserver {3}", core, user, password, agent),
                p =>
                {
                    var error = p.StandardError.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    if (error.Length > 0)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show(error));
                        return false;
                    }

                    return true;
                }))
                {
                    return;
                }

                while (!RunCmdUtil(string.Format("-list activejobs -core {0} -user {1} -password {2} -protectedserver {3}", core, user, password, agent),
                p =>
                {
                    var output = p.StandardOutput.ReadToEnd().Replace("\0", string.Empty).Replace("\r\n", Environment.NewLine);
                    return output.Contains("No jobs of the specified type were found on the core");
                }))
                {
                    Thread.Sleep(100);
                }

                Cli.Core.LicenseManagement.ForcePhoneHome();
                while (Cli.Core.LicenseManagement.IsPhoneHomeInProgress())
                {
                    Thread.Sleep(1000);
                }
            }

            Dispatcher.Invoke(() => Output.Text += "Finished generation!");
            Dispatcher.Invoke(() => Output.Text += Environment.NewLine);
        }

        private bool RunCmdUtil(string arguments, Func<Process, bool> checFunc)
        {
            var processStartInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "cmdutil.exe")
            };

            using (var p = Process.Start(processStartInfo))
            {
                p.WaitForExit();
                return checFunc == null || checFunc(p);
            }
        }

        private void StopGeneratrion(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => Output.Text += "Cancelling generation!");
            Dispatcher.Invoke(() => Output.Text += Environment.NewLine);
            _cancellationTokenSource.Cancel();
            _generationTask.Wait();
        }
    }
}
