using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using XCRM.Module.Data;

namespace Demo.Module.Win {
    public class RichTextEditHidePrintController : ViewController<ListView> {
        public RichTextEditHidePrintController() {
            TargetObjectType = typeof(Note);
        }
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<PrintingController>()?.Active.SetItemValue("RichTextEdit", false);
            Frame.GetController<WinExportController>()?.Active.SetItemValue("RichTextEdit", false);

        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<PrintingController>()?.Active.RemoveItem("RichTextEdit");
            Frame.GetController<WinExportController>()?.Active.RemoveItem("RichTextEdit");
        }
    }
}
