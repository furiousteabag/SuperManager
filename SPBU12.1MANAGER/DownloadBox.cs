using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{

    public partial class DownloadBox : Form
    {

        public DownloadBox()
        {
            InitializeComponent();
        }

        CancellationTokenSource cts;
        string filename;

        // Download button click.
        private void button1_Click(object sender, EventArgs e)

        {
            button1.Enabled = false;
            button2.Enabled = true;
            cts = new CancellationTokenSource();

            var progress = new Progress<string>(i => {
                label2.Text = ("Progress: " + i.ToString()); });

            Task taskA = Task.Run(() => Download(cts.Token, progress));
            
        }

        // Download.
        public async void Download(CancellationToken token, IProgress<string> progress)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
 
            // Opening a stream to file.
            WebRequest request = WebRequest.Create(DownloadTextBox.Text);
            request.Method = WebRequestMethods.File.DownloadFile;
            WebResponse response = await request.GetResponseAsync();
            var responseStream = Entity.GetWebStream(response);

            // A file to write to.
            filename = "downloaded.txt";
            var fs = FileMethods.GetFileStream(@"D:\Test folder\Downloads" + "//" + filename);

            // How many bytes.
            long responsize = response.ContentLength / 1024;

            byte[] buffer = new byte[1024];
            int size = 0;
            bool cancelStatus = false;
            int summary = 0;

            // Downloading buffers.
            while ((size = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                progress.Report((summary / 1024).ToString() + " from " + responsize);

                if (token.IsCancellationRequested == true)
                {
                    MessageBox.Show(
                        "Operation Cancelled",
                        "Operation status",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                                   );
                    cancelStatus = true;
                    break;
                }
                summary += size;
                fs.Write(buffer, 0, size);
            }

            // Closing file and connection.
            fs.Close();
            responseStream.Close();

            // If wasn't cancelled.
            if (!cancelStatus)
                MessageBox.Show(
                    "Download completed",
                    "Operation status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                                       );

            Invoke(new Action(() => { button1.Enabled = true; button2.Enabled = false; label2.Text = ""; DownloadTextBox.Text = ""; }));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            cts.Cancel();
        }

      



        //-----OLD DOWNLOAD-----

        //public WebClient webClient = new WebClient();

        ////start button
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    DownloadAsync(DownloadTextBox.Text);
        //    this.button1.Enabled = false;
        //    this.button2.Enabled = true;
        //}

        ////cancel button
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    webClient.CancelAsync();
        //    this.Close();
        //}


        ////Загрузка файла
        //private void DownloadAsync(string link)
        //{
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //    webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
        //    webClient.DownloadFileAsync(new Uri(link), @"D:\testovaya papka\Downloads\sample.txt");

        //}

        //void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    progressBar1.Value = e.ProgressPercentage;
        //}
    }
}
