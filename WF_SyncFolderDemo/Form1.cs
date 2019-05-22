using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WF_SyncFolderDemo
{
    public partial class Form1 : Form
    {
        string myDocsPath;
        string sourceRootPath;
        string destRootPath;
        string tempDir;
        string trashDir;
        DateTime lastRead = DateTime.MinValue;
        private System.Timers.Timer processTimer;
        System.Collections.Concurrent.ConcurrentQueue<string> fileQueue;


        public Form1()
        {
            InitializeComponent();

            fileQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sourceRootPath = this.textBox1.Text.Trim();
            destRootPath = this.textBox2.Text.Trim();
            myDocsPath = sourceRootPath.Substring(0, sourceRootPath.LastIndexOf(@"\") + 1);
            tempDir = Path.Combine(myDocsPath, "Cache");
            trashDir = Path.Combine(myDocsPath, "Trash");

            PrepareDir(sourceRootPath);
            PrepareDir(destRootPath);
            PrepareDir(tempDir);
            PrepareDir(trashDir);
            this.richTextBox1.Text = "The prepared folder or file has been created" + Environment.NewLine;

            getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
            getSpecifiedPathDirsList(destRootPath, this.treeView2);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox1.Text = path.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox2.Text = path.SelectedPath;
        }

        private void ProcessRefresh(object sender, ElapsedEventArgs args)
        {
            try
            {
                //Console.WriteLine("Processing queue, " + fileQueue.Count + " files created:");
                //rwlock.EnterReadLock();
                //foreach (string fls in filePaths)
                //{
                //    Console.WriteLine(fls);
                //    FileInfo flinfo = new FileInfo(fls);
                //    flinfo.CopyTo(destRootPath + "\\" + flinfo.Name, false);
                //}
                //filePaths.Clear();
                //string fls;
                //while (fileQueue.TryDequeue(out fls))
                //{
                //    Console.WriteLine(fls);
                //    FileOpHelper.DeleteDirOrFile(fls);
                //}

                ThreadInteropUtils.OpeMainFormControl(() =>
                {
                    getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
                    getSpecifiedPathDirsList(destRootPath, this.treeView2);
                }, this);
            }
            finally
            {
                if (processTimer != null)
                {
                    processTimer.Stop();
                    processTimer.Dispose();
                    processTimer = null;
                }
                //rwlock.ExitReadLock();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var watcher = new System.IO.FileSystemWatcher
            {
                //NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Path = sourceRootPath + @"\",
                IncludeSubdirectories = true
            };
            this.button3.Enabled = false;

            try
            {
                // 监控创建目录或文件
                watcher.Created += (S, E) =>
                {
                    Console.WriteLine(E.FullPath + " | " + E.ChangeType);
                    //ThreadInteropUtils.OpeMainFormControl(() =>
                    //{
                    //    this.richTextBox1.Text += "Create folder or file was happened..." + Environment.NewLine;
                    //}, this);

                    //SyncProcess();

                    // v2.0
                    //Thread.Sleep(5000);
                    //FileOpHelper.CopyDirectory(sourceRootPath, destRootPath, true);

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

                    //ThreadInteropUtils.OpeMainFormControl(() =>
                    //{
                    //    this.richTextBox1.Text += "Synchronizing file completed..." + Environment.NewLine + Environment.NewLine;
                    //}, this);

                    //if (processTimer == null)
                    //{
                    //    processTimer = new System.Timers.Timer(2000);
                    //    processTimer.Elapsed += ProcessRefresh;
                    //    processTimer.Start();
                    //}
                    //Thread t = new Thread(() =>
                    //{
                    //    ThreadInteropUtils.OpeMainFormControl(() =>
                    //    {
                    //        getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
                    //        getSpecifiedPathDirsList(destRootPath, this.treeView2);
                    //    }, this);
                    //});
                    //t.Start();

                };

                // 监控删除目录或文件
                watcher.Deleted += (S, E) =>
                {
                    Console.WriteLine(E.FullPath + " | " + E.ChangeType);
                    //ThreadInteropUtils.OpeMainFormControl(() =>
                    //{
                    //    this.richTextBox1.Text += "Delete folder or file was happened..." + Environment.NewLine;
                    //}, this);

                    //SyncProcess();

                    string filename = Path.GetFileName(E.FullPath);
                    string destPath = destRootPath + "\\" + filename;
                    FileOpHelper.DeleteDirOrFile(destPath);

                    //fileQueue.Enqueue(destPath);

                    //ThreadInteropUtils.OpeMainFormControl(() =>
                    //{
                    //    this.richTextBox1.Text += "Synchronizing file completed..." + Environment.NewLine + Environment.NewLine;
                    //}, this);

                    //ThreadInteropUtils.OpeMainFormControl(() =>
                    //{
                    //    getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
                    //    getSpecifiedPathDirsList(destRootPath, this.treeView2);
                    //}, this);

                    //if (processTimer == null)
                    //{
                    //    processTimer = new System.Timers.Timer(2000);
                    //    processTimer.Elapsed += ProcessRefresh;
                    //    processTimer.Start();
                    //}
                };

                // 监控变更文件
                watcher.Changed += (S, E) =>
                {
                    Console.WriteLine(E.FullPath + " | " + E.ChangeType);
                    ThreadInteropUtils.OpeMainFormControl(() =>
                    {
                        this.richTextBox1.Text += "Change folder or file was happened..." + Environment.NewLine;
                    }, this);

                    //if (!Path.GetExtension(E.FullPath).Equals(".metadata") && FileOpHelper.isFile(E.FullPath))
                    //{
                    //    DateTime lastWriteTime = File.GetLastWriteTime(E.FullPath);
                    //    if (lastWriteTime.Ticks - lastRead.Ticks > 100000)  // 要修改为300000？
                    //    {
                    //        ThreadInteropUtils.OpeMainFormControl(() =>
                    //        {
                    //            this.richTextBox1.Text += "Change folder or file was happened..." + Environment.NewLine;
                    //        }, this);
                    //        SyncProcess();
                    //        lastRead = lastWriteTime;

                    //        ThreadInteropUtils.OpeMainFormControl(() =>
                    //        {
                    //            getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
                    //            getSpecifiedPathDirsList(destRootPath, this.treeView2);
                    //        }, this);
                    //    }
                    //}
                    FileOpHelper.CopyDirectory(sourceRootPath, destRootPath);

                    ThreadInteropUtils.OpeMainFormControl(() =>
                    {
                        this.richTextBox1.Text += "Synchronizing file completed..." + Environment.NewLine + Environment.NewLine;
                    }, this);

                };

                // 监控修改目录或文件
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception exc)
            {
                this.richTextBox1.Text = "Error：" + exc.Message;
            }

        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            ThreadInteropUtils.OpeMainFormControl(() =>
            {
                this.richTextBox1.Text += "Rename folder or file was happened..." + Environment.NewLine;
            }, this);

            //SyncProcess();            

            FileOpHelper.CopyDirectory(sourceRootPath, destRootPath);

            string destPath = destRootPath + "\\" + e.Name;
            if (!Directory.Exists(destPath) && FileOpHelper.isFile(destPath) == false)
                Directory.CreateDirectory(destPath);

            if (!File.Exists(destPath) && FileOpHelper.isFile(destPath))
            {
                FileInfo flinfo = new FileInfo(e.FullPath);
                flinfo.CopyTo(destPath, true);
            }

            string oldname = destRootPath + "\\" + e.OldName;
            FileOpHelper.DeleteDirOrFile(oldname);

            ThreadInteropUtils.OpeMainFormControl(() =>
            {
                this.richTextBox1.Text += "Synchronizing file completed..." + Environment.NewLine + Environment.NewLine;
            }, this);

            ThreadInteropUtils.OpeMainFormControl(() =>
            {
                getSpecifiedPathDirsList(sourceRootPath, this.treeView1);
                getSpecifiedPathDirsList(destRootPath, this.treeView2);
            }, this);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }


        #region 私有方法


        private void PrepareDir(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("源文件夹和目标文件夹目录路径不能为空");
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void getSpecifiedPathDirsList(string path, TreeView treeView, ListControl control = null)
        {
            if (treeView.Nodes.Count > 0)
            {
                treeView.Nodes.Clear();
            }
            TreeNode node = new TreeNode("文件");
            treeView.Nodes.Add(node);
            DirectoryInfo dir = new DirectoryInfo(path);
            Traverse(node, dir);

            treeView.Nodes[0].Expand();
        }


        private void SyncProcess()
        {
            try
            {
                // 同步文件
                SyncFileOperate(sourceRootPath, destRootPath, tempDir, trashDir);

                ThreadInteropUtils.OpeMainFormControl(() =>
                {
                    this.richTextBox1.Text += "Synchronizing file completed..." + Environment.NewLine + Environment.NewLine;
                }, this);
            }
            catch (Exception ex)
            {
                ThreadInteropUtils.OpeMainFormControl(() =>
                {
                    this.richTextBox1.Text += "Synchronizing file error..." + " Error Detail：" + ex.Message + Environment.NewLine + Environment.NewLine;
                }, this);
            }
        }

        private void SyncFileOperate(string sourceRootPath, string destRootPath, string tempDir, string trashDir)
        {
            FileSyncScopeFilter filter = new FileSyncScopeFilter();
            filter.FileNameExcludes.Add("*.metadata");
            FileSyncOptions options = FileSyncOptions.None;

            //DetectChanges
            DetectChangesOnFileSystemReplica(sourceRootPath, filter, options, sourceRootPath, "filesync.metadata", tempDir, trashDir);
            DetectChangesOnFileSystemReplica(destRootPath, filter, options, destRootPath, "filesync.metadata", tempDir, trashDir);

            try
            {
                ThreadInteropUtils.OpeMainFormControl(() =>
                {
                    this.richTextBox1.Text += "Start synchronizing..." + Environment.NewLine;
                }, this);

                //SyncChanges Both Ways
                SyncOperationStatistics syncOperationStatistics = null;
                SyncFileUtils.SyncFileSystemReplicasOneWay(sourceRootPath, destRootPath, filter, options, sourceRootPath, "filesync.metadata", destRootPath, "filesync.metadata", tempDir, trashDir, ref syncOperationStatistics);

                //ThreadInteropUtils.OpeMainFormControl(() =>
                //{
                //    this.richTextBox1.Text += "Synchronizing file upload changes... " + syncOperationStatistics.UploadChangesApplied.ToString() + Environment.NewLine;
                //}, this);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void DetectChangesOnFileSystemReplica(string replicaRootPath, FileSyncScopeFilter filter, FileSyncOptions options, string metadataPath, string metadataFile, string tempDir, string trashDir)
        {
            FileSyncProvider provider = null;
            try
            {
                provider = new FileSyncProvider(replicaRootPath, filter, options, metadataPath, metadataFile, tempDir, trashDir);
                provider.DetectChanges();
            }
            finally
            {
                // Release resources
                if (provider != null)
                    provider.Dispose();
            }
        }

        /// <summary>
        /// 非递归的遍历所有的子目录与文件
        /// </summary>
        private void Traverse(TreeNode node, DirectoryInfo dir)
        {
            Stack<DirectoryInfo> stack_dir = new Stack<DirectoryInfo>(); // 用栈来保存没有遍历的子目录
            Stack<TreeNode> stack_node = new Stack<TreeNode>();
            DirectoryInfo currentDir = dir;
            TreeNode currentNode = node;
            stack_dir.Push(dir);
            stack_node.Push(node);

            while (stack_dir.Count != 0) // 栈不为空，说明还有子节点没有访问到
            {
                currentDir = stack_dir.Pop(); // 出栈，获取上一个结点
                currentNode = stack_node.Pop(); // 出栈，获取上一个TreeNode

                // 访问当前目录所有子目录
                DirectoryInfo[] subDirs = currentDir.GetDirectories();
                foreach (DirectoryInfo di in subDirs)
                {
                    TreeNode d = new TreeNode(di.Name);
                    currentNode.Nodes.Add(d);
                    stack_node.Push(d);  // 当前TreeNode结点入栈
                    stack_dir.Push(di);  // 将子节点入栈
                }

                // 访问当前目录所有子文件
                //FileInfo[] files = currentDir.GetFiles();                
                //foreach (var f in files)
                //{
                //    if (f.Extension.Contains(".metadata"))
                //        continue;
                //    // 将文件添加到结点中
                //    TreeNode file = new TreeNode(f.Name);
                //    currentNode.Nodes.Add(file);
                //}

                var sorted = currentDir.GetFiles().OrderBy(f => f.Name);
                foreach (var f in sorted)
                {
                    TreeNode file = new TreeNode(f.Name);
                    currentNode.Nodes.Add(file);
                }
            }
        }

        #endregion

        private void button1_Click_1(object sender, EventArgs e)
        {
            string[] dirs = Directory.GetDirectories(sourceRootPath);
            string destDirPath = "";
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            //foreach (string dir in dirs)
            //{
            //    destDirPath = destRootPath + "\\" + Path.GetFileName(dir);
            //    if (!Directory.Exists(destDirPath))
            //    {
            //        Directory.CreateDirectory(destDirPath);

            //    }
            //}
            FileOpHelper.CopyDirectory(sourceRootPath, destRootPath);
            stopwatch.Stop();
            this.richTextBox1.Text = "Total time：" + stopwatch.ElapsedMilliseconds + "ms";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            FileSyncProvider sourceProvider = null;
            FileSyncProvider destinationProvider = null;
            try
            {
                FileSyncScopeFilter filter = new FileSyncScopeFilter();
                filter.FileNameExcludes.Add("*.metadata");
                FileSyncOptions options = FileSyncOptions.None;

                sourceProvider = new FileSyncProvider(sourceRootPath, filter, options);
                destinationProvider = new FileSyncProvider(destRootPath, filter, options);

                //destinationProvider.AppliedChange += new EventHandler<AppliedChangeEventArgs>(OnAppliedChange);
                //destinationProvider.SkippedChange += new EventHandler<SkippedChangeEventArgs>(OnSkippedChange);

                SyncOrchestrator agent = new SyncOrchestrator();
                agent.LocalProvider = sourceProvider;
                agent.RemoteProvider = destinationProvider;
                //agent.Direction = SyncDirectionOrder.Upload; // Sync source to destination
                agent.Direction = SyncDirectionOrder.DownloadAndUpload; // Sync source to destination
                Console.WriteLine("Synchronizing changes to replica: " + destinationProvider.RootDirectoryPath);
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                agent.Synchronize();

                stopwatch.Stop();
                this.richTextBox1.Text = "Sync framework test total time：" + stopwatch.ElapsedMilliseconds + "ms";

            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                // Release resources
                if (sourceProvider != null) sourceProvider.Dispose();
                if (destinationProvider != null) destinationProvider.Dispose();
            }
        }
    }
}
