using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace XCRM.Module.Controllers {
    public class DisableValidationOnDeleteController : WindowController {
        private void RuleSet_CustomNeedToValidateRule(object sender, DevExpress.Persistent.Validation.CustomNeedToValidateRuleEventArgs e) {
            if(e.ContextId == "Save") {
                e.Handled = true;
                e.NeedToValidateRule = false;
            }
        }
        protected override void OnActivated() {
            base.OnActivated();
            DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            if(deleteObjectsViewController != null) {
                deleteObjectsViewController.DeleteAction.Executing += delegate (object sender, System.ComponentModel.CancelEventArgs e) {
                    DevExpress.Persistent.Validation.RuleSet.CustomNeedToValidateRule -= RuleSet_CustomNeedToValidateRule;
                    DevExpress.Persistent.Validation.RuleSet.CustomNeedToValidateRule += RuleSet_CustomNeedToValidateRule;
                };
                DevExpress.Persistent.Validation.Validator.RuleSet.ValidationCompleted += delegate (object sender, DevExpress.Persistent.Validation.ValidationCompletedEventArgs e) {
                    if(e.Result.State == DevExpress.Persistent.Validation.ValidationState.Invalid) {
                        DevExpress.Persistent.Validation.RuleSet.CustomNeedToValidateRule -= RuleSet_CustomNeedToValidateRule;
                    }
                };
                deleteObjectsViewController.DeleteAction.Executed += delegate (object sender, DevExpress.ExpressApp.Actions.ActionBaseEventArgs e) {
                    DevExpress.Persistent.Validation.RuleSet.CustomNeedToValidateRule -= RuleSet_CustomNeedToValidateRule;
                };
            }
        }
    }
}
