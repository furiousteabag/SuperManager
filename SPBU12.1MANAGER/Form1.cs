using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;

namespace SPBU12._1MANAGER
{
    public partial class Form1 : Form
    {
        //Инициализация переменных
        private string rootLeft, rootRight;
        Dictionary<string, string> dirs;
        ListElements listLeft, listRight;
        FileSystemWatcher watcherLeft, watcherRight;
        bool isChanged1, isChanged2;

        ListView lwOnline;

        public static UserData data;
        private string login, password, rootUser;

        //Окно логина
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
                    rootUser += Path.DirectorySeparatorChar + data.login + ".dat";
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
                rootUser += Path.DirectorySeparatorChar + login + ".dat";

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

        //Передача кастомных данных в форму
        private void UpdateForm()
        {
            this.password = data.password;
            this.Font = data.mainFont;
            listView1.BackColor = data.color1;
            listView2.BackColor = data.color1;
            listView1.Font = data.fileFont;
            listView2.Font = data.fileFont;
        }

        //watcher - онлайн обновление директорий
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

        //Инициализация формы
        public Form1()
        {
            InitializeComponent();
            Initialization();

            WatchersInitialize();

            dirs = new Dictionary<string, string>();

            listLeft = new ListElements();
            listRight = new ListElements();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo info in drives)
            {
                try
                {
                    comboBox1.Items.Add(info.Name + "        " + info.VolumeLabel);
                    comboBox2.Items.Add(info.Name + "        " + info.VolumeLabel);
                }
                catch
                {
                }
            }

            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();

            lwOnline = listView1;
        }

        //Апдейт левой формы
        private void UpdateLeft(object sendler, FileSystemEventArgs e)
        {
            isChanged1 = true;
        }

        //Апдейт правой формы
        private void UpdateRight(object sendler, FileSystemEventArgs e)
        {
            isChanged2 = true;
        }

        //Директория формы
        private string Root(ListView lw)
        {
            if (lw == listView1)
                return rootLeft;
            return rootRight;
        }

        //Список элементов на форме
        private ListElements ListE(ListView lw)
        {
            if (lw == listView1)
                return listLeft;
            return listRight;
        }

        //Обновление формы
        private void UpdateListView(ListView lw)
        {
            if (lw.SelectedItems.Count > 0 && lw == WhichListView() && Path.GetFileName(lw.SelectedItems[0].Text) != lw.SelectedItems[0].Text)
            {
                if (lw == listView1)
                    rootLeft = lw.SelectedItems[0].Text;
                else
                    rootRight = lw.SelectedItems[0].Text;
            }

            ListE(lw).Update(Root(lw));
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

            tb.Invoke().Text = Root(lw);

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

        delegate TextBox TB();

        delegate DriveInfo Disk();

        //Меняет имя пути в правом или левом окне
        private void PathName(string name, bool left)
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
            PathName(comboBox1.Text, true);
            UpdateListView(listView1);
        }
        //Смена диска (правое окно)
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            PathName(comboBox2.Text, false);
            UpdateListView(listView2);
        }

        //Обработка двойного нажатия на элементы формы
        private void DoubleClickLV(ListView lw)
        {
            string path = ListE(lw).DoubleClick(lw, Root(lw));

            if (lw == listView1)
                rootLeft = path;
            else
                rootRight = path;

            UpdateListView(lw);
        }
        //Обработка двойного нажатия на элементы первой формы
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DoubleClickLV(listView1);
        }
        //Обработка двойного нажатия на элементы первой формы
        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            DoubleClickLV(listView2);
        }

        //Кнопки возврата в левом окне
        private void button4_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(rootLeft) != null)
                rootLeft = Path.GetDirectoryName(rootLeft);
            UpdateListView(listView1);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(rootLeft) != null)
                rootLeft = Path.GetPathRoot(rootLeft);
            UpdateListView(listView1);
        }

        //Кнопки возврата в правом окне
        private void button5_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(rootRight) != null)
                rootRight = Path.GetDirectoryName(rootRight);
            UpdateListView(listView2);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(rootRight) != null)
                rootRight = Path.GetPathRoot(rootRight);
            UpdateListView(listView2);
        }


        //Вызов окна help
        private void Help()
        {
            HelpBox hb = new HelpBox();
            hb.Font = data.dialogFont;
            hb.ShowDialog();
        }

        //Возвращает директорию диска в текущем окне
        private string EndDir(string startDir)
        {
            if (startDir == rootLeft)
                return rootRight;
            return rootLeft;
        }

        //Метод копирования директории
        private void CopyDir(string nameDir, string endDir)
        {
            Directory.CreateDirectory(endDir + Path.DirectorySeparatorChar + Path.GetFileName(nameDir));

            DirectoryInfo di = new DirectoryInfo(nameDir);
            DirectoryInfo[] directories = di.GetDirectories();
            FileInfo[] files = di.GetFiles();

            foreach (DirectoryInfo info in directories)
            {
                CopyDir(nameDir + Path.DirectorySeparatorChar + info.Name, endDir);
            }

            foreach (FileInfo info in files)
            {
                CopyFile(info.Name, nameDir, endDir);
            }
        }

        //Метод копирования файла
        private void CopyFile(string nameFile, string startDir, string endDir)
        {
            using (FileStream SourceStream = File.Open(startDir + Path.DirectorySeparatorChar + nameFile, FileMode.Open))
            {
                using (FileStream DestinationStream = File.Create(endDir + Path.DirectorySeparatorChar + nameFile))
                {
                    SourceStream.CopyTo(DestinationStream);
                }
            }
        }

        //Если существует - возвращает false
        private bool IsDir(string path)
        {
            return File.Exists(path) ? false : true;
        }

        //Вывод окна копирования
        private bool ShowWind(string root, string label)
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

        //Скопировать элемент
        private void CopyElement(ListView lw, string item)
        {
            string startDirectory = Root(lw);
            string endDirectory = EndDir(startDirectory);

            if (IsDir(startDirectory + Path.DirectorySeparatorChar + item))
                CopyDir(startDirectory + Path.DirectorySeparatorChar + item.Substring(1).Remove(item.Length - 2), endDirectory);
            else
                CopyFile(item, startDirectory, endDirectory);
        }

        //Скопировать файлы
        private void IsCopy(ListView lw)
        {
            if (ShowWind(EndDir(Root(lw)), "Copy " + lw.SelectedItems.Count + " file(s) to:"))
            {
                foreach (ListViewItem file in lw.SelectedItems)
                {
                    CopyElement(lw, file.Text + file.SubItems[1].Text);
                }
            }
        }







        //Переместить элемент
        private void MoveElement(ListView lw, string el)
        {
            CopyElement(lw, el);
            DeleteElement(Root(lw), el);
        }

        //Переместить файлы
        private void MoveF(ListView lw)
        {
            if (ShowWind(EndDir(Root(lw)), "Rename/move " + lw.SelectedItems.Count + " file(s) to:"))
            {
                foreach (ListViewItem item in lw.SelectedItems)
                {
                    MoveElement(lw, item.Text + item.SubItems[1].Text);
                }
            }
        }

        //Удалить элемент
        private void DeleteElement(string root, string name)
        {
            if (!IsDir(root + Path.DirectorySeparatorChar + name))
                File.Delete(root + Path.DirectorySeparatorChar + name);
            else
            {
                string pathDir = root + Path.DirectorySeparatorChar + name.Substring(1, name.Length - 2);
                Directory.Delete(pathDir, true);
            }
        }

        //Окно удаления
        private bool MBShowOK(int k)
        {
            if (MessageBox.Show("Do you really want to delete " + k + " file(s)?", "Custom file manager", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK)
                return true;
            return false;
        }

        //Удалить несколько выбранных элементов
        private void Delete(ListView lw)
        {
            if (MBShowOK(lw.SelectedItems.Count))
            {
                foreach (ListViewItem item in lw.SelectedItems)
                {
                    string name = item.Text + item.SubItems[1].Text;
                    DeleteElement(Root(lw), name);
                }
            }
        }

        //Переименовать элемент
        private void RenameElement(string newPath, string path)
        {
            if (IsDir(path))
                Directory.Move(path, newPath);
            else
                File.Move(path, newPath);
        }



        //Закрыть программу
        private void Exit()
        {
            Application.Exit();
        }

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
        private ListView WhichListView()
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

        //Сохранить настройки
        private void Save()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream(rootUser, FileMode.Create, FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, data);
            fStream.Close();
        }

        //Закрытие формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save();
        }



        //ОБРАБОТКА СОЧЕТАНИЙ КЛАВИШ И ОБЫЧНЫХ НАЖАТИЙ

        //Сочетания
        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsSelected())
                        DoubleClickLV(WhichListView());
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
                else if (e.Alt && e.KeyCode == Keys.F5)
                {
                    if (IsSelectedOne())
                        Pack(WhichListView());
                }
                else if (e.Alt && e.KeyCode == Keys.F6)
                {
                    if (IsSelectedOne())
                        Unpack(WhichListView());
                }
                else if (e.KeyCode == Keys.F1)
                {
                    Help();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    if (IsSelected())
                        UpdateListView(WhichListView());
                }
                else if (e.KeyCode == Keys.F5)
                {
                    if (IsSelected())
                        IsCopy(WhichListView());
                }
                else if (e.KeyCode == Keys.F6)
                {
                    if (IsSelected())
                        MoveF(WhichListView());
                }
                else if (e.KeyCode == Keys.F7)
                {

                }
                else if (e.KeyCode == Keys.F8 || e.KeyCode == Keys.Delete)
                {
                    if (IsSelected())
                        Delete(WhichListView());
                }
                else if (e.KeyCode == Keys.F9)
                {

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
            if (IsSelected())
                MoveF(WhichListView());
        }
        //Cкопировать
        private void copy_Click(object sender, EventArgs e)
        {
            if (IsSelected())
                Task.Factory.StartNew(() =>
                    IsCopy(WhichListView()));
        }
        //Удалить
        private void delete_Click(object sender, EventArgs e)
        {
            if (IsSelected())
                Delete(WhichListView());
        }
        //Обработка кнопки help
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help();
        }
        //Обработка кнопки архивации
        private void packToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (IsSelectedOne())
                PackFile(WhichListView());
        }
        //Обработка кнопки разархивации
        private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsSelectedOne())
                Unpack(WhichListView());
        }
        //Обработка кнопки options
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration conf = new Configuration(data.fontColor, data.color1, data.color2, data.fileFont, data.mainFont, data.dialogFont);
            conf.Font = data.dialogFont;
            DialogResult res = conf.ShowDialog();

            if (res == DialogResult.OK)
            {
                data.fontColor = conf.FontColor();
                data.color1 = conf.Color1();
                data.color2 = conf.Color2();
                data.fileFont = conf.FileFont();
                data.mainFont = conf.MainFont();
                data.dialogFont = conf.DialogFont();

                this.Font = data.mainFont;
                listView1.BackColor = data.color1;
                listView2.BackColor = data.color1;
                listView1.Font = data.fileFont;
                listView2.Font = data.fileFont;
                UpdateListView(listView1);
                UpdateListView(listView2);
            }
        }
        //Обработка кнопки поиска файла
        private void searchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWindSearch();
        }
        //Обработка кнопки сохранения
        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        //Обработка кнопки закрытия
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }



        //АРХИВАЦИЯ

        //Архивация (архивирует целую папку с таском)
        private void Pack(ListView lw)
        {
            ListViewItem file = lw.SelectedItems[0];
            string path = Root(lw) + Path.DirectorySeparatorChar + file.Text + file.SubItems[1].Text;

            Task.Factory.StartNew(() =>
            {
                if (File.Exists(path))
                {
                    Directory.CreateDirectory(path + "_ZIP");

                    File.Copy(path, path + "_ZIP" + Path.DirectorySeparatorChar + Path.GetFileName(path));

                    ZipFile.CreateFromDirectory(path + "_ZIP", path + ".zip");
                    Directory.Delete(path + "_ZIP", true);

                }
                else
                {
                    path = Root(lw) + Path.DirectorySeparatorChar + file.Text.Substring(1).Remove(file.Text.Length - 2);
                    if (Directory.Exists(path))
                        ZipFile.CreateFromDirectory(path, path + ".zip");
                }
            });
        }

        //Архивация (архивирует папку с файлами с асинхронной архивацией)
        private void PackFile(ListView lw)
        {
            ListViewItem file = lw.SelectedItems[0];
            string path = Root(WhichListView()) + Path.DirectorySeparatorChar + file.Text.Substring(1).Remove(file.Text.Length - 2);
            Directory.CreateDirectory(path + "_archived");

            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();

            Parallel.ForEach(files, (currentFile) =>
            {
                string pathFile = currentFile.FullName;
                string compressfile = path + "_archived" + "\\" + currentFile.ToString() + ".zip";
                WhichCompress(pathFile, compressfile);
            });

        }
        //Выбор алгоритма сжатия
        private void WhichCompress(string sourceFile, string compressedFile)
        {
            Compress(sourceFile, compressedFile);
        }
        //Инициализация асинхронного сжатия
        private async void InitializeAsyncCompress(string sourceFile, string compressedFile)
        {
            await CompressAsync(sourceFile, compressedFile);
        }

        //Метод для архивации одного обьекта
        public static void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }

        //Метод для архивации одного обьекта асинхронно
        public static Task CompressAsync(string sourceFile, string compressedFile)
        {
            return Task.Run(() =>
            {
                // поток для чтения исходного файла
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                {
                    // поток для записи сжатого файла
                    using (FileStream targetStream = File.Create(compressedFile))
                    {
                        // поток архивации
                        using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                        {
                            sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                            Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                                sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                        }
                    }
                }
            });
        }

        //Разархивация
        private void Unpack(ListView lw)
        {
            ListViewItem file = lw.SelectedItems[0];
            if (file.SubItems[1].Text != ".zip")
            {
                MessageBox.Show("Not *.zip!!!");
                return;
            }
            string path = Root(lw) + Path.DirectorySeparatorChar + file.Text + file.SubItems[1].Text;
            ZipFile.ExtractToDirectory(path, path.Substring(0, path.Length - 4));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }






        //ПОИСК

        //Обработка кнопки поиска
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsSelected())
                SearchInitsialization();
            //Task.Factory.StartNew(() => );
        }

        //Инициализация поиска
        private void SearchInitsialization()
        {
            ListViewItem selectFile = WhichListView().SelectedItems[0];
            string fName = Root(WhichListView()) + Path.DirectorySeparatorChar + selectFile.Text;
            string dName = Root(WhichListView()) + Path.DirectorySeparatorChar + selectFile.Text.Substring(1).Remove(selectFile.Text.Length - 2);

            Task.Factory.StartNew(() =>
            {
                if (Directory.Exists(dName))
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        Path.DirectorySeparatorChar + Path.GetFileName(dName) + "_PatternsSearchResults" + @".txt";
                    using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        WhichSearch(dName, file);

                    }
                }
                else if (File.Exists(fName))
                    MessageBox.Show("Берём данные только из целых папок или дисков");
            });

        }

        //Выбор алгоритма поиска
        private void WhichSearch(string path, StreamWriter file)
        {
            // Task.Factory.StartNew(() => SearhParallels(path, file));
            InitializeAsyncSearch(path, file);
        }


        
        //Инициализация асинхронного поиска
        private async void InitializeAsyncSearch(string path, StreamWriter file)
        {
            await SearhAsync(path, file);
        }

        //Параллельный поиск образца
        private void SearhParallels(string path, StreamWriter file)
        {

            Action action = () =>
             {
                 try
                 {

                     DirectoryInfo di = new DirectoryInfo(path);
                     DirectoryInfo[] directories = di.GetDirectories();
                     FileInfo[] files = di.GetFiles();

                     Parallel.ForEach(directories, (info) =>
                      {

                          Task.Factory.StartNew(() =>
                          {
                              SearhParallels(info.FullName, file);

                          });

                      });

                     Parallel.ForEach(files, (currentFile) =>
                     {
                         try
                         {
                             byte[] b = File.ReadAllBytes(currentFile.FullName);

                             UTF8Encoding temp = new UTF8Encoding(true);
                             Regex[] r = new Regex[4];
                             r[0] = new Regex(@"[-a-f0-9_.]+@{1}[-0-9a-z]+\.[a-z]{2,5}");
                             r[1] = new Regex(@"\d{4}\s\d{6}");
                             r[2] = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
                             r[3] = new Regex(@"(8|\+7)([\-\s])?(\(?\d{3}\)?[\-\s])?[\d\-\s]{7,20}");

                             string str;

                             for (int i = 0; i < 4; i++)
                                 foreach (Match m in r[i].Matches(temp.GetString(b)))
                                 {
                                     str = m.ToString();
                                     file.WriteLine(str);
                                 }

                         }
                         catch { }
                     });
                 }
                 catch { }
             };
            Invoke(action);
        }

        //Асинхронный поиск образца
        private Task SearhAsync(string path, StreamWriter file)
        {

            return Task.Run(() =>
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    DirectoryInfo[] directories = di.GetDirectories();
                    FileInfo[] files = di.GetFiles();

                    foreach (DirectoryInfo info in directories)
                    {
                        Task.Factory.StartNew(() =>
                        {
                             SearhAsync(info.FullName, file);
                        });
                    };


                    foreach (FileInfo currentFile in files)
                    {
                        try
                        {

                            byte[] b = File.ReadAllBytes(currentFile.FullName);

                            UTF8Encoding temp = new UTF8Encoding(true);
                            Regex[] r = new Regex[4];
                            r[0] = new Regex(@"[-a-f0-9_.]+@{1}[-0-9a-z]+\.[a-z]{2,5}");
                            r[1] = new Regex(@"\d{4}\s\d{6}");
                            r[2] = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
                            r[3] = new Regex(@"(8|\+7)([\-\s])?(\(?\d{3}\)?[\-\s])?[\d\-\s]{7,20}");

                            string str;

                            for (int i = 0; i < 4; i++)
                                foreach (Match m in r[i].Matches(temp.GetString(b)))
                                {
                                    str = m.ToString();
                                    file.WriteLine(str);
                                }

                        }
                        catch { }
                    };
                }
                catch { }
            });

        }
        

    //Непосредственно поиск образца через потоки
    private void SearhThread(string path, StreamWriter file)
    {
        Queue<string>[] queue = new Queue<string>[Environment.ProcessorCount];
        for (int i = 0; i < Environment.ProcessorCount; i++)
            queue[i] = new Queue<string>();

        AllFiles(queue, path, file);

        QueueProcessor[] proc = new QueueProcessor[Environment.ProcessorCount];
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            try
            {
                proc[i] = new QueueProcessor(queue[i], file);
                proc[i].BeginProcessData();
            }
            catch { }
        }

        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            proc[i].EndProcessData();
        }
    }
    private void AllFiles(Queue<string>[] queue, string path, StreamWriter file)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] directories = di.GetDirectories();
            FileInfo[] files = di.GetFiles();

            foreach (DirectoryInfo info in directories)
            {
                AllFiles(queue, path + Path.DirectorySeparatorChar + info.Name, file);
            }

            int k = 0;
            foreach (FileInfo info in files)
            {
                queue[k % Environment.ProcessorCount].Enqueue(path + Path.DirectorySeparatorChar + info.Name);
                k++;
            }
        }
        catch { }
    }












}

//Класс поиска паттернов
public class QueueProcessor
{
    private Queue<string> queue;
    private Thread thread;
    private StreamWriter file;

    public QueueProcessor(Queue<string> queue, StreamWriter file)
    {
        this.queue = queue;
        this.file = file;
        thread = new Thread(new ThreadStart(this.ThreadFunc));
    }

    public Thread TheThread
    {
        get
        {
            return thread;
        }
    }

    public void BeginProcessData()
    {
        thread.Start();
    }

    public void EndProcessData()
    {
        thread.Join();
    }

    private void ThreadFunc()
    {
        foreach (string path in queue)
            SearchFile(path, file);
    }

    private void SearchFile(string path, StreamWriter file)
    {
        try
        {
            byte[] b = File.ReadAllBytes(path);

            UTF8Encoding temp = new UTF8Encoding(true);
            Regex[] r = new Regex[4];
            r[0] = new Regex(@"[-a-f0-9_.]+@{1}[-0-9a-z]+\.[a-z]{2,5}");
            r[1] = new Regex(@"\d{4}\s\d{6}");
            r[2] = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
            r[3] = new Regex(@"(8|\+7)([\-\s])?(\(?\d{3}\)?[\-\s])?[\d\-\s]{7,20}");
            string str;

            for (int i = 0; i < 5; i++)
                foreach (Match m in r[i].Matches(temp.GetString(b)))
                {
                    str = m.ToString();
                    file.WriteLine(str);
                }
        }
        catch { }
    }
}
}
