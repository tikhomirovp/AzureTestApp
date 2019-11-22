using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;

namespace XCRM.Module.Controllers {
    public class CustomDeleteObjectsController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.CustomDeleteObjects += ObjectSpace_CustomDeleteObjects;
        }
        protected override void OnDeactivated() {
            ObjectSpace.CustomDeleteObjects -= ObjectSpace_CustomDeleteObjects;
            base.OnDeactivated();
        }
        private void ForceLoadDependencies(object objectToDelete) {
            Type type = objectToDelete.GetType();
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            if(typeInfo != null) {
                foreach(IMemberInfo mi in typeInfo.Members) {
                    if(mi != null && mi.IsProperty) {
                        if(mi.IsList) {
                            IList list = mi.GetValue(objectToDelete) as IList;
                        }
                        else {
                            mi.GetValue(objectToDelete);
                        }
                    }
                }
            }
        }
        void ObjectSpace_CustomDeleteObjects(object sender, CustomDeleteObjectsEventArgs e) {
            if(e.Objects != null) {
                foreach(object objectToDelete in e.Objects) {
                    object deletedItem = objectToDelete;
                    if(deletedItem != null) {
                        if(deletedItem is EFDataViewRecord) {
                            deletedItem = ObjectSpace.GetObject(objectToDelete);
                        }
                        ForceLoadDependencies(deletedItem);
                    }
                }
            }
        }
    }
}
