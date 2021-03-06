//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BASIC.AUTHEN.DATA
{
    using System;
    using System.Collections.Generic;
    
    public partial class InvoiceCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvoiceCategory()
        {
            this.Comp_InvCat_InvTemp = new HashSet<Comp_InvCat_InvTemp>();
            this.Decision_Company_Invoice = new HashSet<Decision_Company_Invoice>();
            this.InvoiceTemplates = new HashSet<InvoiceTemplate>();
        }
    
        public System.Guid InvoiceCategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsPublic { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.Guid> LastModifiedByUserId { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comp_InvCat_InvTemp> Comp_InvCat_InvTemp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Decision_Company_Invoice> Decision_Company_Invoice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceTemplate> InvoiceTemplates { get; set; }
    }
}
