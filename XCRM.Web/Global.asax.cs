using System;
using System.Configuration;
using System.Web;
using System.Web.Routing;
using Demos.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Web.Demos;

namespace XCRM.Web {
    public class Global : System.Web.HttpApplication {
        private static void webApplication_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e) {
            e.FormattingCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }

        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(object sender, EventArgs e) {
            AboutInfo.Instance.Copyright = AssemblyInfo.AssemblyCopyright + " All Rights Reserved";
            RouteTable.Routes.RegisterXafRoutes();
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EasyTest
            DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(object sender, EventArgs e) {
            Tracing.Initialize();

            WebApplication.SetInstance(Session, new XCRM.Web.XCRMWebApplication());
            WebApplication.Instance.SwitchToNewStyle();
            WebApplication.Instance.Settings.DefaultVerticalTemplateContentPath = "CustomDefaultVerticalTemplateContent.ascx";
            WebApplication.Instance.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(webApplication_CustomizeFormattingCulture);
            SecurityAdapterHelper.Enable();
            ConnectionStringSettings connectionStringSettings;
            string connectionString = null;
            if(ConfigurationManager.AppSettings["SiteMode"] != null && ConfigurationManager.AppSettings["SiteMode"].ToLower() == "true") {
                WebApplication.Instance.Modules.FindModule<Module.Web.XCRMAspNetModule>().SiteMode = true;
                connectionString = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;
                WebApplication.Instance.ObjectSpaceCreated += (s, args) => {
                    args.ObjectSpace.Committing += ObjectSpace_Committing;
                    args.ObjectSpace.Disposed += ObjectSpace_Disposed;
                };
            }
            else if(string.IsNullOrEmpty(connectionString)) {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                if(connectionStringSettings != null) {
                    connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
                if(string.IsNullOrEmpty(WebApplication.Instance.ConnectionString) && WebApplication.Instance.Connection == null) {
                    connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                    if(connectionStringSettings != null) {
                        connectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(connectionStringSettings.ConnectionString);
                        if(connectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString) {
                            connectionString = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;
                            UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.SQLServerIsNotFoundMessage, DemoEFDatabaseHelper.AlternativeName, DemoEFDatabaseHelper.SQLiteUsageMessage);
                        }
                    }
                }
            }
            WebApplication.Instance.ConnectionString = connectionString;

            //if(System.Diagnostics.Debugger.IsAttached && WebApplication.Instance.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
            WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            //}

            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        private void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            if(CanThrowException((IObjectSpace)sender)) {
                throw new UserFriendlyException(Module.Web.XCRMAspNetModule.DataModificationsExceptionMessage);
            }
            else {
                throw new System.ServiceModel.FaultException(Module.Web.XCRMAspNetModule.DataModificationsExceptionMessage);
            }
        }
        private void ObjectSpace_Disposed(object sender, EventArgs e) {
            IObjectSpace os = (IObjectSpace)sender;
            os.Committing -= ObjectSpace_Committing;
            os.Disposed -= ObjectSpace_Disposed;
        }
        private bool CanThrowException(IObjectSpace os) {
            bool result = true;
            foreach(object obj in os.ModifiedObjects) {
                if(obj is DevExpress.ExpressApp.ReportsV2.IReportDataV2) {
                    result = false;
                    break;
                }
                if(obj is IDashboardData) {
                    result = false;
                    break;
                }
            }
            return result;
        }
        protected void Application_BeginRequest(object sender, EventArgs e) {
            ASPxWebControl.GlobalThemeBaseColor = Utils.CurrentBaseColor;
            ASPxWebControl.GlobalThemeFont = Utils.CurrentFont;
        }
        protected void Application_EndRequest(object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
        }
        protected void Application_Error(object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(object sender, EventArgs e) {
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(object sender, EventArgs e) {
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
