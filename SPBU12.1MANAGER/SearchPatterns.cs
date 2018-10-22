using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{

    //---Parallel pattern search---
    public class SearchPatternsParallels
    {
        private ISearchHandler itsSearchHandler = null;
        private Regex[] regularies = null;
        private string fileName;
        private string directoryName;

        public SearchPatternsParallels(ISearchHandler handler)
        {
            itsSearchHandler = handler;
        }

        // Search initializing
        public void Search(string fName, string dName)
        {
            regularies = itsSearchHandler.WhatToSearch();
            fileName = fName;
            directoryName = dName;

            if (Directory.Exists(dName))
            {
                string fileToWriteInto = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    Path.DirectorySeparatorChar + Path.GetFileName(dName) + "_PatternsSearchResults" + @".txt";
                using (StreamWriter file = new StreamWriter(fileToWriteInto, false, Encoding.UTF8))
                {
                    SearchParallels(dName, file);
                }
            }
            else if (File.Exists(fName))
                MessageBox.Show("Берём данные только из целых папок или дисков");
        }

        // Algorythm
        private void SearchParallels(string path, StreamWriter file)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] directories = di.GetDirectories();
                FileInfo[] files = di.GetFiles();

                foreach (var info in directories)
                {
                    SearchParallels(info.FullName, file);
                };

                foreach (var currentFile in files)
                {
                    try
                    {
                        itsSearchHandler.CheckAndWriteRegularities(file, regularies, currentFile);
                    }
                    catch { }
                };
            }
            catch { }
        }
    }

    //---Thread pattern search---
    public class SearchPatternsTreads
    {
        private ISearchHandler itsSearchHandler = null;
        private Regex[] regularies = null;
        private string fileName;
        private string directoryName;

        public SearchPatternsTreads(ISearchHandler handler)
        {
            itsSearchHandler = handler;
        }

        // Search initializing
        public void Search(string fName, string dName)
        {
            regularies = itsSearchHandler.WhatToSearch();
            fileName = fName;
            directoryName = dName;

            if (Directory.Exists(dName))
            {
                string fileToWriteInto = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    Path.DirectorySeparatorChar + Path.GetFileName(dName) + "_PatternsSearchResults" + @".txt";
                using (StreamWriter file = new StreamWriter(fileToWriteInto, false, Encoding.UTF8))
                {
                    SearhThread(dName, file);
                }
            }
            else if (File.Exists(fName))
                MessageBox.Show("Берём данные только из целых папок или дисков");
        }

        // Algorythm
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

    //---Interface---
    public interface ISearchHandler
    {
        Regex[] WhatToSearch();
        void CheckAndWriteRegularities(StreamWriter whereToWrite, Regex[] r, FileInfo file);       
    }

    //---Interface methods---
    public class SearchHandler : ISearchHandler
    {

        // Array of regularities to search
        public Regex[] WhatToSearch()
        {
            Regex[] r = new Regex[4];
            r[0] = new Regex(@"[-a-f0-9_.]+@{1}[-0-9a-z]+\.[a-z]{2,5}");
            r[1] = new Regex(@"\d{4}\s\d{6}");
            r[2] = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
            r[3] = new Regex(@"(8|\+7)([\-\s])?(\(?\d{3}\)?[\-\s])?[\d\-\s]{7,20}");
            return r;
        }

        // Algorythm that searcing results and writing them to file
        public void CheckAndWriteRegularities(StreamWriter whereToWrite, Regex[] regularies, FileInfo file)
        {
            byte[] b = File.ReadAllBytes(file.FullName);
            UTF8Encoding temp = new UTF8Encoding(true);
            string str;
            for (int i = 0; i < 4; i++)
                foreach (Match m in regularies[i].Matches(temp.GetString(b)))
                {
                    str = m.ToString();
                    whereToWrite.WriteLine(str);
                }
        }       
    }

    // Additions to thread search
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
