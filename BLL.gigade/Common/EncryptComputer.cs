using BLL.gigade.Mgr;
using BLL.gigade.Model.APIModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BLL.gigade.Common
{
    public static class EncryptComputer
    {

        private static byte[] _salt = Encoding.ASCII.GetBytes("4159eecb1277e4d79f8402edae447b");
        private static string _secret = "e719f4b9a693072e41b5295e867f4b";

        #region AES加密 +  string EncryptStringAES(string plainText)
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">要加密的字串</param>
        /// <returns>加密后的字串</returns>
        public static string EncryptStringAES(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            string outStr = null;
            RijndaelManaged aesAlg = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_secret, _salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return outStr;
        } 
        #endregion

        #region AES解密 +string DecryptStringAES(string cipherText)
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">要解密的字串</param>
        /// <returns>解密后的字串</returns>
        public static string DecryptStringAES(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_secret, _salt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        } 
       

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
        #endregion

        /// <summary>
        /// 加密解密
        /// </summary>
        /// <param name="plainText">需要加密的list</param>
        /// <param name="isEncrypt">true加密，false解密</param>
        /// <returns>加密或者解密之後的結果</returns>
        public static List<AesResult> EncryptDecryptListByApi(List<PlainTextViewModel> plainModeList, bool isEncrypt = true)
        {
            try
            {

                GigadeApiRequest request = new GigadeApiRequest();
                string url = string.Empty;
                if (isEncrypt)
                {
                    url = "api/Utility/EncryptWithAes";
                }
                else
                {
                    url = "api/Utility/DecryptWithAes";
                }
                var result = request.Request<List<PlainTextViewModel>, List<AesResult>>("api/Utility/EncryptWithAes", plainModeList);
                if (result.success)
                {
                    return result.result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        #region 加密解密單一字串 +string EncryptDecryptTextByApi(string plainText, bool isEncrypt = true)
        /// <summary>
        /// 加密解密單一字串
        /// </summary>
        /// <param name="plainText">需要加密的list</param>
        /// <param name="isEncrypt">true加密，false解密</param>
        /// <returns>加密或者解密之後的結果</returns>
        public static string EncryptDecryptTextByApi(string plainText, bool isEncrypt = true)
        {
            try
            {
                List<PlainTextViewModel> paintList = new List<PlainTextViewModel>();
                PlainTextViewModel paintModel = new PlainTextViewModel();
                paintModel.text = plainText;
                paintList.Add(paintModel);
                GigadeApiRequest request = new GigadeApiRequest();
                string url = string.Empty;
                if (isEncrypt)
                {
                    url = "api/Utility/EncryptWithAes";
                }
                else
                {
                    url = "api/Utility/DecryptWithAes";
                }
                var result = request.Request<List<PlainTextViewModel>, List<AesResult>>(url, paintList);
                if (result.success)
                {
                    List<AesResult> aesResult = result.result;
                    return aesResult.FirstOrDefault<AesResult>().computed_text;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        } 
        #endregion

    }
}
