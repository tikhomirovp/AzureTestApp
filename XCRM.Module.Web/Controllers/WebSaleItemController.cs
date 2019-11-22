using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using XCRM.Module.Data;

namespace XCRM.Module.Web.Controllers {
    public class WebSaleItemController : ObjectViewController<DetailView, SaleBase> {
        private ASPxDecimalPropertyEditor discountPropertyEditor;
        private ASPxDecimalPropertyEditor discountPercentPropertyEditor;
        protected override void OnActivated() {
            base.OnActivated();
            discountPropertyEditor = View.FindItem(nameof(SaleBase.Discount)) as ASPxDecimalPropertyEditor;
            if(discountPropertyEditor != null) {
                discountPropertyEditor.ControlCreated += DiscountPropertyEditor_ControlCreated;
                UpdateControl(discountPropertyEditor.Editor, 10000);
            }
            discountPercentPropertyEditor = View.FindItem(nameof(SaleBase.DiscountPercent)) as ASPxDecimalPropertyEditor;
            if(discountPercentPropertyEditor != null) {
                discountPercentPropertyEditor.ControlCreated += DiscountPercentPropertyEditor_ControlCreated;
                UpdateControl(discountPercentPropertyEditor.Editor, 100);
            }
        }
        protected override void OnDeactivated() {
            if(discountPropertyEditor != null) {
                discountPropertyEditor.ControlCreated -= DiscountPropertyEditor_ControlCreated;
                discountPropertyEditor = null;
            }
            if(discountPercentPropertyEditor != null) {
                discountPercentPropertyEditor.ControlCreated -= DiscountPercentPropertyEditor_ControlCreated;
                discountPercentPropertyEditor = null;
            }
            base.OnDeactivated();
        }
        private void DiscountPropertyEditor_ControlCreated(object sender, EventArgs e) {
            UpdateControl(discountPropertyEditor.Editor, 10000);
        }
        private void DiscountPercentPropertyEditor_ControlCreated(object sender, EventArgs e) {
            UpdateControl(discountPercentPropertyEditor.Editor, 100);
        }
        private static void UpdateControl(ASPxSpinEdit spinEdit, int maxValue) {
            if(spinEdit != null) {
                spinEdit.MinValue = 0;
                spinEdit.MaxValue = maxValue;
            }
        }
    }
}
