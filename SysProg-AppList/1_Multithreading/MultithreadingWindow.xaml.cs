using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System;

namespace SysProg_AppList._1_Multithreading
{
    /// <summary>
    /// Interaction logic for MultithreadingWindow.xaml
    /// </summary>
    public partial class MultithreadingWindow : Window
    {
        #region Constructor

        public MultithreadingWindow()
        {
            InitializeComponent();
        }

        #endregion


        #region 1 - Inflation - Bad multithreading example

        // Sync
        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            double sum = 100;
            Inflation.Text = "На начало года: " + sum;
            for (int i = 0; i < 12; i++)
            {
                sum *= 1.1;
                Inflation.Text += String.Format($"\nМесяц {i + 1}, Итог {sum}");
            }
        }


        // Async
        Random rnd = new();
        object locker = new();
        object locker2 = new();
        double Sum;
        int activeThreads = 0;

        class ThreadData
        {
            public int Month { get; set; }
        }

        private void StartAsync_Click(object sender, RoutedEventArgs e)
        {
            StartAsyncButton.IsEnabled = false;

            Sum = 100;
            Inflation.Text = "На начало года: " + Sum;

            int months = 12;
            activeThreads = months;
            for (int i = 0; i < 12; i++)
            {
                new Thread(AddMonth).Start(new ThreadData { Month = i + 1 });
            }
        }

        private void AddMonth(object? data)
        {
            if (data is ThreadData threadData)
            {
                double coef;
                double sum;
                Thread.Sleep(100 + rnd.Next(100));
                coef = 1 + rnd.NextDouble() / 5;

                lock (locker)
                {
                    sum = Sum * coef;
                    Sum = sum;
                }

                Dispatcher.Invoke(() => Inflation.Text += String.Format($"\nМесяц {threadData.Month} Итог {sum} Active Threads {activeThreads}"));

                lock (locker2)
                {
                    activeThreads--;
                    if (activeThreads == 0)
                    {
                        Dispatcher.Invoke(InflationComputed);
                    }
                }
            }
        }

        private void InflationComputed()
        {
            StartAsyncButton.IsEnabled = true;

            Inflation.Text += String.Format($"\n--- Итог {Sum}");
        }

        #endregion


        #region 2 - Multithreading management

        private Thread _worker;
        private CancellationTokenSource _tokenSource;

        private void Worker(object? pars)
        {
            int i = 0;
            if (pars is CancellationToken token)
            {
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(100);
                        Dispatcher.Invoke(() => ProgressBar1.Value++);
                    }
                }
                catch(OperationCanceledException) 
                { 
                    while (i > 0)
                    {
                        Thread.Sleep(30);
                        Dispatcher.Invoke(() => ProgressBar1.Value--);
                        i--;
                    }
                }
            }
        }

        private void Start1_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value = 0;
            _worker = new Thread(Worker);
            _tokenSource= new();
            CancellationToken token = _tokenSource.Token;
            _worker.Start(token);
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();
        }

        #endregion
    }
}