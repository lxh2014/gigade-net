using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.gigade.Common
{
    /// <summary>
    /// 此类提供MD5，SHA1，SHA256，SHA512等四种算法，加密字串的长度依次增大。
    /// </summary>
    public class HashEncrypt
    {
        /// <summary>
        /// MD%加密，不区分大小写的
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="type">16位还是32位，16位就是取32位的第8到16位</param>
        /// <returns></returns>
        public string Md5Encrypt(string str, string type)
        {
            byte[] result = Encoding.UTF8.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            if (type == "16")
                return BitConverter.ToString(output).Replace("-", "").ToLower().Substring(8, 16);
            else
                return BitConverter.ToString(output).Replace("-", "").ToLower();

        }

        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="strIN">需要加密的字符串</param>
        /// <returns>密文</returns>
        public string SHA1_Encrypt(string Source_String)
        {
            byte[] StrRes = Encoding.UTF8.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        /// <summary>
        /// SHA256加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public string SHA256Encrypt(string str)
        {
            //System.Security.Cryptography.SHA256 s256 = new System.Security.Cryptography.SHA256Managed();
            //byte[] byte1;
            //byte1 = s256.ComputeHash(Encoding.UTF8.GetBytes(str));
            //s256.Clear();
            //return Convert.ToBase64String(byte1);

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            HashAlgorithm algorithm = null;
            algorithm = new SHA256Managed();
            return BitConverter.ToString(algorithm.ComputeHash(bytes)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// SHA384加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public string SHA384Encrypt(string str)
        {
            System.Security.Cryptography.SHA384 s384 = new System.Security.Cryptography.SHA384Managed();
            byte[] byte1;
            byte1 = s384.ComputeHash(Encoding.UTF8.GetBytes(str));
            s384.Clear();
            return Convert.ToBase64String(byte1);
        }

        /// <summary>
        /// SHA512加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public string SHA512Encrypt(string str)
        {
            System.Security.Cryptography.SHA512 s512 = new System.Security.Cryptography.SHA512Managed();
            byte[] byte1;
            byte1 = s512.ComputeHash(Encoding.UTF8.GetBytes(str));
            s512.Clear();
            return Convert.ToBase64String(byte1);
        }
    }
}
