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
        FileSystemWatcher watcher = null;
        string sourceRootPath = @"c:\SyncFileTest\Dir1";
        string destRootPath = @"c:\SyncFileTest\Dir2";
        uint times = 0;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.StartWatch();

            Console.WriteLine("Start watching...");
            Console.ReadKey();
        }

        void StartWatch()
        {
            watcher = new System.IO.FileSystemWatcher
            {
                //NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Path = sourceRootPath,
                IncludeSubdirectories = true
            };

            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Error += Watcher_Error;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.GetException());
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            times++;
            Console.WriteLine(e.FullPath + " | " + e.ChangeType + " | " + times);

            //string filename = Path.GetFileName(e.FullPath);
            //string destPath = destRootPath + "\\" + filename;
            //FileOpHelper.DeleteDirOrFile(destPath);

            string destPath = destRootPath + @"\" + e.Name;
            while (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            //times++;
            Console.WriteLine(e.FullPath + " | " + e.ChangeType);
            string destFilePath;
            foreach (string fls in Directory.GetFiles(sourceRootPath))
            {
                if (!FileStatusHelper.IsFileOccupied(fls))
                {
                    //FileInfo flinfo = new FileInfo(fls);
                    destFilePath = destRootPath + "\\" + Path.GetFileName(fls);
                    //flinfo.CopyTo(destFilePath, true);  // 测试同步创建完，原目录文件无法删除
                    //flinfo = null;

                    FileStream fs = File.Create(destFilePath);
                    fs.Dispose();
                    fs.Close();
                }
            }
            GC.Collect();
            System.Threading.Thread.Sleep(2000);
        }
    }
}
