using System.Windows.Forms;

namespace SPBU12._1MANAGER
{
    public partial class HelpBox : Form
    {
        public HelpBox()
        {
            InitializeComponent();

            listView1.Items.Add(new ListViewItem(new string[] { "F1", "Help" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F5", "Copy files" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F6", "Rename or move files" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F7", "txt file statistics" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F8", "MD5 hash" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F9", "Cypher" }));
            listView1.Items.Add(new ListViewItem(new string[] { "F10", "De-Cypher" }));
            listView1.Items.Add(new ListViewItem(new string[] { "Alt + F5", "Pack" }));
            listView1.Items.Add(new ListViewItem(new string[] { "Alt + F6", "Unpack" }));
            listView1.Items.Add(new ListViewItem(new string[] { "Delete", "Delete" }));
            listView1.Items.Add(new ListViewItem(new string[] { "Alt + F4", "Exit" }));
        }
    }
}
