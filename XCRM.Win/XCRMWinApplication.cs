using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.EF;
using System.Data.Common;
using XCRM.Module.Data;
using Demos.Data;
using System.Data.SQLite;
using System.Configuration;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XCRM.Win {
	public partial class XCRMWinApplication : WinApplication {
		#region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
		static XCRMWinApplication() {
			DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
			DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
            DevExpress.ExpressApp.ReportsV2.Win.WinReportServiceController.UseNewWizard = true;
        }
		private void InitializeDefaults() {
			LinkNewObjectToParentImmediately = false;
			OptimizedControllersCreation = true;
			EnableModelCache = true;
			UseLightStyle = true;
            SplashScreen = new DevExpress.ExpressApp.Win.Utils.DXSplashScreen(typeof(Demos.Win.XafDemoSplashScreen), new DefaultOverlayFormOptions());
            ExecuteStartupLogicBeforeClosingLogonWindow = true;
        }
		#endregion
		public XCRMWinApplication() {
			InitializeComponent();
			InitializeDefaults();
		}
		protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
			if(args.Connection != null) {
				args.ObjectSpaceProvider = new EFObjectSpaceProvider(typeof(XCRMDbContext), TypesInfo, null, (DbConnection)args.Connection);
			}
			else {
                args.ObjectSpaceProvider = DemoEFDatabaseHelper.CreateObjectSpaceProvider(typeof(XCRMDbContext), TypesInfo, args.ConnectionString, ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString);
			}
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
		}
        protected override DevExpress.ExpressApp.Core.ControllersManager CreateControllersManager() {
            DevExpress.ExpressApp.Core.ControllersManager result = base.CreateControllersManager();
            result.RegisterController(new Demo.Module.Win.DemoAboutInfoController());
            result.RegisterController(new Demo.Module.Win.DashboardCustomizeController());
            result.RegisterController(new Demos.Data.Win.UseSQLAlternativeInfoController());
            result.RegisterController(new Demo.Module.Win.RichTextEditHidePrintController());
            return result;
        }
        protected override void ProcessStartupActions() {
            base.ProcessStartupActions();
            if(UseSQLAlternativeInfoSingleton.Instance.UseAlternative) {
                PopupWindowShowAction showUseSQLAlternativeInfoAction = new PopupWindowShowAction();
                IObjectSpace objectSpace = this.CreateObjectSpace(typeof(UseSQLAlternativeInfo));
                UseSQLAlternativeInfo useSqlAlternativeInfo = objectSpace.GetObject<UseSQLAlternativeInfo>(UseSQLAlternativeInfoSingleton.Instance.Info);
                showUseSQLAlternativeInfoAction.CustomizePopupWindowParams += delegate (Object sender, CustomizePopupWindowParamsEventArgs e) {
                    e.View = this.CreateDetailView(objectSpace, useSqlAlternativeInfo, true);
                    e.DialogController.CancelAction.Active["Required"] = false;
                    e.IsSizeable = false;
                };
                Tracing.Tracer.LogVerboseText("showSQLAlternativeInfoAction is executing");
                StopSplash();//Dennis: We do not need to display splash while these startup windows are shown.
                showUseSQLAlternativeInfoAction.Application = this;
                showUseSQLAlternativeInfoAction.HandleException += (s, e) => {
                    if(!e.Handled) {
                        HandleException(e.Exception);
                        e.Handled = true;
                    }
                };

                using(PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(showUseSQLAlternativeInfoAction)) {
                    helper.ShowPopupWindow();
                }
                Tracing.Tracer.LogText("showSQLAlternativeInfoAction executed");
            }
        }
        private void XCRMWinApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}
        private void XCRMWinApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            // Just to read demo user name for logon.
            AuthenticationStandardLogonParameters logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if(logonParameters != null) {
                if(String.IsNullOrEmpty(logonParameters.UserName)) {
					logonParameters.UserName = XCRM.Module.Updater.AdministratorUserName;
                }
            }
        }
        /*private static XCRMXpoDataStoreProvider provider;
        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            base.OnCreateCustomObjectSpaceProvider(args);
            provider = new XCRMXpoDataStoreProvider();
            args.ObjectSpaceProvider = new ObjectSpaceProvider(provider);
            provider.Initialize(this.XPDictionary,
                string.Format(@"XpoProvider=XmlDataSet; Read Only=True; Data Source={0}nwind.xml", AppDomain.CurrentDomain.SetupInformation.ApplicationBase),
                this.ConnectionString);
        }*/
	}
}
