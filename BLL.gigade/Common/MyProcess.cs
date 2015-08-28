using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
namespace BLL.gigade.Common
{
    public class MyProcess
    {
        private Process p;
        public MyProcess(string userName,string password)
        {
            string passwd = password;
            SecureString ss = new SecureString();
            foreach (char c in passwd)
            {
                ss.AppendChar(c);
            }
            p = new Process();
            p.StartInfo.UserName = userName;
            p.StartInfo.Password = ss;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true; 
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }
        public bool Start() {
           return p.Start();
        }

        public void ExecCMD(string cmd)
        {
            p.StandardInput.WriteLine(cmd);
        }

        public void Exit() {
             p.StandardInput.WriteLine("exit");
             p.WaitForExit();
        }

        public void Close() {
            if (p != null)
                p.Close();
        }
        public string GetResult() {
            if (p == null)
                return string.Empty;
            return p.StandardOutput.ReadToEnd();
        }
    }
}
