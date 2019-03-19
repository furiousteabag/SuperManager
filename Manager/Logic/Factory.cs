using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPBU12._1MANAGER.Logic
{
    class Factory
    {
        public static Entity Get(string path)
        {


            if (new FileMethods(path.Replace(".archive.zip", "")).GetExtension() != "" && path.EndsWith(".archive.zip"))
            {
                return new ZippedFile(path);
            }
            if (new FileMethods(path.Replace(".archive.zip", "")).GetExtension() != "" && path.EndsWith(".archive.zip\\"))
            {
                return new ZippedFile(path);
            }
            if (new FileMethods(path).GetExtension() != "" && path.Contains(".archive.zip\\") && !path.EndsWith(".archive.zip\\"))
            {
                return new ZippedFile(path);
            }
            if (new FileMethods(path).GetExtension() == "" && path.Contains(".archive.zip\\") && !path.EndsWith(".archive.zip\\"))
            {
                return new ZippedFolder(path);
            }
            if (new FolderMethods(path.Replace(".archive.zip", "")).GetExtension() == "" && (path.EndsWith(".archive.zip")))
            {
                return new ZippedFolder(path);
            }
            if ((new FileMethods(path).GetExtension() != ""))
            {
                return new FileMethods(path);
            }

            return new FolderMethods(path);
        }
    }
}
