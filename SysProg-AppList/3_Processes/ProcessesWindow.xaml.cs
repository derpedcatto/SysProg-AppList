using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SysProg_AppList._3_Processes
{
    /// <summary>
    /// Interaction logic for ProcessesWindow.xaml
    /// </summary>
    public partial class ProcessesWindow : Window
    {
        #region Variables

        public ObservableCollection<Process> Processes { get; set; }

        #endregion


        #region Constructor

        public ProcessesWindow()
        {
            InitializeComponent();
            Processes = new();
            this.DataContext = this;
        }

        #endregion


        #region Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process[] proc = Process.GetProcesses();
            Processes.Clear();
            foreach (Process process in proc)
            {
                Processes.Add(process);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Process process)
                {
                    try
                    {
                        ThreadsBlock.Text = string.Empty;
                        foreach (ProcessThread thread in process.Threads)
                        {
                            ThreadsBlock.Text += thread.Id + " " + thread.TotalProcessorTime + "\n";
                        }
                    }
                    catch
                    {
                        ThreadsBlock.Text = "Отказано в доступе";
                    }
                }
            }
        }

        private void NotepadButton_Click(object sender, RoutedEventArgs e)
        {
            var notepad = Process.Start("notepad.exe");
            Task.Run(() =>
            {
                Task.Delay(3000).ContinueWith(t => notepad.Kill());
            });
        }

        private void ExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private void ITSTEPButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("MicrosoftEdge.exe", "http://itstep.org");
        }

        #endregion
    }
}
