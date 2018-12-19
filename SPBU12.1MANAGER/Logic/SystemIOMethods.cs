using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace SPBU12._1MANAGER {

    // Methods which are common to both file and folder.
    abstract class Entity {

        protected string path;

        public Entity(string path)
        {
            this.path = path;
        }

        // Returns GzipStream.
        public static GZipStream GetGZipStream(FileStream targetStream) {
            GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            return compressionStream;
        }

        // Returns directory separator char.
        public static char GetDirectorySeparatorChar()
        {            
            return Path.DirectorySeparatorChar;
        }

        // Returns web responce stream.
        public static Stream GetWebStream(WebResponse response) {
            return response.GetResponseStream();
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
            if (!FileMethods.IfExist(root + Path.DirectorySeparatorChar + name))
                File.Delete(root + Path.DirectorySeparatorChar + name);
            else
            {
                string pathDir = root + Path.DirectorySeparatorChar + name.Substring(1, name.Length - 2);
                Directory.Delete(pathDir, true);
            }
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
        public static void CopyToZippedFolder(string pathfile) {
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


        // Returns memorystream.
        public static MemoryStream Memory()
        {
            MemoryStream memoryStream = new MemoryStream();
            return memoryStream;
        }

        // MD5 hash.
        public static void md5hash(string path) {
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
    class FolderMethods : Entity {

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
        public static void Unzip(string pathToUzip, string whereToUnzip) {
            System.IO.Compression.ZipFile.ExtractToDirectory(pathToUzip, whereToUnzip);
        }

        // If the directory exists.
        public static bool IfExist(string path) {
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
        public  void wew()
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
    
}
