using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{
    public class ListElements
    {
        public List<ListViewItem> list;
        public string path;

        public ListElements()
        {
            list = new List<ListViewItem>();
        }


        public void Update(string path)
        {
            try
            {
                this.path = path;
                var di = FolderMethods.GetDirectoryInfo(path);

                list.Clear();

                FolderMethods.UpdateDirectories(di, list, path);
                FileMethods.UpdateFiles(di, list);
            }
            catch
            {
                path = FolderMethods.GetName(path);
                //throw new Exception();
            }
        }

        private bool IsDir(string name)
        {
            return (name.Remove(1) == "[");
        }

        private bool IsBack(string name)
        {
            return (name == "[..]");
        }

        public string DoubleClick(ListView lw, string path)
        {
            string name = lw.SelectedItems[0].Text;

            // If folder.
            if (IsDir(name))
            {
                if (IsBack(name))
                    path = FolderMethods.GetName(path);
                else
                {
                    if (path[path.Length - 1] != Entity.GetDirectorySeparatorChar())
                        path += Entity.GetDirectorySeparatorChar();
                    path += name.Remove(0, 1).Remove(name.Length - 2);
                }
            }
            // If something from folder.
            else
            {
                // File path (if exists).
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] smFiles = di.GetFiles(name + "*");
                string pathfile = "";
                if (smFiles.Count() > 0)
                {
                    path = smFiles[0].FullName;
                }

                
            }

            return path;
        }
    }
}
