using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace SysProg_AppList
{
    struct AdditionTaskData
    {
        public int Step { get; set; }
        public int AdditionNum { get; set; }
        public int TotalSum { get; set; }
    }


    /// <summary>
    /// Interaction logic for Task1Window.xaml
    /// </summary>
    public partial class Task1Window : Window
    {
        #region Variables

        int _totalSum;
        int _currentStep;

        #endregion


        #region Constructor

        public Task1Window()
        {
            InitializeComponent();
        }

        #endregion


        #region Methods

        private void SwitchInterfaceLock()
        {
            InputTextBox.IsEnabled = !InputTextBox.IsEnabled;
            SubmitButton.IsEnabled = !SubmitButton.IsEnabled;
        }


        private void StartAdditionTask(int baseNum)
        {
            _totalSum = 0;
            _currentStep = 0;

            for (int i = 1; i <= baseNum; i++)
            {
                Thread thread = new (ExecuteAddition!);
                thread.Start(i);
            }
        }

        private void ExecuteAddition(object additionNum)
        {
            _totalSum += (int)additionNum;
            int localSum = _totalSum;   // Current '_totalSum' value for storing in local thread
            _currentStep++;

            AdditionTaskData data = new()
            {
                Step = _currentStep,
                AdditionNum = (int)additionNum,
                TotalSum = localSum
            };
            Dispatcher.Invoke(() => OutputListView.Items.Add(data));
        }

        #endregion


        #region Window Events

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Block input field until all threads are finished
            int baseNum;
            try
            {
                baseNum = Convert.ToInt32(InputTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Input is not valid!");
                return;
            }

            if (baseNum < 1)
            {
                MessageBox.Show("Number must be greater than 1!");
                return;
            }

            OutputListView.Items.Clear();
            SwitchInterfaceLock();
            StartAdditionTask(baseNum);
            SwitchInterfaceLock();
        }

        #endregion
    }
}
