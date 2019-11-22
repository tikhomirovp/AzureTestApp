using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.SystemModule;

namespace XCRM.Module.Web.Controllers {

    public class WebXCRMDeleteObjectsViewController : WebDeleteObjectsViewController {
        protected override void Delete(SimpleActionExecuteEventArgs args) {
            if(Application.Modules.FindModule<Module.Web.XCRMAspNetModule>().SiteMode) {
                throw new UserFriendlyException(XCRMAspNetModule.DataModificationsExceptionMessage);
            }
            else {
                base.Delete(args);
            }
        }
    }
}
