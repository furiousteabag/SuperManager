using SPBU12._1MANAGER.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// bober bejit na peresdu azaza
namespace SPBU12._1MANAGER
{
    interface IView
    {

        // Elements.
        UserData data { get; set; }
        string rootUser { get; set; }
        bool isChanged1 { get; set; }
        bool isChanged2 { get; set; }
        string txtStatisticsFilePath { get; set; }
        ListView getlistView1 { get; set; }
        ListView getlistView2 { get; set; }
        ComboBox getcomboBox1 { get; set; }
        ComboBox getcomboBox2 { get; set; }
        string rootLeft { get; set; }
        string rootRight { get; set; }
        ListElements listLeft { get; set; }
        ListElements listRight { get; set; }
        FileSystemWatcher watcherLeft { get; set; }
        FileSystemWatcher watcherRight { get; set; }
        TextBox gettextBox4 { get; set; }
        TextBox gettextBox5 { get; set; }

        // EventHandlers.
        event EventHandler helpToolStripMenuItem_Click_interface;
        event EventHandler comboBox1_SelectedValueChanged_interface;
        event EventHandler comboBox2_SelectedValueChanged_interface;
        event EventHandler buttonLeftRoot_Click_interface;
        event EventHandler buttonLeftPrev_Click_inteface;
        event EventHandler buttonRightRoot_Click_interface;
        event EventHandler buttonRightPrev_Click_inteface;
        event EventHandler listView1_DoubleClick_interface;
        event EventHandler listView2_DoubleClick_interface;
        event KeyEventHandler File_Statistics_interface;
        event EventHandler saveSettingsToolStripMenuItem_Click_interface;
        event FileSystemEventHandler UpdateLeft_interface;
        event FileSystemEventHandler UpdateRight_interface;


    }

    class Presenter
    {
        private IView view;

        public Presenter(IView view)
        {
            this.view = view;

            // Attaching events to realizations.
            view.helpToolStripMenuItem_Click_interface += new EventHandler(Help);
            view.comboBox1_SelectedValueChanged_interface += new EventHandler(comboBox1_SelectedValueChanged);
            view.comboBox2_SelectedValueChanged_interface += new EventHandler(comboBox2_SelectedValueChanged);
            view.buttonLeftRoot_Click_interface += new EventHandler(buttonLeftRoot_Click);
            view.buttonLeftPrev_Click_inteface += new EventHandler(buttonLeftPrev_Click);
            view.buttonRightRoot_Click_interface += new EventHandler(buttonRightRoot_Click);
            view.buttonRightPrev_Click_inteface += new EventHandler(buttonRightPrev_Click);
            view.listView1_DoubleClick_interface += new EventHandler(listView1_DoubleClick);
            view.listView2_DoubleClick_interface += new EventHandler(listView2_DoubleClick);
            view.UpdateLeft_interface += new FileSystemEventHandler(UpdateLeft);
            view.UpdateRight_interface += new FileSystemEventHandler(UpdateRight);
            view.File_Statistics_interface += new KeyEventHandler(FileStatistics);
            view.saveSettingsToolStripMenuItem_Click_interface += new EventHandler(Save);
        }

        /*
         * 
         * Event handlers.
         * 
         */

        //Вызов окна help
        private void Help(object sender, EventArgs e)
        {
            HelpBox hb = new HelpBox();
            hb.Font = view.data.dialogFont;
            hb.ShowDialog();
        }

        //Смена диска (левое окно)
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            PathName(view.getcomboBox1.Text, true);
            UpdateListView(view.getlistView1);
        }

        //Смена диска (правое окно)
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            PathName(view.getcomboBox2.Text, false);
            UpdateListView(view.getlistView2);
        }

        //Кнопки возврата в левом окне
        private void buttonLeftPrev_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(view.rootLeft) != null)
                view.rootLeft = Path.GetDirectoryName(view.rootLeft);
            UpdateListView(view.getlistView1);
        }
        private void buttonLeftRoot_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(view.rootLeft) != null)
                view.rootLeft = Path.GetPathRoot(view.rootLeft);
            UpdateListView(view.getlistView1);
        }

        //Кнопки возврата в правом окне
        private void buttonRightPrev_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(view.rootRight) != null)
                view.rootRight = Path.GetDirectoryName(view.rootRight);
            UpdateListView(view.getlistView2);
        }
        private void buttonRightRoot_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(view.rootRight) != null)
                view.rootRight = Path.GetPathRoot(view.rootRight);
            UpdateListView(view.getlistView2);
        }

        //Обработка двойного нажатия на элементы первой формы
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DoubleClickLV(view.getlistView1);
        }
        //Обработка двойного нажатия на элементы первой формы
        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            DoubleClickLV(view.getlistView2);
        }

        //Апдейт левой формы
        private void UpdateLeft(object sendler, FileSystemEventArgs e)
        {
            view.isChanged1 = true;
        }

        //Апдейт правой формы
        private void UpdateRight(object sendler, FileSystemEventArgs e)
        {
            view.isChanged2 = true;
        }

        // File statistics.
        private void FileStatistics(object sendler, KeyEventArgs e)
        {
            StatisticsTXT(view.txtStatisticsFilePath);
        }

        //Сохранить настройки
        private void Save(object sendler, EventArgs e)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream(view.rootUser, FileMode.Create, FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, view.data);
            fStream.Close();
        }

        /*
         * 
         * Methods.
         * 
         */


        //Меняет имя пути в правом или левом окне
        public void PathName(string name, bool left)
        {
            //name = name.Substring(0, name.IndexOf(" "));
            if (left)
                view.rootLeft = name;
            else
                view.rootRight = name;
        }

        //Обновление формы
        private void UpdateListView(ListView lw)
        {

            if (lw.SelectedItems.Count > 0 && lw == WhichListView() && FileMethods.GetName(lw.SelectedItems[0].Text) != lw.SelectedItems[0].Text)
            {
                if (lw == view.getlistView1)
                    view.rootLeft = lw.SelectedItems[0].Text;
                else
                    view.rootRight = lw.SelectedItems[0].Text;
            }

            ListE(lw).Update(Root(lw));
            lw.Items.Clear();


            var c = Factory.Get(ListE(lw).path);
            var d = new ZippedFile(ListE(lw).path);
            if (c == d)
            {
                ZippedFolder zippedFolder = new ZippedFolder(ListE(lw).path);
                List<string> files = zippedFolder.GetAllFiles();
                view.getlistView1.Items.Clear();
                foreach (var item in files)
                {
                    lw.Items.Add(item);
                }
            }
            else
            {
                foreach (ListViewItem item in ListE(lw).list)
                {
                    lw.Items.Add(item);
                    if ((item.Index % 2) == 0)
                        item.BackColor = view.data.color1;
                    else
                        item.BackColor = view.data.color2;
                    item.ForeColor = view.data.fontColor;
                }
            }

            TextBox tb = GetTextBoxFromListView(lw);

            tb.Text = Root(lw);

            try
            {
                if (lw == view.getlistView1)
                {
                    view.watcherLeft.Path = view.rootLeft;
                    view.watcherLeft.EnableRaisingEvents = true;
                }
                else
                {
                    view.watcherRight.Path = view.rootRight;
                    view.watcherRight.EnableRaisingEvents = true;
                }
            }
            catch { }
        }

        //Какой листвью выбран
        public ListView WhichListView()
        {
            if (view.getlistView2.Focused)
            {
                return view.getlistView2;
            }
            return view.getlistView1;
        }

        //Список элементов на форме
        private ListElements ListE(ListView lw)
        {
            if (lw == view.getlistView1)
                return view.listLeft;
            return view.listRight;
        }

        //Директория формы
        public string Root(ListView lw)
        {
            if (lw == view.getlistView1)
                return view.rootLeft;
            return view.rootRight;
        }

        // Choosing textbox which attached to selected listview
        private TextBox GetTextBoxFromListView(ListView lw)
        {
            if (lw == view.getlistView1)
                return view.gettextBox4;
            return view.gettextBox5;
        }

        //Обработка двойного нажатия на элементы формы
        private void DoubleClickLV(ListView lw)
        {
            string path = ListE(lw).DoubleClick(lw, Root(lw));

            if (FolderMethods.IfExist(path))
            {
                if (lw == view.getlistView1)
                    view.rootLeft = path;
                else
                    view.rootRight = path;
            }
            else
            {
                var c = Factory.Get(path);
                var d = new ZippedFile(path);

                if (c==d)
                {

                    if (lw == view.getlistView1)
                        view.rootLeft = path;
                    else
                        view.rootRight = path;

                }
                else
                {
                    Process.Start(path);
                }

            }


            UpdateListView(lw);
        }

        public static void Delete(string PathOfListView, string name)
        {
            Entity.DeleteElement(PathOfListView, name);
        }

        //---СТАТИСТИКА ПО ФАЙЛУ---

        private void StatisticsTXT(string filePath)
        {


            try
            {
                //Length of top words
                var topWordsLength = 5;

                //Starting work message
                MessageBox.Show(
                       "Starting work...",
                       "File statistics",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk
                                       );

                string output = "";
                var lineCount = 0;
                var unicWordCount = 0;



                //Initializing timer
                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[] b = File.ReadAllBytes(filePath);


                //Counting the number of lines
                Task taskA = Task.Run(() =>
                {
                    using (var reader = File.OpenText(filePath))
                    {
                        while (reader.ReadLine() != null)
                            lineCount++;
                    }
                    output += "Number of lines: " + lineCount + "\n";
                });

                //Number of words, number of unic words, top 10 words
                Task taskB = Task.Run(() =>
                {
                    //Creating an array of words
                    string textToAnalyse = Encoding.Default.GetString(b).ToLower().Replace(",", "").Replace(".", "").Replace("(", "").Replace(")", "").Replace("-", "");
                    string[] arrayOfWords = textToAnalyse.Split();
                    //Counting the number of words
                    output += "Number of words: " + arrayOfWords.Length + "\n";

                    //Counting the number of unic words
                    unicWordCount = (from word in arrayOfWords.AsParallel() select word).Distinct().Count();
                    output += "Number of unic words: " + unicWordCount + "\n";

                    //Top 10 words
                    var presortedList = arrayOfWords.GroupBy(s => s).Where(g => g.Count() > 1).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();
                    presortedList.Remove("");
                    var sortedList = (from word in presortedList where word.Length > topWordsLength select word);

                    var topTenWords = sortedList.Take(10);

                    output += "Top ten words with length > " + topWordsLength + ":\n";
                    int i = 1;
                    foreach (var word in topTenWords)
                    {
                        output += i + ") " + word + "\n";
                        i++;
                    }
                });

                //MessageBox
                var finalTask = Task.Factory.ContinueWhenAll(new Task[] { taskA, taskB }, ant =>
                {
                    stopwatch.Stop();
                    output += "Time: " + stopwatch.Elapsed;
                    MessageBox.Show(
                        output,
                        "File statistics",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }






    }
}
