using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private Process? _notepadInstance;
        public ObservableCollection<Process> Processes { get; set; }

        #endregion


        #region Constructor

        public ProcessesWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Processes = new();
            _notepadInstance = new();
        }

        #endregion


        #region Methods - Process List

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

        #endregion


        #region Methods - Notepad

        private void NotepadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _notepadInstance = Process.Start("notepad.exe", openFileDialog.FileName);
                _notepadInstance.EnableRaisingEvents = true;
                _notepadInstance.Exited += new EventHandler(KillNotepad);

                NotepadCloseButton.IsEnabled = true;
                NotepadButton.IsEnabled = false;
            }
        }

        private void NotepadCloseButton_Click(object sender, RoutedEventArgs e)
        {
            KillNotepad(sender, e);
        }

        private void KillNotepad(object sender, EventArgs e)
        {
            _notepadInstance!.Kill();
            Dispatcher.BeginInvoke(() => { NotepadCloseButton.IsEnabled = false; NotepadButton.IsEnabled = true; });
        }

        #endregion


        #region Methods - URL Field

        private void ButtonOpenURL_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", TextFieldURL.Text);
        }

        private void TextFieldURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Uri.IsWellFormedUriString(TextFieldURL.Text, UriKind.Absolute))
            {
                ButtonOpenURL.IsEnabled = true;
            }
            else
            {
                ButtonOpenURL.IsEnabled = false;
            }
        }

        #endregion


        #region Methods - Other processes

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
