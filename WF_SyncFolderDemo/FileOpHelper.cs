using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_SyncFolderDemo
{
    public class FileOpHelper
    {
        public static string getDirectoryName(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return null;

            if (dirPath.LastIndexOf(@"\") > -1)
            {
                return dirPath.Substring(dirPath.LastIndexOf(@"\") + 1);
            }
            return null;
        }

        public static string getFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            return Path.GetFileName(filePath);
        }

        public static bool isFile(string path)
        {
            bool flag;
            try
            {
                FileAttributes attr = File.GetAttributes(path);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    flag = false;
                else
                    flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static bool CopyDirectory(string sourcePath, string destPath, bool overwriteexisting = false)
        {
            bool ret = false;
            try
            {
                sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
                destPath = destPath.EndsWith(@"\") ? destPath : destPath + @"\";

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destPath) == false)
                        Directory.CreateDirectory(destPath);

                    string destFilePath;
                    foreach (string fls in Directory.GetFiles(sourcePath))
                    {
                        if (!FileStatusHelper.IsFileOccupied(fls))
                        {
                            FileInfo flinfo = new FileInfo(fls);
                            destFilePath = destPath + "\\" + flinfo.Name;
                            flinfo.CopyTo(destFilePath, overwriteexisting);
                            //else
                            //{
                            //    if (flinfo.LastWriteTime.CompareTo(new FileInfo(destFilePath).LastWriteTime) > 0)
                            //    {
                            //        flinfo.CopyTo(destFilePath, true);
                            //    }
                            //}
                        }

                    }
                    foreach (string drs in Directory.GetDirectories(sourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, destPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
            return ret;
        }

        public static bool CopySingleFile(string sourcePath, string destPath, bool overwriteexisting = false)
        {
            bool ret = false;
            try
            {
                sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
                destPath = destPath.EndsWith(@"\") ? destPath : destPath + @"\";

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destPath) == false)
                        Directory.CreateDirectory(destPath);

                    string destFilePath;
                    if (isFile(destPath))
                    {
                        FileInfo flinfo = new FileInfo(destPath);
                        if (!File.Exists(destPath))
                        {
                            flinfo.CopyTo(destPath, overwriteexisting);
                        }
                    }

                    foreach (string drs in Directory.GetDirectories(sourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, destPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
            return ret;
        }

        internal static void DeleteDirOrFile(string destPath)
        {
            if (!FileStatusHelper.IsFileOccupied(destPath))
            {
                if (Directory.Exists(destPath))
                {
                    Directory.Delete(destPath, true);
                }
                if (File.Exists(destPath))
                {
                    File.Delete(destPath);
                }
            }

        }
    }
}
