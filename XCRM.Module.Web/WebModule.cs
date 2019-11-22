using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace XCRM.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class XCRMAspNetModule : ModuleBase {
        public const string DataModificationsExceptionMessage = "Data modifications are not allowed in this online demo. You can test data editing functionality by installing XAF on your machine and running the demo locally.";

        public XCRMAspNetModule() {
            InitializeComponent();
            SiteMode = false;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
        public bool SiteMode { get; set; }
    }
}
