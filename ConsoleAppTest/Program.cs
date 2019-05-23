using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTest
{
    class Program
    {
        FileSystemWatcher watcher = null;
        string sourceRootPath = @"c:\SyncFileTest\Dir1";
        string destRootPath = @"c:\SyncFileTest\Dir2";
        //static ConcurrentQueue<string> fileQueue = new ConcurrentQueue<string>();
        static ConcurrentQueue<string> fileQueue = new ConcurrentQueue<string>();
        static ConcurrentQueue<string> delFileQueue = new ConcurrentQueue<string>();
        uint times = 0;
        uint i = 0, j = 0;

        static void Main(string[] args)
        {
            uint h = 0, k = 0;
            Program p = new Program();
            p.StartWatch();

            Thread t = new Thread(() =>
            {
                while (1 == 1)
                {
                    if (fileQueue.Count > 0)
                    {
                        aa:
                        h++;
                        Console.WriteLine("process files：" + fileQueue.Count);
                        string destFilePath = null;
                        fileQueue.TryDequeue(out destFilePath);
                        FileStream fs = null;
                        try
                        {
                            // 真实创建文件方法
                            fs = File.Create(destFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            goto aa;
                        }
                        finally
                        {
                            fs.Dispose();
                            fs.Close();
                            if (h >= 3000)
                            {
                                h = 0;
                                Thread.Sleep(3000);
                            }
                        }
                    }
                    else
                        Thread.Sleep(1000);
                }
            });
            t.IsBackground = true;
            t.Start();

            Thread t2 = new Thread(() =>
            {
                while (1 == 1)
                {
                    if (delFileQueue.Count > 0)
                    {
                        bb:
                        k++;
                        Console.WriteLine("process del files：" + delFileQueue.Count);
                        string destFilePath = null;
                        delFileQueue.TryDequeue(out destFilePath);
                        try
                        {
                            // 真正删除文件方法
                            if (File.Exists(destFilePath))
                            {
                                File.Delete(destFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            goto bb;
                        }
                        finally
                        {
                            if (k >= 3000)
                            {
                                k = 0;
                                Thread.Sleep(3000);
                            }
                        }
                    }
                    else
                        Thread.Sleep(1000);
                }
            });
            t2.IsBackground = true;
            t2.Start();

            Console.WriteLine("Start watching...");
            Console.ReadKey();
        }

        void StartWatch()
        {

            //using (watcher = new System.IO.FileSystemWatcher())
            //{
            watcher = new System.IO.FileSystemWatcher();
            watcher.Path = sourceRootPath;
            //watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.InternalBufferSize = 65536;
            watcher.IncludeSubdirectories = false;

            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Error += Watcher_Error;

            watcher.EnableRaisingEvents = true;
            //}
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.GetException());
            while (e != null && e.GetException() != null)
            {
                e = null;
                StartWatch();
            }
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            times++;
            Console.WriteLine(e.FullPath + " | " + e.ChangeType + " | " + times);

            //string filename = Path.GetFileName(e.FullPath);
            //string destPath = destRootPath + "\\" + filename;
            //FileOpHelper.DeleteDirOrFile(destPath);

            string destPath = destRootPath + @"\" + e.Name;
            delFileQueue.Enqueue(destPath);
            //while (File.Exists(destPath))
            //{
            //File.Delete(destPath);
            //}
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath + " | " + e.ChangeType);
            string fls = e.FullPath;
            string destFilePath;
            //foreach (string fls in Directory.GetFiles(sourceRootPath))
            //{
            //if (!FileStatusHelper.IsFileOccupied(fls))
            //{
            //FileInfo flinfo = new FileInfo(fls);
            destFilePath = destRootPath + "\\" + Path.GetFileName(fls);
            //flinfo.CopyTo(destFilePath, true);  // 测试同步创建完，原目录文件无法删除
            //flinfo = null;

            //FileStream fs = File.Create(destFilePath);
            //fs.Dispose();
            //fs.Close();


            // 创建事件太慢，把要创建的文件放队列，在线程去执行
            fileQueue.Enqueue(destFilePath);
            //if (fileQueue.Count > 2000)
            //    Thread.Sleep(2000);
            //}
            //}

        }

    }
}