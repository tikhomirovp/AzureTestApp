using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XCRM.Module.Data {
    public enum InvoiceStatus {
        None = 0,
        Canceled = 1,
        Completed = 2,
        Partial = 3
    }

    [ImageName("BO_Sale")]
    [VisibleInReports]
    [Appearance("Disable Paid Invoice", TargetItems = "*", Criteria = "[Status] = ##Enum#XCRM.Module.Data.InvoiceStatus,Completed#", Enabled = false)]
    [ListViewAutoFilterRowAttribute(true)]
    [Table("Invoices")]
    [DisplayName("Invoice")]
    public class Invoice : SaleBase {
        private Customer customer;
        private InvoiceStatus status;

        public InvoiceStatus Status {
            get { return status; }
            set {
                if(status != value) {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        [VisibleInListView(false)]
        public Nullable<DateTime> DueDate { get; set; }

        [System.ComponentModel.Browsable(false)]
        [VisibleInListView(false)]
        [InverseProperty(nameof(Data.Customer.Invoices))]
        public virtual Customer Customer {
            get { return customer; }
            set { customer = value; PotentialCustomer = Customer; }
        }

        protected override void PotentialCustomerUpdated() {
            if (Customer != PotentialCustomer) {
                Customer = PotentialCustomer;
            }
        }

        // TODO: Move to controller
        [Action(PredefinedCategory.View, Caption = "Close Invoice...", AutoCommit = true,
            TargetObjectsCriteria = "Status = ##Enum#XCRM.Module.Data.InvoiceStatus,None#",
            SelectionDependencyType = MethodActionSelectionDependencyType.RequireSingleObject)]
        public void Close(CloseInvoice convertParams) {
            Status = convertParams.Status;
        }
    }

    [DomainComponent]
    public class CloseInvoice {
        public InvoiceStatus Status { get; set; }
    }
}
