using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration.Install;

namespace SendMailService
{
    [RunInstaller(true)]
    public partial class SEInstall : Installer
    {
        private System.ComponentModel.IContainer components = null;
        private System.ServiceProcess.ServiceProcessInstaller spInstaller;
        private System.ServiceProcess.ServiceInstaller sInstaller;

        public SEInstall()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose();
        }
        #region 组件设计器生成的代码

        ///<summary>
        ///设计器支持所需的方法 - 不要
        ///使用代码编辑器修改此方法的内容。
        ///</summary>
        private void InitializeComponent()
        {
            this.spInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.sInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // spInstaller
            // 
            this.spInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.spInstaller.Password = null;
            this.spInstaller.Username = null;
            // 
            // sInstaller
            // 
            this.sInstaller.Description = "Send System Mail for gigade.admin";
            this.sInstaller.DisplayName = "SendMail";
            this.sInstaller.ServiceName = "SendMailService";
            // 
            // SEInstall
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spInstaller,
            this.sInstaller});

        }

        #endregion
    }
}
