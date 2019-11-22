using System.Collections.Generic;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Demos.Feedback {
    public class XAFFeedbackHelper {
        static HashSet<string> openedViews = new HashSet<string>();
        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
         
        }
        private string GetOpenedViews() {
            string result = "Opened Views \n";
            foreach(string view in openedViews) {
                result += view + "\n";
            }
            return result;
        }
        private void Application_ViewCreated(object sender, ViewCreatedEventArgs e) {
            openedViews.Add(e.View.Id);
        }
        public XAFFeedbackHelper(XafApplication application) {
            application.CustomizeTemplate += Application_CustomizeTemplate;
            application.ViewCreated += Application_ViewCreated;
        }
    }
}
