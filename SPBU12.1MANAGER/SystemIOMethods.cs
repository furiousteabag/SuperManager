using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
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

        // Returns the list of files from path.
        public static FileInfo[] GetFileInfos(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();
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

        // Creates a zip archive of a folder.
        public static void CreateZipFrom(string pathfile, string compressfile)
        {
            ZipFile.CreateFromDirectory(pathfile, compressfile);
        }

        // Returns the list of directories from path.
        public static DirectoryInfo[] GetDirectoryInfos(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] directories = di.GetDirectories();
            return directories;
        }

        // Returns the directory info.
        public static DirectoryInfo GetDirectoryInfo(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di;
        }

        // Creates an unzipped folder near the zip.
        public static void Unzip(string pathToUzip, string whereToUnzip) {
            ZipFile.ExtractToDirectory(pathToUzip, whereToUnzip);
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
    }
}
