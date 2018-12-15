using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{
    public partial class SearchBox : Form
    {
        private string root;
        Dictionary<string, string> dirs;
        ListElements list;

        // Watcher - нельзя убирать System.IO
        FileSystemWatcher watcher;
        bool isChanged;
        delegate TextBox TB();
        delegate DriveInfo Disk();

        public SearchBox()
        {
            InitializeComponent();
            WatchersInitialize();

            dirs = new Dictionary<string, string>();
            list = new ListElements();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo info in drives)
            {
                try
                {
                    comboBox1.Items.Add(info.Name + "        " + info.VolumeLabel);
                }
                catch { }
            }

            comboBox1.Text = comboBox1.Items[0].ToString();

        }

        //Поиск и отпечатка названий файлов и папок по маске
        private void Search(string directoryPath, ListElements list, ListView lw)
        {
            //Иногда не зайти в папку
            try
            {
                string[] pathsToFilteredDirectories = Directory.GetDirectories(directoryPath, textBox1.Text);
                string[] pathsToFilteredFiles = Directory.GetFiles(directoryPath, textBox1.Text);

                //Стандартный метод немного кривой, если печатаю точку, он выдаёт папки. Здесь я обнуляю массив папок если в маске есть точка
                if (textBox1.Text.Contains("."))
                    Array.Clear(pathsToFilteredDirectories, 0, pathsToFilteredDirectories.Length);

                //Масств для сливания массивов
                string[] FilteredThings = new string[pathsToFilteredDirectories.Length + pathsToFilteredFiles.Length];

                int k = 0;

                //Сливание: сначала вливаем папки, приписывая к ним скобочки (item.Text показывает папки именно так), потом файлы без разрешений
                for (int i = 0; i < pathsToFilteredDirectories.Length; i++)
                {
                    FilteredThings[i] = "[" + Path.GetFileNameWithoutExtension(pathsToFilteredDirectories[i]) + "]";
                }
                for (int i = pathsToFilteredDirectories.Length; i < pathsToFilteredDirectories.Length + pathsToFilteredFiles.Length; i++)
                {

                    FilteredThings[i] = Path.GetFileNameWithoutExtension(pathsToFilteredFiles[k]);
                    k++;
                }

                foreach (ListViewItem item in list.list)
                {
                    //Если элемент содержится в отсортированном массиве, то печатаем
                    if (FilteredThings.Contains(item.Text))
                    {
                        lw.Items.Add(item);
                    }
                }
            }
            catch { }
            
        }

        //Обновление формы
        private void UpdateListView(ListView lw)
        {
            if (lw.SelectedItems.Count > 0 && Path.GetFileName(lw.SelectedItems[0].Text) != lw.SelectedItems[0].Text)
            {
                root = lw.SelectedItems[0].Text;

            }

            list.Update(root);
            lw.Items.Clear();

            //Если поле ввода текста пусто то просто выводим всё как обычно
            if (textBox1.Text == "")
            {
                foreach (ListViewItem item in list.list)
                {
                    lw.Items.Add(item);
                }
            }
            //Если поле ввода текста не пусто
            else
            {
                //ОСНОВНАЯ ДИРЕКТОРИЯ

                Search(textBox4.Text, list, lw);

                //ВЛОЖЕННЫЕ ДИРЕКТОРИИ

                DirectoryInfo di = new DirectoryInfo(textBox4.Text);
                DirectoryInfo[] directories = di.GetDirectories();
                
                //Как распараллелить если разные потоки не могут отпечатывать в один и тот же listwiew
                foreach (DirectoryInfo info in directories)
                {
                    ListElements list1 = new ListElements();
                    list1.Update(info.FullName);
                    Search(info.FullName, list1, lw);
                };

            }

            TB tb = new TB(() =>
            {
                return textBox4;
            });

            tb.Invoke().Text = root;

            watcher.Path = root;
            watcher.EnableRaisingEvents = true;

        }

        //watcher - онлайн обновление директорий
        private void WatchersInitialize()
        {
            timer1.Interval = 10;
            timer1.Tick += timer1_Tick;
            timer1.Enabled = true;

            watcher = new FileSystemWatcher();

            watcher.Changed += Update;
            watcher.Created += Update;
            watcher.Deleted += Update;
            watcher.Renamed += Update;

            isChanged = false;
        }
        //Для watcher-a
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isChanged)
            {
                UpdateListView(listView1);
                isChanged = false;
            }

        }
        //Изменено ли что нибудь
        private void Update(object sendler, FileSystemEventArgs e)
        {
            isChanged = true;
        }

        //Выбрано ли что-нибудь
        private bool IsSelected()
        {
            return (listView1.SelectedItems.Count > 0);
        }
        //Выбран ли один элемент
        private bool IsSelectedOne()
        {
            return (listView1.SelectedItems.Count == 1);
        }

        //Меняет имя пути в правом или левом окне
        private void PathName(string name)
        {
            name = name.Substring(0, name.IndexOf(" "));
            root = name;

        }

        //Обработка двойного нажатия на элементы формы
        private void DoubleClickLV(ListView lw)
        {
            string path = list.DoubleClick(lw, root);
            root = path;
            UpdateListView(lw);
        }
        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            DoubleClickLV(listView1);
        }
        //Кнопки возврата в левом окне
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(root) != null)
                root = Path.GetPathRoot(root);
            UpdateListView(listView1);

        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(root) != null)
                root = Path.GetDirectoryName(root);
            UpdateListView(listView1);
        }

        //Обработка ввода паттерна
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateListView(listView1);
        }

        //Смена диска
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            PathName(comboBox1.Text);
            UpdateListView(listView1);
        }


    }
}
