using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Persistent.BaseImpl.EF;

namespace XCRM.Module.Data {
    public class MyAppUserValidationRules {
        public const string LastNameIsRequired = "MyAppUserLastNameIsRequired";
    }
    [VisibleInReports(false)]
    [DefaultProperty(nameof(MyAppUser.DisplayName))]
    [ImageName("BO_User")]
    [DisplayName("User")]
    [CurrentUserDisplayImage(nameof(Photo))]
    public class MyAppUser : DCUser, IXafEntityObject, IObjectSpaceLink {
        private Person person;

        public MyAppUser()
            : base() {
        }

        public virtual MediaDataObject Photo {
            get;
            set;
        }

        #region Person
        [Browsable(false)]
        public virtual Person Person {
            get { return person; }
            set {
                person = value;
				if (person != null) {
					DisplayName = Person.FullName;
				}
				else {
					DisplayName = string.Empty;
				}
            }
        }
        [ImmediatePostData]
        [NotMapped]
        [RuleRequiredField(MyAppUserValidationRules.LastNameIsRequired, DefaultContexts.Save)]
        public string LastName {
            get { return Person.LastName; }
            set {
                Person.LastName = value;
                DisplayName = Person.FullName;
            }
        }
        [ImmediatePostData]
        [NotMapped]
        public string FirstName {
            get { return Person.FirstName; }
            set {
                Person.FirstName = value;
                DisplayName = Person.FullName;
            }
        }
        [ImmediatePostData]
        [NotMapped]
        public Nullable<DateTime> BirthDate {
            get { return Person.BirthDate; }
            set { Person.BirthDate = value; }
        }
        [ImmediatePostData]
        [NotMapped]
        public string JobTitle {
            get { return Person.JobTitle; }
            set { Person.JobTitle = value; }
        }
        [ImmediatePostData]
        [NotMapped]
        public Gender Gender {
            get { return Person.Gender; }
            set { Person.Gender = value; }
        }
        [ImmediatePostData]
        [NotMapped]
        public MaritalStatus MaritalStatus {
            get { return Person.MaritalStatus; }
            set { Person.MaritalStatus = value; }
        }
        [ImmediatePostData]
        [NotMapped]
        public string SpouseName {
            get { return Person.SpouseName; }
            set { Person.SpouseName = value; }
        }
        [ImmediatePostData]
        [NotMapped]
        public Nullable<DateTime> Anniversary {
            get { return Person.Anniversary; }
            set { Person.Anniversary = value; }
        }
        #endregion

        #region IXafEntityObject
        public void OnCreated() {
            Person = ObjectSpace.CreateObject<Person>();
            Photo = ObjectSpace.CreateObject<MediaDataObject>();
        }
        public void OnLoaded() { }
        public void OnSaving() { }
        #endregion

        #region IObjectSpaceLink
        [NotMapped]
        [Browsable(false)]
        public IObjectSpace ObjectSpace { get; set; }
        #endregion
    }
}
