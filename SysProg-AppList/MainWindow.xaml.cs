using SysProg_AppList._1_Multithreading;
using SysProg_AppList._2_Tasks;
using SysProg_AppList._3_Processes;
using System.Windows;

namespace SysProg_AppList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ButtonMultithreading_Click(object sender, RoutedEventArgs e)
        {
            new MultithreadingWindow().ShowDialog();
        }

        private void ButtonMultithreadingHomework_Click(object sender, RoutedEventArgs e)
        {
            new Task1Window().ShowDialog();
        }


        private void ButtonTasks_Click(object sender, RoutedEventArgs e)
        {
            new TasksWindow().ShowDialog();
        }

        private void ButtonTasksHomework_Click(object sender, RoutedEventArgs e)
        {
            new Task2Window().ShowDialog();
        }


        private void ButtonProcesses_Click(object sender, RoutedEventArgs e)
        {
            new ProcessesWindow().ShowDialog();
        }
    }
}
