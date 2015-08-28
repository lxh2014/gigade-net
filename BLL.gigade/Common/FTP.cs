using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Threading;

namespace BLL.gigade.Common
{
    public class FTP
    {
        private FtpWebRequest ftpRequest;
        private FtpWebResponse response;
        public FTP() { }
        public FTP(string requestUriString, string ftpUserID, string ftpPassword)
        {
            ftpRequest = FtpWebRequest.Create(requestUriString) as FtpWebRequest;
            ftpRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftpRequest.KeepAlive = false;
            ftpRequest.UsePassive = false;
            ftpRequest.UseBinary = true;
        }

        /// <summary>
        /// 判斷目錄是否存在
        /// </summary>
        /// <param name="directoryName">目錄名</param>
        /// <returns>目錄是否存在</returns>
        public bool DirectoryExist(string directoryName)
        {
            List<string> tmp = GetFileList();
            foreach (string s in tmp)
            {
                if (s.ToLower() == directoryName)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetFileList()
        {
            List<string> result = new List<string>();
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = ftpRequest.GetResponse() as FtpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名
            string line = reader.ReadLine();
            while (line != null)
            {
                result.Add(line);
                line = reader.ReadLine();
            }
            return result;
        } 

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public void MakeDirectory()
        {
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                response = (FtpWebResponse)ftpRequest.GetResponse();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("550") < 0)
                {
                    throw ex;
                }
                //throw ex;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 創建多層文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名</param>
        public void MakeMultiDirectory(string path, string[] Mappath, string ftpuser, string ftppwd)
        {

            FTP ftp = null;
            string fullPath = path;
            foreach (string s in Mappath)
            {
                ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                fullPath += s;

                if (!ftp.DirectoryExist(s.Replace("/", "")))
                {
                    ftp = new FTP(fullPath, ftpuser, ftppwd);
                    ftp.MakeDirectory();
                }
                fullPath += "/";
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="FileName"></param>
        public void UploadFile(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.ContentLength = fi.Length;
            Thread.Sleep(1000);
            Stream responseStream = ftpRequest.GetRequestStream();
            int buffer_count = 2048;
            byte[] buffer = new byte[buffer_count];

            FileStream fs = fi.OpenRead();
            int size = 0;
            try
            {
                while ((size = fs.Read(buffer, 0, buffer_count)) > 0)
                {
                    responseStream.Write(buffer, 0, size);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }

        /// <summary>
        /// 刪除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>成功與否</returns>
        public bool DeleteFile(string fileName)
        {
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            try
            {
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}