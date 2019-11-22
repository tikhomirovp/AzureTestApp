using System;
using System.Configuration;
using System.Data.Common;
using System.Reflection;
using System.Windows.Forms;
using Demos.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
// TODO: DXCORE3
#if !DXCORE3
using DevExpress.Tutorials;
#endif
using DevExpress.XtraEditors;

namespace XCRM.Win {
    public class Program {
        private static void winApplication_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e) {
            e.FormattingCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }

        [STAThread]
        public static void Main(string[] arguments) {
            WindowsFormsSettings.LoadApplicationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(Tracing.GetFileLocationFromSettings() == FileLocation.CurrentUserApplicationDataFolder) {
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
            }
            Tracing.Initialize();
            DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
#if NETSTANDARD || NETCOREAPP
            DbProviderFactories.RegisterFactory("System.Data.SQLite.EF6", System.Data.SQLite.SQLiteFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);
#endif
            XCRMWinApplication winApplication = new XCRMWinApplication();
// TODO: DXCORE3
#if !DXCORE3
            Demos.Feedback.XAFFeedbackHelper helper = new Demos.Feedback.XAFFeedbackHelper(winApplication);
#endif
#if EasyTest
            try {
                DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
            }
            catch(Exception) { }
            DevExpress.XtraScheduler.Internal.Diagnostics.XtraSchedulerDebug.SkipInsertionCheck = true;
#endif
            winApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(winApplication_CustomizeFormattingCulture);
            SecurityAdapterHelper.Enable();
            try {
                string connectionString = null;
                ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                if(connectionStringSettings != null) {
                    connectionString = connectionStringSettings.ConnectionString;
                }
                if(string.IsNullOrEmpty(winApplication.ConnectionString) && winApplication.Connection == null) {
                    connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                    if(connectionStringSettings != null) {
                        connectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(connectionStringSettings.ConnectionString);
                        if(connectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString) {
                            connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
                            UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.SQLServerIsNotFoundMessage, DemoEFDatabaseHelper.AlternativeName, DemoEFDatabaseHelper.SQLiteUsageMessage);
                        }
                    }
                }
                winApplication.ConnectionString = connectionString;
                if(System.Diagnostics.Debugger.IsAttached && winApplication.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                    winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
                }
                winApplication.Setup();
                winApplication.Start();
            }
            catch(Exception e) {
                winApplication.StopSplash();
                winApplication.HandleException(e);
            }
        }
    }

}
