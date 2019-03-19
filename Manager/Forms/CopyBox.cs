using System;
using System.Windows.Forms;


namespace SPBU12._1MANAGER
{
    public partial class CopyBox : Form
    {
        string rootTo;

        public CopyBox(string root, string text)
        {
            InitializeComponent();

            label1.Text = text;         
            label2.Text = root;
        }

        //public string Root {
        //    get {
        //        return comboBox1.Text;
        //    }
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void CopyBox_Load(object sender, EventArgs e)
        {

        }
    }
}
