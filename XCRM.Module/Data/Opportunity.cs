using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XCRM.Module.Data {
    public enum OpportunityStatus {
        None = 0,
        Won = 1,
        Canceled = 2,
        [XafDisplayName("Out-Sold")]
        OutSold = 3
    }
    public enum OpportunityRating {
        Hot = 0,
        Warm = 1,
        Cold = 2
    }

    public class OpportunityValidationRules {
        public const string ProbabilityIsBeetwen0And100 = "ProbabilityyIsBeetwen0And100";
    }

    public interface IOpportunityCustomer {
        IList<Opportunity> Opportunities { get; set; }
    }

    public interface IOpportunityTarget {
        Opportunity SourceOpportunity { get; set; }
    }

    public interface IOpportunityInvoice : IOpportunityTarget {
        Opportunity BackRefOpportunity { get;  set; }
    }

    [VisibleInReports]
    [ImageName("BO_Opportunity")]
    [Appearance("Disable IOpportunity by StatusReason", TargetItems = "*", Criteria = "Status != ##Enum#XCRM.Module.Data.OpportunityStatus,None#", Enabled = false)]
    [Appearance("Hide Amount, Discount, DiscountPercent by Empty Criteria", TargetItems = "Amount, Discount, DiscountPercent", Criteria = "", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Enable EstimatedRevenue", TargetItems = nameof(EstimatedRevenue), Criteria = nameof(RevenueIsUserProvided), Enabled = true)]
    [Appearance("Disable EstimatedRevenue", TargetItems = nameof(EstimatedRevenue), Criteria = "!RevenueIsUserProvided", Enabled = false)]
    [Table("Opportunities")]
    [DisplayName("Opportunity")]
    public class Opportunity : SaleBase{
        public Opportunity()
            : base() {
                Invoices = new List<CRMInvoice>();
                Quotes = new List<Quote>();
                Orders = new List<Order>();
        }
        private bool revenueIsUserProvided;
        private Customer customer;
        private OpportunityStatus status;

        public OpportunityRating Rating { get; set; }
        public OpportunityStatus Status {
            get { return status; }
            set {
                if(status != value) {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal EstimatedRevenue { get; set; }
        [VisibleInListView(false)]
        public decimal ActualRevenue { get; set; }
        [RuleRange(OpportunityValidationRules.ProbabilityIsBeetwen0And100, DefaultContexts.Save, 0, 100)]
        public int Probability { get; set; }
        [VisibleInListView(false)]
        public Nullable<DateTime> CloseDate { get; set; }
        [VisibleInListView(false)]
        public Nullable<DateTime> EstimatedCloseDate { get; set; }
        [InverseProperty(nameof(CRMInvoice.BackRefOpportunity))]
        public virtual IList<CRMInvoice> Invoices { get; set; }
        [ImmediatePostData]
        [VisibleInListView(false)]
        public bool RevenueIsUserProvided {
            get { return revenueIsUserProvided; }
            set {
                revenueIsUserProvided = value;
                UpdateAmount();
                //Fix for EF & Conditional Appearance
                OnPropertyChanged();
            }
        }
        [System.ComponentModel.Browsable(false)]
        [InverseProperty(nameof(Data.Customer.Opportunities))]
        public virtual Customer Customer {
            get { return customer; }
            set { customer = value; PotentialCustomer = Customer; }
        }

        [InverseProperty(nameof(Quote.BackRefOpportunity))]
        public virtual IList<Quote> Quotes { get; set; }

        [InverseProperty(nameof(Order.Opportunity))]
        public virtual IList<Order> Orders { get; set; }

        public override void UpdateAmount() {
            if (!IsLoaded && !IsCreated) { return; }

            Amount = DetailAmount;

            if (!RevenueIsUserProvided) {
                EstimatedRevenue = Amount;
            }
        }
        protected override void PotentialCustomerUpdated() {
            if (Customer != PotentialCustomer) {
                Customer = PotentialCustomer;
            }
        }
        //TODO: Move to controller
        [Action(PredefinedCategory.View, Caption = "Close Opportunity...", AutoCommit = true,
            TargetObjectsCriteria = "Status = ##Enum#XCRM.Module.Data.OpportunityStatus,None#",
            SelectionDependencyType = MethodActionSelectionDependencyType.RequireSingleObject)]
        public void Close(CloseOpportunityParameters parameters) {
            Status = parameters.Status;
            CloseDate = parameters.CloseDate;
            ActualRevenue = parameters.ActualRevenue;
        }
    }

    [DomainComponent]
    public class CloseOpportunityParameters {
        public CloseOpportunityParameters(Opportunity opportunity) {
            this.CloseDate = DateTime.Now;
            this.ActualRevenue = opportunity.Amount;
        }
        public OpportunityStatus Status { get; set; }
        public decimal ActualRevenue { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
    }
}
