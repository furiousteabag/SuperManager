using System;
using System.Threading.Tasks;

namespace SPBU12._1MANAGER
{
    public abstract class Archive
    {
        //Архивация(архивирует папку с файлами с асинхронной архивацией)
        protected void DoPack(string path)
        {
            //If we got a folder
            if (IsFolder(path))
            {
                path = path.Replace("[", "").Replace("]", "");

                FolderMethods.CreateDirectory(path + "_archived");

                Parallel.ForEach(FileMethods.GetFileInfos(path), (currentFile) =>
                {
                    string pathFile = currentFile.FullName;
                    string compressfile = path + "_archived" + "\\" + currentFile.ToString() + ".zip";
                    CompressFile(pathFile, compressfile);
                });
            }
            //If we got a file
            else
            {              
                CompressFile(path + ".txt", FolderMethods.GetName(path) + "\\" + FileMethods.GetName(path) + ".txt" + ".zip");
            }
        }

        //Compress one file
        protected abstract void CompressFile(string pathfile, string compressfile);

        //Check if we got a folder
        private bool IsFolder(string path)
        {
            return path.Contains("[");
        }

    }

    // First realization
    class ArchiveFile : Archive
    {

        public void Pack(string path)
        {
            DoPack(path);
        }

        protected override void CompressFile(string pathfile, string compressfile)
        {
            // поток для чтения исходного файла
            using (var sourceStream = FileMethods.GetFileStream(pathfile))
            {
                // поток для записи сжатого файла
                using (var targetStream = FileMethods.GetTargetStream(compressfile))
                {
                    // поток архивации
                    using (var compressionStream = Entity.GetGZipStream(targetStream))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            pathfile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }
    }

    // Second realization
    class ArchiveFileSecond : Archive
    {

        public void Pack(string path)
        {
            DoPack(path);
        }

        protected override void CompressFile(string pathfile, string compressfile)
        {
            FolderMethods.CreateDirectory(pathfile + "_ZIP");
            FileMethods.CopyToZippedFolder(pathfile);
            FolderMethods.CreateZipFrom(pathfile + "_ZIP", compressfile);
            FolderMethods.DeleteDirectory(pathfile + "_ZIP");
        }
    }
}
