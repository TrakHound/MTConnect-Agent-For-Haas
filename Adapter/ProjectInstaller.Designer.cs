namespace MTConnect.Adapters.Haas
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AdapterServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AdapterServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AdapterServiceProcessInstaller
            // 
            this.AdapterServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.AdapterServiceProcessInstaller.Password = null;
            this.AdapterServiceProcessInstaller.Username = null;
            // 
            // AdapterServiceInstaller
            // 
            this.AdapterServiceInstaller.DisplayName = "MTConnect-Adapter-for-Haas";
            this.AdapterServiceInstaller.ServiceName = "MTConnect-Adapter-for-Haas";
            this.AdapterServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.AdapterServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.AdapterServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AdapterServiceProcessInstaller,
            this.AdapterServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AdapterServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AdapterServiceInstaller;
    }
}