using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_SyncFolderDemo
{
    public class MyFileInfo
    {
        public string Path { get; set; }

        public FileType FileType { get; set; }
    }

    public enum FileType
    {
        Directory = 1,
        File = 2
    }

}
