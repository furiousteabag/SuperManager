using System.Collections.Generic;
using System.Windows.Forms;

namespace SPBU12._1MANAGER
{

    // Visitor interface. 
    internal interface IVisitor
    {
        void Visit(FileMethods p);
        void Visit(FolderMethods p);
        void Visit(ZippedFile zippedFile);
        void Visit(ZippedFolder zippedFolder);
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
            var files = FileMethods.GetFileInfosForSearch(folderpath);

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
        // Visiting file.
        public void Visit(ZippedFile p)
        {
        }
        // Visiting file.
        public void Visit(ZippedFolder p)
        {
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

                // If clicked OK button.
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;

                    // Encrypting file.
                    CypherFile.EncryptFile(filepath, filepath + "_cyphered", key);

                    // Delete the original file.
                    FileMethods.DeleteFile(filepath);

                    // We're done.
                    MessageBox.Show(
                                "Сyphering file: done",
                                "Сyphering",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                                           );
                }
            }

           
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
                // If clicked OK button.
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;

                    // Getting the list of files.
                    var files = FileMethods.GetFileInfosForSearch(folderpath);

                    // Encrypting each file and deleting the original one.
                    foreach (var info in files)
                    {
                        CypherFile.EncryptFile(info.FullName, info.FullName + "_cyphered", key);
                        FileMethods.DeleteFile(info.FullName);
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

            
        }
        // Visiting file.
        public void Visit(ZippedFile p)
        {
        }
        // Visiting file.
        public void Visit(ZippedFolder p)
        {
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
                // If clicked OK button.
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;

                    // Decrypt file.
                    CypherFile.DecryptFile(filepath, filepath.Substring(0, filepath.Length - 9), key);

                    // Delete encrypted file.
                    FileMethods.DeleteFile(filepath);

                    // We're done.
                    MessageBox.Show(
                                "De-Сyphering file: done",
                                "De-Сyphering",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                                           );
                }
            }

            
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
                // If clicked OK button.
                if (cypherKeyBox.ShowDialog() == DialogResult.OK)
                {
                    key = cypherKeyBox.GetKeyString;

                    // Getting the list of files in directory.
                    var files = FileMethods.GetFileInfosForSearch(folderpath);

                    // Decrypting each file and deleting the encrypted ones.
                    foreach (var info in files)
                    {
                        CypherFile.DecryptFile(info.FullName, info.FullName.Substring(0, info.FullName.Length - 9), key);
                        FileMethods.DeleteFile(info.FullName);
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
        // Visiting file.
        public void Visit(ZippedFile p)
        {
        }
        // Visiting file.
        public void Visit(ZippedFolder p)
        {
        }
    }
}
