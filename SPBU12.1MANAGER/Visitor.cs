using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{

    // Visitor interface. 
    internal interface IVisitor
    {
        void Visit(FileMethods p);
        void Visit(FolderMethods p);
    }

    // MD5 visitor.
    internal class MD5hashVisitor : IVisitor
    {
        // Visiting file.
        public void Visit(FileMethods p)
        {
            string filepath = p.path;

            // Showing the message with taken hash.
            MessageBox.Show(
                        "MD5 hash of a file: " + MD5Hash.GetMd5Hash(filepath),
                        "MD-5 hash",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }

        // Visiting folder.
        public void Visit(FolderMethods p)
        {
            string folderpath = p.path;
            string allFiles = "";

            // Getting the list of all files.
            DirectoryInfo di = new DirectoryInfo(folderpath);
            FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);

            // Making big string of filenames to take hash from it.
            foreach (var info in files)
            {
                allFiles += info.FullName;
            }

            // Showing the message with taken hash.
            MessageBox.Show(
                        "MD5 hash of a folder: " + MD5Hash.GetMd5Hash(allFiles),
                        "MD-5 hash",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }
    }

    // Cypher visitor.
    internal class СypherVisitor : IVisitor
    {
        // Visiting file.
        public void Visit(FileMethods p)
        {
            string key = "";
            string filepath = p.path;

            // Getting the cypher key.
            using (CypherKeyBox cypherKeyBox = new CypherKeyBox())
            {
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;
                }
            }

            // Encrypting file.
            CypherFile.EncryptFile(filepath, filepath + "_cyphered", key);

            // Delete the original file.
            File.Delete(filepath);

            // We're done.
            MessageBox.Show(
                        "Сyphering file: done",
                        "Сyphering",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }

        // Visiting folder.
        public void Visit(FolderMethods p)
        {
            string folderpath = p.path;            
            string key = "";
            List<string> allFiles = new List<string>();

            // Getting the cypher key.
            using (CypherKeyBox cypherKeyBox = new CypherKeyBox())
            {
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;
                }
            }

            // Getting the list of files.
            DirectoryInfo di = new DirectoryInfo(folderpath);
            FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);

            // Encrypting each file and deleting the original one.
            foreach (var info in files)
            {
                CypherFile.EncryptFile(info.FullName, info.FullName + "_cyphered", key);
                File.Delete(info.FullName);
            }

            // We're done.
            MessageBox.Show(
                        "Сyphering folder: done",
                        "Сyphering",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }
    }

    // De-Cypher visitor.
    internal class DeСypherVisitor : IVisitor
    {
        // Visiting file.
        public void Visit(FileMethods p)
        {
            string key = "";
            string filepath = p.path;

            // Getting the cypher key.
            using (CypherKeyBox cypherKeyBox = new CypherKeyBox())
            {
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;
                }
            }

            // Decrypt file.
            CypherFile.DecryptFile(filepath, filepath.Substring(0, filepath.Length - 9), key);

            // Delete encrypted file.
            File.Delete(filepath);

            // We're done.
            MessageBox.Show(
                        "De-Сyphering file: done",
                        "De-Сyphering",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }

        // Visiting folder.
        public void Visit(FolderMethods p)
        {

            string folderpath = p.path;            
            string key = "";
            List<string> allFiles = new List<string>();

            // Getting the cypher key.
            using (CypherKeyBox cypherKeyBox = new CypherKeyBox())
            {
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;
                }
            }

            // Getting the list of files in directory.
            DirectoryInfo di = new DirectoryInfo(folderpath);
            FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);

            // Decrypting each file and deleting the encrypted ones.
            foreach (var info in files)
            {
                CypherFile.DecryptFile(info.FullName, info.FullName.Substring(0, info.FullName.Length - 9), key);
                File.Delete(info.FullName);
            }

            // We're done.
            MessageBox.Show(
                        "De-Сyphering folder: done",
                        "De-Сyphering",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                                   );
        }
    }
}
