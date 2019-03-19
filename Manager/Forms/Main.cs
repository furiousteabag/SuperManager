using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace SPBU12._1MANAGER
{
    public partial class Main : Form, IView
    {
        /*
         * Variables initializing
         */

        public UserData data { get; set; }

        public string login { get; set; }
        public string password { get; set; }
        public string rootUser { get; set; }

        public string txtStatisticsFilePath { get; set; }

        public Dictionary<string, string> dirs { get; set; }

        public bool isChanged1 { get; set; }
        public bool isChanged2 { get; set; }

        public string rootLeft { get; set; }
        public string rootRight { get; set; }

        public FileSystemWatcher watcherLeft { get; set; }
        public FileSystemWatcher watcherRight { get; set; }

        public ListElements listLeft { get; set; }
        public ListElements listRight { get; set; }

        public ListView lwOnline { get; set; }
        public ListView getlistView1 { get { return listView1; } set { } }
        public ListView getlistView2 { get { return listView2; } set { } }
        public ComboBox getcomboBox1 { get { return comboBox1; } set { } }
        public ComboBox getcomboBox2 { get { return comboBox2; } set { } }
        public TextBox gettextBox4 { get { return textBox4; } set { } }
        public TextBox gettextBox5 { get { return textBox5; } set { } }

        /*
         * Initializing interface events.
         */

        public event EventHandler helpToolStripMenuItem_Click_interface;
        public event EventHandler comboBox1_SelectedValueChanged_interface;
        public event EventHandler comboBox2_SelectedValueChanged_interface;
        public event EventHandler buttonLeftRoot_Click_interface;
        public event EventHandler buttonLeftPrev_Click_inteface;
        public event EventHandler buttonRightRoot_Click_interface;
        public event EventHandler buttonRightPrev_Click_inteface;
        public event EventHandler listView1_DoubleClick_interface;
        public event EventHandler listView2_DoubleClick_interface;
        public event FileSystemEventHandler UpdateLeft_interface;
        public event FileSystemEventHandler UpdateRight_interface;
        public event KeyEventHandler File_Statistics_interface;
        public event EventHandler saveSettingsToolStripMenuItem_Click_interface;

        // Form initialization.
        public Main()
        {
            InitializeComponent();

            // Login box.
            Initialization();

            // Attaching presenter.
            Presenter presenter = new Presenter(this);

            // Initializing lists of elements.
            listLeft = new ListElements();
            listRight = new ListElements();

            // Initializing dictionary.
            dirs = new Dictionary<string, string>();

            // Initializing watchers.
            WatchersInitialize();

            // Filling the comboboxes.
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo info in drives)
            {
                try
                {
                    comboBox1.Items.Add(info.Name);
                    comboBox2.Items.Add(info.Name);
                }
                catch { }
            }

            // Setting text to combobox.
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();

            // Current form - left form.
            lwOnline = listView1;
        }



        // Return path of listView.
        public string PathOfListView(ListView lw)
        {
            if (lw == listView1)
                return rootLeft;
            return rootRight;
        }

        // Returns the curent path of another listview
        private string PathOfSecondListView(string startDir)
        {
            if (startDir == rootLeft)
                return rootRight;
            return rootLeft;
        }






        // Tick if left form has changed.
        private void UpdateLeft(object sendler, FileSystemEventArgs e)
        {
            UpdateLeft_interface(sendler, e);
        }

        // Tick if right form has changed.
        private void UpdateRight(object sendler, FileSystemEventArgs e)
        {
            UpdateRight_interface(sendler, e);
        }



        //Список элементов на форме
        private ListElements ListE(ListView lw)
        {
            if (lw == listView1)
                return listLeft;
            return listRight;
        }

        delegate TextBox TB();

        //Обновление формы
        public void UpdateListView(ListView lw)
        {
            if (lw.SelectedItems.Count > 0 && lw == WhichListView() && Path.GetFileName(lw.SelectedItems[0].Text) != lw.SelectedItems[0].Text)
            {
                if (lw == listView1)
                    rootLeft = lw.SelectedItems[0].Text;
                else
                    rootRight = lw.SelectedItems[0].Text;
            }

            ListE(lw).Update(PathOfListView(lw));
            lw.Items.Clear();

            foreach (ListViewItem item in ListE(lw).list)
            {
                lw.Items.Add(item);
                if ((item.Index % 2) == 0)
                    item.BackColor = data.color1;
                else
                    item.BackColor = data.color2;
                item.ForeColor = data.fontColor;
            }

            TB tb = new TB(() =>
            {
                if (lw == listView1)
                    return textBox4;
                return textBox5;
            });

            tb.Invoke().Text = PathOfListView(lw);

            if (lw == listView1)
            {
                watcherLeft.Path = rootLeft;
                watcherLeft.EnableRaisingEvents = true;
            }
            else
            {
                watcherRight.Path = rootRight;
                watcherRight.EnableRaisingEvents = true;
            }
        }


        

        //Меняет имя пути в правом или левом окне
        public void PathName(string name, bool left)
        {
            name = name.Substring(0, name.IndexOf(" "));
            if (left)
                rootLeft = name;
            else
                rootRight = name;
        }

        //Смена диска (левое окно)
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedValueChanged_interface(sender, e);
        }
        //Смена диска (правое окно)
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            comboBox2_SelectedValueChanged_interface(sender, e);
        }

        //Обработка двойного нажатия на элементы первой формы
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            listView1_DoubleClick_interface(sender, e);
        }
        //Обработка двойного нажатия на элементы первой формы
        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            listView2_DoubleClick_interface(sender, e);
        }

        //Кнопки возврата в левом окне
        private void button4_Click(object sender, EventArgs e)
        {
            buttonLeftPrev_Click_inteface(sender, e);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            buttonLeftRoot_Click_interface(sender, e);
        }

        //Кнопки возврата в правом окне
        private void button5_Click(object sender, EventArgs e)
        {
            buttonRightPrev_Click_inteface(sender, e);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            buttonRightRoot_Click_interface(sender, e);
        }

        ////Переместить элемент
        //private void MoveElement(ListView lw, string el)
        //{
        //    Entity.CopyFileOrFolder(PathOfListView(lw), PathOfSecondListView(PathOfListView(lw)), el);
        //    Entity.DeleteElement(PathOfListView(lw), el);
        //}

        ////Переместить файлы
        //private void MoveF()
        //{
        //    var lw = WhichListView();
        //    if (ShowWindCopy(PathOfSecondListView(PathOfListView(lw)), "Rename/move " + lw.SelectedItems.Count + " file(s) to:"))
        //    {
        //        foreach (ListViewItem item in lw.SelectedItems)
        //        {
        //            MoveElement(lw, item.Text + item.SubItems[1].Text);
        //        }
        //    }
        //}
        













        


















        //Выбрано ли что-нибудь
        private bool IsSelected()
        {
            return (listView1.SelectedItems.Count > 0 || listView2.SelectedItems.Count > 0);
        }

        //Выбран ли один элемент
        private bool IsSelectedOne()
        {
            return (listView1.SelectedItems.Count == 1 || listView2.SelectedItems.Count == 1);
        }

        //Какой листвью выбран
        public ListView WhichListView()
        {
            if (listView2.Focused)
            {
                return listView2;
            }
            return listView1;
        }

        //Для watcher-a
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isChanged1)
            {
                UpdateListView(listView1);
                isChanged1 = false;
            }
            if (isChanged2)
            {
                UpdateListView(listView2);
                isChanged2 = false;
            }

            if (IsSelected() && lwOnline != WhichListView())
            {
                lwOnline = WhichListView();
            }
        }



        //Закрытие формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettingsToolStripMenuItem_Click_interface(sender, e);
        }



        //ОБРАБОТКА СОЧЕТАНИЙ КЛАВИШ И ОБЫЧНЫХ НАЖАТИЙ

        //// Search patterns button
        //private void findToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (IsSelected())
        //    {
        //        ListViewItem selectFile = WhichListView().SelectedItems[0];
        //        FileInfo fileInfo = new FileInfo(selectFile.Text);
        //        string fName = PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + selectFile.Text;
        //        string dName = PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + selectFile.Text.Substring(1).Remove(selectFile.Text.Length - 2);

        //        // File path (if exists).
        //        DirectoryInfo di = new DirectoryInfo(PathOfListView(WhichListView()));
        //        FileInfo[] smFiles = di.GetFiles(selectFile.Text + "*");
        //        string pathfile = "";
        //        if (smFiles.Count() > 0)
        //        {
        //            pathfile = smFiles[0].FullName;
        //        }

        //        (new SearchPatternsParallels(new SearchHandler())).Search(pathfile, dName);
        //    }
        //}

        //Сочетания
        private void Main_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsSelected())
                    {
                        if (WhichListView() == listView1)
                            listView1_DoubleClick_interface(sender, e);
                        else listView2_DoubleClick_interface(sender, e);
                    }

                }
                else if (e.Alt && e.KeyCode == Keys.F1)
                {
                    comboBox1.Focus();
                    comboBox1.DroppedDown = true;
                }
                else if (e.Alt && e.KeyCode == Keys.F2)
                {
                    comboBox2.Focus();
                    comboBox2.DroppedDown = true;
                }
                ////Pack button
                //else if (e.Alt && e.KeyCode == Keys.F5)
                //{
                //    if (IsSelectedOne())
                //    {
                //        ArchiveFile file = new ArchiveFile();

                //        // A path of file to pack
                //        var lw = WhichListView();
                //        ListViewItem fileChosen = lw.SelectedItems[0];
                //        string path = PathOfListView(WhichListView()) + Entity.GetDirectorySeparatorChar() + fileChosen.Text;

                //        file.Pack(path);
                //    }
                //}
                //else if (e.Alt && e.KeyCode == Keys.F6)
                //{
                //    if (IsSelectedOne()) { }
                //    //  Unpack();
                //}
                else if (e.KeyCode == Keys.F1)
                {
                    helpToolStripMenuItem_Click_interface(sender, e);
                }
                else if (e.KeyCode == Keys.F2)
                {
                    if (IsSelected())
                        UpdateListView(WhichListView());
                }
                //else if (e.KeyCode == Keys.F5)
                //{
                //    if (IsSelected())
                //        ShowCopy();
                //}
                //else if (e.KeyCode == Keys.F6)
                //{
                //    if (IsSelected()) { }
                //    // MoveF();
                //}
                else if (e.KeyCode == Keys.Delete)
                {
                    if (IsSelected())
                    {
                        var lw = WhichListView();
                        if (ShowWindDelete(lw.SelectedItems.Count))
                        {
                            foreach (ListViewItem item in lw.SelectedItems)
                            {
                                string name = item.Text + item.SubItems[1].Text;
                                Presenter.Delete(PathOfListView(lw), name);
                            }
                        }
                    }

                }
                else if (e.KeyCode == Keys.F7)
                {
                    if (IsSelectedOne())
                    {
                        var lw = WhichListView();
                        ListViewItem file = lw.SelectedItems[0];
                        //Creating a file path
                        string filePath = PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + file.Text.Substring(0) + ".txt";
                        txtStatisticsFilePath = filePath;
                        File_Statistics_interface(sender, e);
                    }

                }
                else if (e.KeyCode == Keys.F8)
                {
                    var lw = WhichListView();
                    ListViewItem fileChosen = lw.SelectedItems[0];

                    // Folder path.
                    string path = (PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + fileChosen.Text).Replace("[", "").Replace("]", "");

                    //// File path (if exists).
                    //DirectoryInfo di = new DirectoryInfo(PathOfListView(WhichListView()));
                    //FileInfo[] smFiles = di.GetFiles(fileChosen.Text + "*");

                    var smFiles = FolderMethods.GetR(PathOfListView(WhichListView()), fileChosen.Text + "*");
                    string pathfile = "";
                    if (smFiles.Count() > 0)
                    {
                        pathfile = smFiles[0].FullName;
                    }

                    // If folder.
                    if (FolderMethods.IfExist(path))
                    {
                        FolderMethods.md5hash(path);
                    }
                    // If file.
                    else if (FileMethods.IfExist(pathfile))
                    {
                        FileMethods.md5hash(pathfile);
                    }
                    // If something else.
                    else MessageBox.Show(
                        "Not a file or folder",
                        "MD-5 hash",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                                   );

                }
                else if (e.KeyCode == Keys.F9)
                {
                    var lw = WhichListView();
                    ListViewItem fileChosen = lw.SelectedItems[0];

                    // Folder path.
                    string path = (PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + fileChosen.Text).Replace("[", "").Replace("]", "");

                    // File path (if exists).
                    //DirectoryInfo di = new DirectoryInfo(PathOfListView(WhichListView()));
                    //FileInfo[] smFiles = di.GetFiles(fileChosen.Text + "*");
                    var smFiles = FolderMethods.GetR(PathOfListView(WhichListView()), fileChosen.Text + "*");
                    string pathfile = "";
                    if (smFiles.Count() > 0)
                    {
                        pathfile = smFiles[0].FullName;
                    }

                    // If folder.
                    if (FolderMethods.IfExist(path))
                    {
                        FolderMethods.cypher(path);
                    }
                    // If file.
                    else if (FileMethods.IfExist(pathfile))
                    {
                        FileMethods.cypher(pathfile);
                    }
                    // If something else.
                    else MessageBox.Show(
                        "Not a file or folder",
                        "MD-5 hash",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                                   );

                }
                else if (e.KeyCode == Keys.F10)
                {
                    var lw = WhichListView();
                    ListViewItem fileChosen = lw.SelectedItems[0];

                    // Folder path.
                    string path = (PathOfListView(WhichListView()) + Path.DirectorySeparatorChar + fileChosen.Text).Replace("[", "").Replace("]", "");

                    // File path (if exists).
                    var smFiles = FolderMethods.GetR(PathOfListView(WhichListView()), fileChosen.Text + "*");
                    string pathfile = "";
                    if (smFiles.Count() > 0)
                    {
                        pathfile = smFiles[0].FullName;
                    }

                    // If folder.
                    if (FolderMethods.IfExist(path))
                    {
                        FolderMethods.decypher(path);
                    }
                    // If file.
                    else if (FileMethods.IfExist(pathfile))
                    {
                        FileMethods.decypher(pathfile);
                    }
                    // If something else.
                    else MessageBox.Show(
                        "Not a file or folder",
                        "MD-5 hash",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                                   );

                }
                else if (e.Alt && e.KeyCode == Keys.Left)
                {
                    if (WhichListView() == listView1)
                    {
                        if (Path.GetDirectoryName(rootLeft) != null)
                            rootLeft = Path.GetDirectoryName(rootLeft);
                        UpdateListView(listView1);
                    }
                    else if (WhichListView() == listView2)
                    {
                        if (Path.GetDirectoryName(rootRight) != null)
                            rootRight = Path.GetDirectoryName(rootRight);
                        UpdateListView(listView2);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Загрузка
        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWindDownload();
        }
        //Переместить
        private void move_Click(object sender, EventArgs e)
        {
            if (IsSelected()) { }
            //MoveF();
        }
        ////Cкопировать
        //private void copy_Click(object sender, EventArgs e)
        //{
        //    if (IsSelected())
        //        ShowCopy();
        //}
        //Удалить
        private void delete_Click(object sender, EventArgs e)
        {
            if (IsSelected())
            {
                var lw = WhichListView();
                if (ShowWindDelete(lw.SelectedItems.Count))
                {
                    foreach (ListViewItem item in lw.SelectedItems)
                    {
                        string name = item.Text + item.SubItems[1].Text;
                        Presenter.Delete(PathOfListView(lw), name);
                    }
                }

            }

        }
        //Обработка кнопки help
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            helpToolStripMenuItem_Click_interface(sender, e);
        }

        ////Обработка кнопки разархивации
        //private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (IsSelectedOne())
        //        Unpack();
        //}

        ////Обработка кнопки options
        //private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Configuration conf = new Configuration(data.fontColor, data.color1, data.color2, data.fileFont, data.mainFont, data.dialogFont);
        //    conf.Font = data.dialogFont;
        //    DialogResult res = conf.ShowDialog();

        //    if (res == DialogResult.OK)
        //    {
        //        data.fontColor = conf.FontColor();
        //        data.color1 = conf.Color1();
        //        data.color2 = conf.Color2();
        //        data.fileFont = conf.FileFont();
        //        data.mainFont = conf.MainFont();
        //        data.dialogFont = conf.DialogFont();

        //        this.Font = data.mainFont;
        //        listView1.BackColor = data.color1;
        //        listView2.BackColor = data.color1;
        //        listView1.Font = data.fileFont;
        //        listView2.Font = data.fileFont;
        //        UpdateListView(listView1);
        //        UpdateListView(listView2);
        //    }
        //}
        //Обработка кнопки поиска файла
        private void searchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWindSearch();
        }
        //Обработка кнопки сохранения
        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveSettingsToolStripMenuItem_Click_interface(sender, e);
        }
        //Обработка кнопки закрытия
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /*
         * Show windows.
         */

        //Вывод окна копирования
        private bool ShowWindCopy(string root, string label)
        {
            CopyBox cb = new CopyBox(root, label);
            cb.Font = data.dialogFont;
            if (cb.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }

        //Вывод окна загрузки
        private void ShowWindDownload()
        {
            DownloadBox cb = new DownloadBox();
            cb.Font = data.dialogFont;
            cb.Show();

        }

        //Вывод окна поиска файлов
        private void ShowWindSearch()
        {
            SearchBox cb = new SearchBox();
            cb.Font = data.dialogFont;
            cb.Show();

        }

        //Окно удаления
        private bool ShowWindDelete(int k)
        {
            if (MessageBox.Show("Do you really want to delete " + k + " file(s)?", "Custom file manager", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK)
                return true;
            return false;
        }



        /*
         * 
         * Form methods.
         * 
         */

        // Initializing form with user information.
        private void Initialization()
        {
            Start start = new Start();
            DialogResult r = start.ShowDialog();

            if (r == DialogResult.Yes)
            {
                if (start.Login != "" && start.Password != "")
                {
                    data = new UserData();
                    data.login = start.Login;
                    data.password = start.Password;
                    rootUser = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    rootUser += Entity.GetDirectorySeparatorChar() + data.login + ".dat";
                }
                else
                {
                    MessageBox.Show("Enter something");
                    Initialization();
                }
            }
            else if (r == DialogResult.OK)
            {
                login = start.Login;
                password = start.Password;
                rootUser = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                rootUser += Entity.GetDirectorySeparatorChar() + login + ".dat";

                if (!File.Exists(rootUser))
                {
                    MessageBox.Show("Incorrect login");
                    Initialization();
                }
                else
                {
                    BinaryFormatter binFormat = new BinaryFormatter();
                    Stream fStream = File.Open(rootUser, FileMode.Open);
                    data = (UserData)binFormat.Deserialize(fStream);
                    fStream.Close();

                    if (data.password != password)
                    {
                        MessageBox.Show("Incorrect password");
                        Initialization();
                    }
                }
            }
            else if (r == DialogResult.Cancel)
            {
                Environment.Exit(0);
            }

            UpdateForm();
        }

        // Trasfering user data to form.
        private void UpdateForm()
        {
            this.password = data.password;
            this.Font = data.mainFont;
            listView1.BackColor = data.color1;
            listView2.BackColor = data.color1;
            listView1.Font = data.fileFont;
            listView2.Font = data.fileFont;
        }

        // Initializing watchers.
        private void WatchersInitialize()
        {
            timer1.Interval = 10;
            timer1.Tick += timer1_Tick;
            timer1.Enabled = true;


            watcherLeft = new FileSystemWatcher();
            watcherRight = new FileSystemWatcher();

            watcherLeft.Changed += UpdateLeft;
            watcherLeft.Created += UpdateLeft;
            watcherLeft.Deleted += UpdateLeft;
            watcherLeft.Renamed += UpdateLeft;

            watcherRight.Changed += UpdateRight;
            watcherRight.Created += UpdateRight;
            watcherRight.Deleted += UpdateRight;
            watcherRight.Renamed += UpdateRight;

            isChanged1 = false;
            isChanged2 = false;
        }

        ////Разархивация
        //private void Unpack()
        //{
        //    var lw = WhichListView();
        //    ListViewItem file = lw.SelectedItems[0];
        //    if (file.SubItems[1].Text != ".zip")
        //    {
        //        MessageBox.Show(
        //               "Operation Cancelled: not *.zip file.",
        //               "Unpack status",
        //               MessageBoxButtons.OK,
        //               MessageBoxIcon.Error
        //                          );
        //        return;
        //    }
        //    string path = PathOfListView(lw) + Entity.GetDirectorySeparatorChar() + file.Text + file.SubItems[1].Text;
        //    FolderMethods.Unzip(path, path.Substring(0, path.Length - 4));
        //}

    }


}
//// Method to copy (cannot move because of showwind)
//private void ShowCopy()
//{
//    var lw = WhichListView();
//    if (ShowWindCopy(PathOfSecondListView(PathOfListView(lw)), "Copy " + lw.SelectedItems.Count + " file(s) to:"))
//    {
//        foreach (ListViewItem file in lw.SelectedItems)
//        {
//            Entity.CopyFileOrFolder(PathOfListView(lw), PathOfSecondListView(PathOfListView(lw)), file.Text + file.SubItems[1].Text);
//        }
//    }
//}