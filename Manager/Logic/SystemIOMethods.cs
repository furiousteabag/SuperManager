using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.IO.MemoryMappedFiles;

namespace SPBU12._1MANAGER
{

    // Methods which are common to both file and folder.
    abstract class Entity
    {

        protected string path;

        public Entity(string path)
        {
            this.path = path;
        }

        // Returns GzipStream.
        public static GZipStream GetGZipStream(FileStream targetStream)
        {
            GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            return compressionStream;
        }

        // Returns directory separator char.
        public static char GetDirectorySeparatorChar()
        {
            return Path.DirectorySeparatorChar;
        }

        // Returns web responce stream.
        public static Stream GetWebStream(WebResponse response)
        {
            return response.GetResponseStream();
        }

        public string GetExtension()
        {
            string ext = Path.GetExtension(path);
            return ext;
        }

        // Move element.
        public static void MoveElement(string newPath, string path)
        {
            if (FolderMethods.IfExist(path))
                FolderMethods.MoveFolder(path, newPath);
            else
                FileMethods.MoveFile(path, newPath);
        }

        //Удалить элемент
        public static void DeleteElement(string root, string name)
        {
            if (FileMethods.IfExist(root + Path.DirectorySeparatorChar + name))
                File.Delete(root + Path.DirectorySeparatorChar + name);
            else if (name.Contains(".zip"))
            {
                string pathDir = root + Path.DirectorySeparatorChar + name;
                ZippedFolder.Delete(pathDir);
            }
            else
            {
                string pathDir = root + Path.DirectorySeparatorChar + name.Substring(1, name.Length - 2);
                Directory.Delete(pathDir, true);
            }
        }
        //Скопировать элемент
        public static void CopyFileOrFolder(string startDirectory, string endDirectory, string item)
        {
            if (FileMethods.IfExist(startDirectory + Entity.GetDirectorySeparatorChar() + item))
                FileMethods.CopyFile(item, startDirectory, endDirectory);
            else

                FolderMethods.CopyDir(startDirectory + Entity.GetDirectorySeparatorChar() + item.Substring(1).Remove(item.Length - 2), endDirectory);
        }




        public abstract void Accept(IVisitor visitor);



    }

    // Methods about files.
    class FileMethods : Entity
    {

        public string path;

        public FileMethods(string path) : base(path)
        {
            this.path = path;
        }

        // Returns the path of the file.
        public static string GetName(string path)
        {
            return Path.GetFileName(path);
        }

        // Delete the file.
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        // Move the file.
        public static void MoveFile(string pathFrom, string pathTo)
        {
            File.Move(pathFrom, pathTo);
        }

        // Returns the list of files from path.
        public static FileInfo[] GetFileInfos(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();
            return files;
        }

        // Returns Fileinfo.
        public static FileStream GetFileStream1(string sInputFilename)
        {
            FileStream fsInput = new FileStream(sInputFilename,
               FileMode.Open,
               FileAccess.Read);
            return fsInput;
        }

        // Returns Fileinfo.
        public static FileStream GetFileStream2(string sOutputFilename)
        {
            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create,
               FileAccess.Write);
            return fsEncrypted;
        }

        // Returns StreamWriter.
        public static StreamWriter GetStreamWriter(string sOutputFilename)
        {
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            return fsDecrypted;
        }

        // Returns StreamReader.
        public static StreamReader GetStreamReader(CryptoStream cryptostreamDecr)
        {
            StreamReader sr = new StreamReader(cryptostreamDecr);
            return sr;
        }

        // Returns the list of files from path with search filters from every podcategory.
        public static FileInfo[] GetFileInfosForSearch(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);
            return files;
        }

        // Returns filestream.
        public static FileStream GetFileStream(string pathfile)
        {
            FileStream fileStream = new FileStream(pathfile, FileMode.OpenOrCreate);
            return fileStream;
        }

        // Creates file and returns its filestream.
        public static FileStream GetTargetStream(string compressfile)
        {
            FileStream targetStream = File.Create(compressfile);
            return targetStream;
        }

        // Copies file to zipped folder.
        public static void CopyToZippedFolder(string pathfile)
        {
            File.Copy(pathfile, pathfile + "_ZIP" + Path.DirectorySeparatorChar + Path.GetFileName(pathfile));
        }

        // If the file exists.
        public static bool IfExist(string path)
        {
            return File.Exists(path);
        }

        // Read bytes.
        public static byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        //Метод копирования файла
        public static void CopyFile(string nameFile, string startDir, string endDir)
        {
            using (FileStream SourceStream = File.Open(startDir + Path.DirectorySeparatorChar + nameFile, FileMode.Open))
            {
                using (FileStream DestinationStream = File.Create(endDir + Path.DirectorySeparatorChar + nameFile))
                {
                    SourceStream.CopyTo(DestinationStream);
                }
            }
        }

        // Updates files list.
        public static void UpdateFiles(DirectoryInfo di, List<ListViewItem> list)
        {
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo info in files)
            {
                ListViewItem el = new ListViewItem(Path.GetFileNameWithoutExtension(info.Name));
                el.SubItems.Add(Path.GetExtension(info.Name));
                el.SubItems.Add((info.Length / 1000).ToString());
                el.SubItems.Add(info.CreationTime.ToString());
                list.Add(el);
            }
        }

        // Returns memorystream.
        public static MemoryStream Memory(byte[] encryptedTextBytes)
        {
            MemoryStream memoryStream = new MemoryStream(encryptedTextBytes);
            return memoryStream;
        }

        public static void copy(string from, string to, bool overwr)
        {
            File.Copy(from, to, overwr);
        }


        // Returns memorystream.
        public static MemoryStream Memory()
        {
            MemoryStream memoryStream = new MemoryStream();
            return memoryStream;
        }

        // MD5 hash.
        public static void md5hash(string path)
        {
            FileMethods p = new FileMethods(path);
            IVisitor visitor = new MD5hashVisitor();
            p.Accept(visitor);
        }

        // Cypher.
        public static void cypher(string path)
        {
            FileMethods p = new FileMethods(path);
            IVisitor visitor = new СypherVisitor();
            p.Accept(visitor);
        }

        // De-Cypher.
        public static void decypher(string path)
        {
            FileMethods p = new FileMethods(path);
            IVisitor visitor = new DeСypherVisitor();
            p.Accept(visitor);
        }

        // Visitor accept.
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }



        //// Returns streamwriter
        //public static StreamWriter GetStreamWriter(string fileToWriteInto) {
        //    StreamWriter file = new StreamWriter(fileToWriteInto, false, Encoding.UTF8);
        //    return file;
        //}



    }

    // Folder methods.
    class FolderMethods : Entity
    {

        public string path;

        public FolderMethods(string path) : base(path)
        {
            this.path = path;
        }

        // Returns the path of the folder.
        public static string GetName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        // Create directory.
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        // Deletes directory
        public static void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        // Move the folder.
        public static void MoveFolder(string pathFrom, string pathTo)
        {
            Directory.Move(pathFrom, pathTo);
        }

        // Move the folder.
        public static FileInfo[] GetR(string pathFrom, string pathTo)
        {
            DirectoryInfo di = new DirectoryInfo(pathFrom);
            FileInfo[] smFiles = di.GetFiles(pathTo);
            return smFiles;
        }
        

        // Creates a zip archive of a folder.
        public static void CreateZipFrom(string pathfile, string compressfile)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(pathfile, compressfile);
        }

        // Returns the list of directories from path.
        public static DirectoryInfo[] GetDirectoryInfos(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] directories = di.GetDirectories();
            return directories;
        }

        //Метод копирования директории
        public static void CopyDir(string nameDir, string endDir)
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
                FileMethods.CopyFile(info.Name, nameDir, endDir);
            }
        }

        // Updates Directory list.
        public static void UpdateDirectories(DirectoryInfo di, List<ListViewItem> list, string path)
        {
            if (path != Path.GetPathRoot(path))
            {
                list.Add(new ListViewItem("[..]"));
            }

            DirectoryInfo[] directories = di.GetDirectories();

            foreach (DirectoryInfo info in directories)
            {
                ListViewItem el = new ListViewItem("[" + info.Name + "]");
                el.SubItems.Add("");
                el.SubItems.Add("<DIR>");
                el.SubItems.Add(info.CreationTime.ToString());
                list.Add(el);
            }
        }

        // Returns the directory info.
        public static DirectoryInfo GetDirectoryInfo(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di;
        }

        // Creates an unzipped folder near the zip.
        public static void Unzip(string pathToUzip, string whereToUnzip)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(pathToUzip, whereToUnzip);
        }

        // If the directory exists.
        public static bool IfExist(string path)
        {
            return Directory.Exists(path);
        }

        // MD5 hash.
        public static void md5hash(string path)
        {
            FolderMethods p = new FolderMethods(path);
            IVisitor visitor = new MD5hashVisitor();
            p.Accept(visitor);
        }

        // Sypher.
        public static void cypher(string path)
        {
            FolderMethods p = new FolderMethods(path);
            IVisitor visitor = new СypherVisitor();
            p.Accept(visitor);
        }

        // De-Sypher.
        public static void decypher(string path)
        {
            FolderMethods p = new FolderMethods(path);
            IVisitor visitor = new DeСypherVisitor();
            p.Accept(visitor);
        }

        // Visitor accept.
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        // Visitor accept.
        public void wew()
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                // add this map file into the "images" directory in the zip archive
                zip.AddFile("c:\\images\\personal\\7440-N49th.png", "images");
                // add the report into a different directory in the archive
                zip.AddFile("c:\\Reports\\2008-Regional-Sales-Report.pdf", "files");
                zip.AddFile("ReadMe.txt");
                zip.Save("MyZipFile.zip");
            }

        }
    }




    class ZippedFile : Entity
    {

        public string path;
        public void createFolder()
        {
            new ZippedFolder(path).CreateFolder("new folder");
        }
        public void createFile()
        {
            new ZippedFolder(path).CreateFile("newfile.txt");
        }
        public ZippedFile(string path) : base(path)
        {
            this.path = path;
        }
        public Ionic.Zip.ZipFile CreateZip()
        {
            Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(path);
            return zf;
        }
        public void Close()
        {
            Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(path);
            zf.Save();
        }
        public string GetName()
        {
            string name = path.Remove(path.Length - 12, 12);
            return name;
        }
        public bool Existing()
        {
            if (Ionic.Zip.ZipFile.IsZipFile(path) == true)
            { return true; }
            else { return false; }

        }
        public string GetFullName(string name)
        {
            return name;
        }
        public void Delete(string name)
        {
            //int ZipPlace = path.IndexOf(".zip\\");

            //string path1 = path.Substring(0, ZipPlace + 4);
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
            {
                List<string> fullist = zip.EntryFileNames.ToList();
                if (name[name.Length - 1] == '/')
                    name = name.Remove(name.Length - 1, 1) + "\\";
                zip.RemoveEntry(name.Replace('\\', '/'));

                zip.Save();
            }
        }
        public void OpenFile(string name)
        {
            int ZipPlace = path.IndexOf(".zip");
            string path1 = path.Substring(0, ZipPlace + 4);
            string ArchiveWay = path.Substring(ZipPlace + 4);
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path1))
            {
                while (path1[path1.Length - 1] != '\\')
                {
                    path1 = path1.Remove(path1.Length - 1, 1);
                }
                foreach (ZipEntry e in zip)
                {
                    if (e.FileName == ArchiveWay.Replace('\\', '/') + name)
                        e.Extract(path1, ExtractExistingFileAction.DoNotOverwrite);
                }

            }
        }
        public static void OpenZipFile(string DirectoryPath, string FilePath)
        {
            new ZippedFile(DirectoryPath).OpenFile(FilePath);
            int ZipPlace = DirectoryPath.IndexOf(".zip\\");
            string name = FilePath;
            string path1 = DirectoryPath.Substring(0, ZipPlace + 4);
            string ArchiveWay = DirectoryPath.Substring(ZipPlace + 5);
            while (path1[path1.Length - 1] != '\\')
            {
                path1 = path1.Remove(path1.Length - 1, 1);
            }

            while (DirectoryPath[DirectoryPath.Length - 1] != '\\')
            {
                DirectoryPath = DirectoryPath.Remove(DirectoryPath.Length - 1, 1);
            }
            Process.Start(path1 + name);
        }
        public void DeleteZipFile()
        {
            ZippedFile file = new ZippedFile(path);

        }
        public int method()
        {
            int ZipPlace = path.IndexOf(".zip\\");
            return ZipPlace;
        }
        public Ionic.Zip.ZipFile CreateZipFile()
        {
            Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(path + "archive.zip", Encoding.Default);

            return zf;
        }
        public void AddFileZip(string f, Ionic.Zip.ZipFile zf)
        {
            zf.AddFile(f);
        }
        public void Save()
        {
            Ionic.Zip.ZipFile zf = Ionic.Zip.ZipFile.Read(path + "archive.zip");
            zf.Save();
        }
        // Visitor accept.
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }





    }
    class ZippedFolder : Entity
    {
        public string path;
        public void createFolder()
        {
            new ZippedFolder(path).CreateFolder("new folder");
        }
        public void createFile()
        {
            new ZippedFolder(path).CreateFile("newfile.txt");
        }
        public ZippedFolder(string path) : base(path)
        {
            this.path = path;
        }
        MemoryStream data = new MemoryStream();
        public bool Existing()
        {
            if (Ionic.Zip.ZipFile.IsZipFile(path) == true)
            { return true; }
            else { return false; }
        }

        public string GetName()
        {
            string name = path.Remove(path.Length - 12, 12);
            return name;
        }

        public string GetFullName(string name)
        {
            return name;
        }

        public List<string> GetAllFiles()
        {
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
            {
                zip.AlternateEncoding = Encoding.Default;
                List<string> fulList = zip.EntryFileNames.ToList();
                List<string> shortList = new List<string>();
                int i = 0;
                foreach (string elem in fulList)
                {
                    i = elem.IndexOf("/");
                    if (i != -1)
                        shortList.Add(elem.Substring(0, i) + "/");
                    else
                        shortList.Add(elem);
                }
                //List<string> gi = shortList.Distinct().ToList();

                return shortList.Distinct().ToList();
            }
        }

        public void InsertZipToZip(string newWay)
        {
            int ZipPlace = path.IndexOf(".archive.zip\\");
            string ArchiveWay = path.Substring(ZipPlace + 13);
            string path1 = path.Substring(0, ZipPlace + 12);
            try
            {
                FolderMethods.DeleteDirectory("ExtractData");
            }
            catch { }
            FolderMethods.CreateDirectory("ExtractData");
            int newZipPlace = newWay.IndexOf(".archive.zip");
            string newArchiveWay = newWay.Substring(newZipPlace);
            string newpath = newWay.Substring(0, newZipPlace + 12);
            if (ArchiveWay[ArchiveWay.Length - 1] == '/')
            {

                ArchiveWay = ArchiveWay.Remove(ArchiveWay.Length - 1, 1) + "\\";
                if (newArchiveWay != "")
                { newArchiveWay = newArchiveWay.Remove(newArchiveWay.Length - 1, 1) + "\\"; }
                else { newArchiveWay = "\\"; }
                using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path1))
                {
                    foreach (ZipEntry e in zip)
                    {
                        if (e.FileName.Contains(ArchiveWay.Replace('\\', '/')) && e.FileName.IndexOf(ArchiveWay.Replace('\\', '/')) == 0)
                        {
                            e.Extract("ExtractData", ExtractExistingFileAction.DoNotOverwrite);
                        }
                    }
                }

            }
            else
            {
                using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
                {

                    foreach (ZipEntry e in zip)
                    {
                        if (e.FileName == ArchiveWay.Replace('\\', '/'))
                            e.Extract("ExtractData", ExtractExistingFileAction.DoNotOverwrite);
                    }
                }
            }

            while (newArchiveWay[newArchiveWay.Length - 1] != '\\')
            {
                newArchiveWay = newArchiveWay.Remove(newArchiveWay.Length - 1, 1);
            }
            using (Ionic.Zip.ZipFile newzip = Ionic.Zip.ZipFile.Read(newpath))
            {
                newzip.AddItem("ExtractData\\" + ArchiveWay, ArchiveWay.Replace('\\', '/'));
                newzip.Save();
            }
            FolderMethods.DeleteDirectory("ExtractData");
        }

        public void CreateFolder(string directoryname)
        {

            int index = path.LastIndexOf('\\');
            string name = path.Remove(index, path.Length - index);

            var dir = Directory.CreateDirectory(name + '\\' + directoryname);


            //  zp.AddDirectory(dir.FullName);
            using (Ionic.Zip.ZipFile zp = Ionic.Zip.ZipFile.Read(path))
            {
                zp.AddItem(dir.FullName, directoryname);
                zp.Save();
            }
            dir.Delete();
        }

        public void CreateFile(string filename)
        {

            MemoryStream memory = new MemoryStream();
            var bytes = ReadMMFAllBytes(filename);
            memory.Write(bytes, 0, bytes.Length);

            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(path))

            {
                ZipEntry e = zip.AddEntry(filename, memory);
                // zip.AddFile(filename);

                zip.Save();

            }

        }

        public static Byte[] ReadMMFAllBytes(string fileName)
        {
            using (var mmf = MemoryMappedFile.CreateFromFile("C:\\Users\\Илья\\Desktop\\Новая папка\\arch.zip"))
            {
                using (var stream = mmf.CreateViewStream())
                {
                    using (BinaryReader binReader = new BinaryReader(stream))
                    {
                        return binReader.ReadBytes((int)stream.Length);
                    }
                }
            }
        }
        public List<string> OpenFolder(string name)
        {

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
            {
                zip.AlternateEncoding = Encoding.GetEncoding(1251);


                List<string> fulList = zip.EntryFileNames.ToList();
                List<string> shortList = new List<string>();
                int i = 0;

                foreach (string elem in fulList)
                {
                    i = elem.IndexOf("/");
                    if (i != -1)
                        shortList.Add(elem.Substring(0, i) + "/");
                    else
                        shortList.Add(elem);
                }

                return shortList.Distinct().ToList();
            }

            //return (names);
        }
        public static void Delete(string name)
        {

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(name))
            {

                zip.RemoveEntry(name);

                zip.Save();
            }
        }
        public void InsertZipToDir(string newway)
        {
            try
            {
                FolderMethods.DeleteDirectory("ExtractData");
            }
            catch { }
            FolderMethods.CreateDirectory("ExtractData");

            int ZipPlace = path.IndexOf(".archive.zip\\");
            string ArchiveWay = path.Substring(ZipPlace + 13);
            string path1 = path.Substring(0, ZipPlace + 12);
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path1))
            {
                if (path[path.Length - 1] == '/')
                {
                    ArchiveWay = ArchiveWay.Remove(ArchiveWay.Length - 1, 1) + "\\";
                    foreach (ZipEntry e in zip)
                    {
                        if (e.FileName.Contains(ArchiveWay.Replace('\\', '/')) && e.FileName.IndexOf(ArchiveWay.Replace('\\', '/')) == 0)
                        {
                            e.Extract("ExtractData", ExtractExistingFileAction.DoNotOverwrite);
                        }
                    }
                    FolderMethods.CopyDir("ExtractData\\" + ArchiveWay, newway + '\\' + ArchiveWay);

                    FolderMethods.DeleteDirectory("ExtractData");
                }
                else
                {
                    foreach (ZipEntry e in zip)
                    {
                        if (e.FileName == ArchiveWay.Replace('\\', '/'))
                            e.Extract("ExtractData", ExtractExistingFileAction.DoNotOverwrite);
                    }

                    FileMethods.copy("ExtractData\\" + ArchiveWay, newway, false);
                    FolderMethods.DeleteDirectory("ExtractData");
                }
            }
        }
        public void InsertDirToZip(bool isfile, string newway)
        {
            int ZipPlace = newway.IndexOf(".archive.zip\\");
            string ArchiveWay;
            string path1;
            if (ZipPlace <= 0)
            {
                ZipPlace = newway.IndexOf(".archive.zip");
                int index = newway.LastIndexOf("\\");
                ArchiveWay = newway.Substring(index); ;
                path1 = newway.Substring(0, ZipPlace + 12);
            }
            else
            {
                ArchiveWay = newway.Substring(ZipPlace + 13);
                path1 = newway.Substring(0, ZipPlace + 12);
            }


            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path1))
            {
                if (isfile)
                {

                    while (ArchiveWay != "" && ArchiveWay[ArchiveWay.Length - 1] != '\\')
                    {
                        ArchiveWay = ArchiveWay.Remove(ArchiveWay.Length - 1, 1);

                    }
                    zip.AddItem(path, ArchiveWay.Replace('\\', '/'));
                }

                else
                    zip.AddDirectory(path + "\\", ArchiveWay.Replace('\\', '/').Replace(".archive.zip", ""));
                zip.Save();
            }
        }

        public List<string> OpenFolderInZip()
        {

            int ZipPlace = path.IndexOf(".archive.zip\\");
            string path1 = path.Substring(0, ZipPlace + 12);
            string ArchiveWay = path.Substring(ZipPlace);
            string Way = path.Substring(ZipPlace + 13);

            int SleshCount = (ArchiveWay.Length - ArchiveWay.Replace("\\", "").Length) - 1;
            List<string> l = new ZippedFolder(path1).GetLevelFiles(SleshCount, Way.Replace('\\', '/'));

            return l;

        }
        public List<string> GetLevelFiles(int Slesh, string name)
        {
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(path))
            {
                int ZipPlace1 = name.IndexOf("//");
                if (ZipPlace1 > 0)
                { name = name.Remove(ZipPlace1, 1); }
                List<string> fulList = zip.EntryFileNames.ToList();
                string[] shortList = new string[fulList.Count];
                string[] newList = new string[fulList.Count];
                List<string> finalList = new List<string>();


                shortList = fulList.ToArray();
                int k = 0;
                for (int i = 0; i < fulList.Count; i++)
                {
                    if (shortList[i].Contains(name) && shortList[i].IndexOf(name) == 0)
                    {
                        newList[k] = shortList[i];
                        k++;
                    }

                }
                k = 0;
                int count = 0;
                while (k < newList.Length && newList[k] != null)
                {
                    count++;
                    k++;
                }


                string[] s = new string[count];
                k = 0;
                while (k < newList.Length && newList[k] != null)
                {
                    s[k] = newList[k];
                    k++;
                }

                for (int i = 1; i <= Slesh; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        k = s[j].IndexOf("/");
                        s[j] = s[j].Substring(k + 1);
                    }
                }

                foreach (string elem in s)
                {
                    k = elem.IndexOf("/");
                    if (k != -1)
                        if (elem.Substring(0, k) != "")
                            finalList.Add(elem.Substring(0, k) + "/");
                        else { }
                    else
                       if (elem != "")
                        finalList.Add(elem);
                }
                return finalList.Distinct().ToList();
            }
        }
        // Visitor accept.
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}