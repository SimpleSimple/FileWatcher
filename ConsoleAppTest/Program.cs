using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTest
{
    class Program
    {
        string sourceRootPath = @"c:\SyncFileTest\Dir1";
        string destRootPath = @"c:\SyncFileTest\Dir2";

        static void Main(string[] args)
        {
            Program p = new Program();
            p.StartWatch();

            Console.WriteLine("Start watching...");
            Console.ReadKey();
        }

        void StartWatch()
        {
            var watcher = new System.IO.FileSystemWatcher
            {
                //NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Path = sourceRootPath,
                IncludeSubdirectories = true
            };

            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath + " | " + e.ChangeType);

            string filename = Path.GetFileName(e.FullPath);
            string destPath = destRootPath + "\\" + filename;
            FileOpHelper.DeleteDirOrFile(destPath);
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath + " | " + e.ChangeType);
            string destFilePath;
            foreach (string fls in Directory.GetFiles(sourceRootPath))
            {
                if (!FileStatusHelper.IsFileOccupied(fls))
                {
                    //FileInfo flinfo = new FileInfo(fls);
                    destFilePath = destRootPath + "\\" + Path.GetFileName(fls);
                    //flinfo.CopyTo(destFilePath, true);
                    FileStream fs = File.Create(destFilePath);
                    fs.Dispose();
                    fs.Close();
                }
            }
        }
    }
}
