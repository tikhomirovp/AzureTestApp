using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using XCRM.Module.Data;

namespace XCRM.Module.Web.Controllers {
    public class RefreshDataSourceController : ObjectViewController<DetailView, CRMAccount> {
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
        }
        protected override void OnDeactivated() {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            base.OnDeactivated();
        }
        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            ASPxGridLookupPropertyEditor editor = View.FindItem("PrimaryContact") as ASPxGridLookupPropertyEditor;
            if(editor != null) {
                editor.RefreshDataSource();
            }
        }
    }
}
