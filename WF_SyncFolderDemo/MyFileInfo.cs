using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_SyncFolderDemo
{
    public class MyFileInfo
    {
        public string SourcePath { get; set; }

        public string DestinationPath { get; set; }

        public FileType FileType { get; set; }

        public WatcherChangeTypes ChnageType { get; set; }
    }

    public enum FileType
    {
        Directory = 1,
        File = 2
    }

}
