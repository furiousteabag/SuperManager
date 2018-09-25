using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
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

        public WebClient webClient = new WebClient();

        //start button
        private void button1_Click(object sender, EventArgs e)
        {
            DownloadAsync(DownloadTextBox.Text);
            this.button1.Enabled = false;
            this.button2.Enabled = true;
        }

        //cancel button
        private void button2_Click(object sender, EventArgs e)
        {
            webClient.CancelAsync();
            this.Close();
        }


        //Загрузка файла
        private void DownloadAsync(string link)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
            webClient.DownloadFileAsync(new Uri(link), @"D:\testovaya papka\Downloads\sample.txt");
        
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
