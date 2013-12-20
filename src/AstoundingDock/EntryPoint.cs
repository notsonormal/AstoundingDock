using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;


namespace AstoundingApplications.AstoundingDock
{
    class EntryPoint
    {
        private static log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        public static void Main(string[] args)
        {           
            SingleInstanceManager manager = new SingleInstanceManager();
            try
            {
                manager.Run(args);
            }
            catch(Exception ex)
            {
                // This can sometimes run into the error 'This single-instance application could not connect to the original instance'
                // http://msdn.microsoft.com/en-us/library/ms184570(v=vs.80).aspx
               
                // Shutting down the original instance and starting it up again is probably the best choice
                // NOTE: This might be something that only happens in development

                Logger.WarnFormat("SingleInstanceManager.Run - Exception {0}", ex);
                try
                {
                    if (System.Windows.Application.Current.MainWindow != null)
                        System.Windows.Application.Current.MainWindow.Close();

                    System.Windows.Application.Current.Shutdown();
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);
                }
                catch (Exception ex2)
                {
                    Logger.WarnFormat("SingleInstanceManager.Run - Another exception {0}", ex2);
                }
            }
        }
    }

    class SingleInstanceManager : WindowsFormsApplicationBase
    {
        private static log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        App app;

        public SingleInstanceManager()
        {
            this.IsSingleInstance = true;
        }

        /// <summary>
        /// First time app is launched
        /// </summary>
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            Logger.DebugFormat("OnStartup");

            app = new App();
            app.Run();
            return false;
        }

        /// <summary>
        /// Subsequent launches
        /// </summary>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            Logger.DebugFormat("OnStartupNextInstance");            

            app.Activate();
            base.OnStartupNextInstance(eventArgs);
        }
    }
}
