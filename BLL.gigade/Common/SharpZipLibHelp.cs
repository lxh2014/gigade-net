using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
namespace BLL.gigade.Common
{
   public class SharpZipLibHelp
    {
        /// <summary>
        /// 生成压缩文件
        /// </summary>
        /// <param name="strZipPath">生成的zip文件的路径</param>
        /// <param name="strZipTopDirectoryPath">源文件的上级目录</param>
        /// <param name="intZipLevel">T压缩等级</param>
        /// <param name="strPassword">压缩包解压密码</param>
        /// <param name="filesOrDirectoriesPaths">源文件路径</param>
        /// <returns></returns>
       public bool Zip(string strZipPath, string strZipTopDirectoryPath, int intZipLevel, string strPassword, List<string> filesOrDirectoriesPaths)
        {
            try
            {
                List<string> AllFilesPath = new List<string>();
                if (filesOrDirectoriesPaths.Count > 0) // get all files path
                {
                    for (int i = 0; i < filesOrDirectoriesPaths.Count; i++)
                    {
                        if (File.Exists(filesOrDirectoriesPaths[i]))
                        {
                            AllFilesPath.Add(filesOrDirectoriesPaths[i]);
                        }
                        else if (Directory.Exists(filesOrDirectoriesPaths[i]))
                        {
                            GetDirectoryFiles(filesOrDirectoriesPaths[i], AllFilesPath);
                        }
                    }
                }

                if (AllFilesPath.Count > 0)
                {

                    ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(strZipPath));
                    zipOutputStream.SetLevel(intZipLevel);
                    zipOutputStream.Password = strPassword;

                    for (int i = 0; i < AllFilesPath.Count; i++)
                    {
                        string strFile = AllFilesPath[i].ToString();
                        try
                        {
                            if (strFile.Substring(strFile.Length - 1) == "") //folder
                            {
                                string strFileName = strFile.Replace(strZipTopDirectoryPath, "");
                                if (strFileName.StartsWith(""))
                                {
                                    strFileName = strFileName.Substring(1);
                                }
                                ZipEntry entry = new ZipEntry(strFileName);
                                entry.DateTime = DateTime.Now;
                                zipOutputStream.PutNextEntry(entry);
                            }
                            else //file
                            {
                                FileStream fs = File.OpenRead(strFile);

                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);

                                string strFileName = strFile.Replace(strZipTopDirectoryPath, "");
                                if (strFileName.StartsWith(""))
                                {
                                    strFileName = strFileName.Substring(0);
                                }
                                ZipEntry entry = new ZipEntry(strFileName);
                                entry.DateTime = DateTime.Now;
                                zipOutputStream.PutNextEntry(entry);
                                zipOutputStream.Write(buffer, 0, buffer.Length);

                                fs.Close();
                                fs.Dispose();
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    zipOutputStream.Finish();
                    zipOutputStream.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the directory files.
        /// </summary>
        /// <param name="strParentDirectoryPath">源文件路径</param>
        /// <param name="AllFilesPath">所有文件路径</param>
        private void GetDirectoryFiles(string strParentDirectoryPath, List<string> AllFilesPath)
        {
            string[] files = Directory.GetFiles(strParentDirectoryPath);
            for (int i = 0; i < files.Length; i++)
            {
                AllFilesPath.Add(files[i]);
            }
            string[] directorys = Directory.GetDirectories(strParentDirectoryPath);
            for (int i = 0; i < directorys.Length; i++)
            {
                GetDirectoryFiles(directorys[i], AllFilesPath);
            }
            if (files.Length == 0 && directorys.Length == 0) //empty folder
            {
                AllFilesPath.Add(strParentDirectoryPath);
            }
        }
    }
}
