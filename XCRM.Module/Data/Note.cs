using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace XCRM.Module.Data {
    public class NoteValidationRules {
        public const string NoteTitleIsRequired = nameof(NoteTitleIsRequired);
    }

    [DefaultProperty(nameof(Note.Title))]
    [DisplayName("Note")]
    public class Note {
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int ID { get; protected set; }

        [RuleRequiredField(NoteValidationRules.NoteTitleIsRequired, DefaultContexts.Save)]
        [MaxLength(255)]
        public string Title { get; set; }

        [DevExpress.Xpo.Size(DevExpress.Xpo.SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.RichTextPropertyEditor)]
        [VisibleInListView(true)]
        public string Description { get; set; }

        public virtual FileData Attachment { get; set; }
    }
}
