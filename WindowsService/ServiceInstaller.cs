using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsService
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        private System.ComponentModel.IContainer components = null;
        private System.ServiceProcess.ServiceProcessInstaller spInstaller;
        private System.ServiceProcess.ServiceInstaller sInstaller;

        public ServiceInstaller()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        ///<summary>
        ///设计器支持所需的方法不要
        ///使用代DN码编辑器修改此方法的内容
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
            this.sInstaller.Description = "去掉商品前後綴以及更改商品狀態";
            this.sInstaller.DisplayName = "吉甲地服務";
            this.sInstaller.ServiceName = "gigadeService";
            this.sInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spInstaller,
            this.sInstaller});

        }
        #endregion
    }
}
