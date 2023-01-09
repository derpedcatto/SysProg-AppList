using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysProg_AppList._2_Tasks
{
    /// <summary>
    /// Interaction logic for Task2Window.xaml
    /// </summary>
    public partial class Task2Window : Window
    {
        #region Variables

        private List<ProgressBar> _progressBarList;

        #endregion


        #region Constructor

        public Task2Window()
        {
            InitializeComponent();
            _progressBarList= new() { ProgressBar_Left, ProgressBar_Center, ProgressBar_Right };
        }

        #endregion


        #region Service Methods

        private void RefreshProgressBarValues()
        {
            foreach(var item in _progressBarList) { item.Value = 0; }
        }

        private void SwitchButtonStatus(bool isBlocked)
        {
            ButtonInOrder.IsEnabled = isBlocked;
            ButtonAsync.IsEnabled = isBlocked;
            ButtonAwait.IsEnabled = isBlocked;
        }

        private void ProgressBarRandomIncrement(ProgressBar progressBar)
        {
            progressBar.Value += new Random().Next(1, 10);
        }

        private async Task FillProgressBar(ProgressBar progressBar)
        {
            // Dispatcher is screaming at me so I have to do this
            bool exitLoop = false;

            while(!exitLoop)
            {
                await Task.Delay(100);
                Dispatcher.Invoke(() => 
                {
                    ProgressBarRandomIncrement(progressBar);
                    if (progressBar.Value == progressBar.Maximum) 
                        exitLoop = true; 
                });
            }
        }

        #endregion


        #region Events

        private async void ButtonInOrder_Click(object sender, RoutedEventArgs e)
        {
            RefreshProgressBarValues();
            SwitchButtonStatus(false);
            foreach (var item in _progressBarList) 
            {
                await Task.Run(() => FillProgressBar(item));
            }
            SwitchButtonStatus(true);
        }

        private async void ButtonAsync_Click(object sender, RoutedEventArgs e)
        {
            List<Task> tasks = new();

            RefreshProgressBarValues();
            SwitchButtonStatus(false);
            
            foreach (var item in _progressBarList)
            {
                tasks.Add(Task.Run(() => FillProgressBar(item)));
            }

            await Task.WhenAll(tasks);
            SwitchButtonStatus(true);
        }

        private async void ButtonAwait_Click(object sender, RoutedEventArgs e)
        {
            RefreshProgressBarValues();
            SwitchButtonStatus(false);

            var taskLeft = Task.Run(() => FillProgressBar(ProgressBar_Left));
            var taskRight = Task.Run(() => FillProgressBar(ProgressBar_Right));

            await Task.WhenAll(taskLeft,taskRight);

            var taskCenter = Task.Run(() => FillProgressBar(ProgressBar_Center));
            await taskCenter;

            SwitchButtonStatus(true);
        }

        #endregion
    }
}
