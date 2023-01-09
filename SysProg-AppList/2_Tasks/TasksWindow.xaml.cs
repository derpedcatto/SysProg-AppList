using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SysProg_AppList._2_Tasks
{
    /// <summary>
    /// Interaction logic for TasksWindow.xaml
    /// </summary>
    public partial class TasksWindow : Window
    {
        #region Constructor

        public TasksWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region Thread Pool

        CancellationTokenSource _cts;

        private void PoolWorker(object? pars)
        {
            if (pars is PoolWorkerData data)
            {
                if (data.CancellationToken.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => Log1.Text += $"..{data.Num}\t");
            }
        }

        private void Start1_Click(object sender, RoutedEventArgs e)
        {
            Log1.Text = "Start";
            _cts = new();
            for (int i = 0; i < 40; i++)
            {
                ThreadPool.QueueUserWorkItem(PoolWorker, new PoolWorkerData
                {
                    Num = i,
                    CancellationToken = _cts.Token
                });
            }
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            Log1.Text += "Stop";
            _cts?.Cancel();
        }

        #endregion


        #region Multitasking

        private string TaskMethod1(int num)
        {
            Thread.Sleep(1000);
            return $"tm1-{num}\t";
        }

        private void TaskMethod2(int num)
        {
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => Log2.Text += $"..{num}\t");
        }

        private void Starter2()
        {
            Task t1 = new(() => TaskMethod2(20));
            t1.Start();

            Task t2 = Task.Run(() => TaskMethod2(10));
            t2.Wait();

            Task.Run(() => TaskMethod2(30));
        }


        private void Starter3_Bad()
        {
            Task<String> t = Task.Run(() => TaskMethod1(40));
            String res = t.Result;

            t = Task.Run(() => TaskMethod1(50));
            res = t.Result;
            Dispatcher.Invoke(() => Log2.Text += res);

            t = Task.Run(() => TaskMethod1(60));
            res = t.Result;
            Dispatcher.Invoke(() => Log2.Text += res);
        }

        private void Starter3_Good()
        {
            Task<String> t1 = Task.Run(() => TaskMethod1(40));
            Task<String> t2 = Task.Run(() => TaskMethod1(50));
            Task<String> t3 = Task.Run(() => TaskMethod1(60));

            Dispatcher.Invoke(() => Log2.Text += t1.Result);
            Dispatcher.Invoke(() => Log2.Text += t2.Result);
            Dispatcher.Invoke(() => Log2.Text += t3.Result);
        }


        private void Start2_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Starter2);
        }

        private void Start3_Click(object sender, RoutedEventArgs e)
        {
            Log2.Text = "Start3 ";
            Task.Run(Starter3_Good);
        }

        private void Stop2_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region Async / Await

        private async Task<String> FileContentAsync(String filename)
        {
            return await Task.Run(() => 
            {
                var sb = new StringBuilder();
                using (StreamReader reader = new(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        sb.AppendLine(reader.ReadLine());
                    }
                }
                return sb.ToString(); 
            });
        }

        private async Task<Dictionary<String, String>> ParseIniAsync(String ini)
        {
            return await Task.Run(() =>
            {
                Dictionary<String, String> res = new();
                String[] lines = ini.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach(String line in lines)
                {
                    if (!line.StartsWith('#'))
                    {
                        String[] pair = line.Split('=');
                        res[pair[0]] = pair[1];
                    }
                }

                return res;
            });
        }

        private async void LogDicAsync(Dictionary<String, String> dic)
        {
            await Task.Run((() =>
            {
                foreach (var pair in dic)
                {
                    this.Dispatcher.Invoke(() => LogAsync.Text += $"{pair.Key, -10} = {pair.Value}");
                }
            }));
        }

        private async void StartAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            var loader = FileContentAsync("C:\\Users\\sasha\\source\\repos\\SysProg-AppList\\SysProg-AppList\\2_Tasks\\TaskWindow.ini");
            var task = loader.ContinueWith(res => ParseIniAsync(res.Result))
                             .Unwrap()
                             .ContinueWith(res => LogDicAsync(res.Result));

            LogAsync.Text = "ini loading...\n";
        }

        #endregion
    }
}