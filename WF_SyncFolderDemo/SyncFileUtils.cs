using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_SyncFolderDemo
{
    public class SyncFileUtils
    {
        public static SyncId getSyncID(string SyncFilePath)
        {
            SyncId replicaID = null;
            Guid guid__1;
            if (!File.Exists(SyncFilePath))
            {
                guid__1 = Guid.NewGuid();
                replicaID = new SyncId(guid__1);
                FileStream fs = File.Open(SyncFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(guid__1.ToString());
                sw.Close();
                fs.Close();
            }
            else
            {
                String guidString;
                FileStream fs = new FileStream(SyncFilePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                guidString = sr.ReadLine();
                guid__1 = new Guid(guidString);
                replicaID = new SyncId(guid__1);
                sr.Close();
                fs.Close();
            }
            return replicaID;

        }

        //public static void SyncFileSystemReplicasOneWay(string sourceReplicaRootPath, string destinationReplicaRootPath, FileSyncScopeFilter filter, FileSyncOptions options, string metadataPath1, string metadataFile1, string metadataPath2, string metadataFile2, string tempDir = null, string trashDir = null)
        //{
        //    FileSyncProvider sourceProvider = null;
        //    FileSyncProvider destProvider = null;
        //    try
        //    {
        //        sourceProvider = new FileSyncProvider(sourceReplicaRootPath, filter, options);
        //        destProvider = new FileSyncProvider(destinationReplicaRootPath, filter, options);

        //        sourceProvider.DetectChanges();
        //        destProvider.DetectChanges();

        //        //启动和控制同步会话。
        //        SyncOrchestrator agent = new SyncOrchestrator();
        //        agent.LocalProvider = sourceProvider;
        //        agent.RemoteProvider = destProvider;
        //        agent.Direction = SyncDirectionOrder.Upload;

        //        agent.Synchronize();    //此处开始执行文件（夹）同步。
        //    }
        //    finally
        //    {
        //        if (sourceProvider != null) sourceProvider.Dispose();
        //        if (destProvider != null) destProvider.Dispose();
        //    }
        //}

        public static void SyncFileSystemReplicasOneWay(string sourceReplicaRootPath, string destinationReplicaRootPath, FileSyncScopeFilter filter, FileSyncOptions options, string metadataPath1, string metadataFile1, string metadataPath2, string metadataFile2, string tempDir, string trashDir,
            ref SyncOperationStatistics syncOperationStatistics)
        {
            FileSyncProvider sourceProvider = null;
            FileSyncProvider destinationProvider = null;
            try
            {
                sourceProvider = new FileSyncProvider(sourceReplicaRootPath, filter, options, metadataPath1, metadataFile1, tempDir, trashDir);
                destinationProvider = new FileSyncProvider(destinationReplicaRootPath, filter, options, metadataPath2, metadataFile2, tempDir, trashDir);

                //destinationProvider.AppliedChange += new EventHandler<AppliedChangeEventArgs>(OnAppliedChange);
                //destinationProvider.SkippedChange += new EventHandler<SkippedChangeEventArgs>(OnSkippedChange);

                SyncOrchestrator agent = new SyncOrchestrator();
                agent.LocalProvider = sourceProvider;
                agent.RemoteProvider = destinationProvider;
                agent.Direction = SyncDirectionOrder.Upload; // Sync source to destination
                Console.WriteLine("Synchronizing changes to replica: " + destinationProvider.RootDirectoryPath);
                syncOperationStatistics = agent.Synchronize();

            }
            catch (Exception e)
            {
                throw e;
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
