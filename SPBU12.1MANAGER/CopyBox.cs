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
            rootTo = root;

            comboBox1.Items.Add(rootTo);
            comboBox1.Text = comboBox1.Items[0].ToString();
            var directories = FolderMethods.GetDirectoryInfos(rootTo);
            foreach (var info in directories)
            {
                comboBox1.Items.Add(rootTo + Entity.GetDirectorySeparatorChar() + info.Name);
            }
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
