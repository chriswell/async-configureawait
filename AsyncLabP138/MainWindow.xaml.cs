using System;
using System.Threading.Tasks;
using System.Windows;
using System.Net;

namespace AsyncLabP138
{
    public partial class MainWindow : Window
    {
        //ui: ui-thread
        //tp: threadpool-thread

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Reset()
        {
            Dispatcher.Invoke(() =>
            {
                MessageListBox.Items.Clear();
                ContentTextBox.Text = string.Empty;
            });
        }

        private void Message(string message)
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Dispatcher.Invoke(() =>
                MessageListBox.Items.Add($"ThreadId: {id}, {message}"));
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Message("LoadButton_Click()"); //ui
            Reset();
            var content = await DownloadPageAsync(); //-> ui

            Message("again LoadButton_Click()"); //ui
            ContentTextBox.Text = content;
        }

        private string Modify(string text)
        {
            Message("Modify()"); //tp
            return $"Downloaded: {Environment.NewLine}{text}";
        }

        private async Task<string> DownloadPageAsync()
        {
            Message("DownloadPageAsync()"); //ui
            var content = string.Empty;
            using (var client = new WebClient())
            {
                Message("before download"); //ui
                content = await client.DownloadStringTaskAsync("http://www.msdn.com")
                    .ConfigureAwait(continueOnCapturedContext: false); //-> tp
                Message("after download"); //tp
                content = Modify(content); //tp
            }

            return content;
        }
    }
}
