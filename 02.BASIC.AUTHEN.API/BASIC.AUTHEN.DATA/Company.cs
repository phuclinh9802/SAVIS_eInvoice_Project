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
    
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            this.Certificates = new HashSet<Certificate>();
            this.Comp_InvCat_InvTemp = new HashSet<Comp_InvCat_InvTemp>();
            this.Customers = new HashSet<Customer>();
            this.Decision_Company_Invoice = new HashSet<Decision_Company_Invoice>();
            this.Products = new HashSet<Product>();
        }
    
        public System.Guid CompanyId { get; set; }
        public System.Guid CompanyTypeId { get; set; }
        public string Name { get; set; }
        public string Name_F { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string Address_F { get; set; }
        public string Description { get; set; }
        public string Description_F { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool Status { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.Guid> LastModifiedByUserId { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public string SAPCode { get; set; }
        public string Fax { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public Nullable<int> Ext { get; set; }
        public string TaxNumber { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseAddress { get; set; }
        public string Picture { get; set; }
        public string FormNo { get; set; }
        public string Sign { get; set; }
        public Nullable<System.Guid> NewsId { get; set; }
        public string BankAccountName { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string RepresentPerson { get; set; }
        public Nullable<System.Guid> TaxAuthorityId { get; set; }
        public Nullable<System.Guid> AccountId { get; set; }
        public string SignatureImage { get; set; }
        public string Domain { get; set; }
        public bool Approved { get; set; }
        public string MailContact { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string PortalLink { get; set; }
        public bool IsUsed { get; set; }
        public Nullable<int> SignType { get; set; }
        public Nullable<int> DatePoint { get; set; }
        public Nullable<int> Level { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Certificate> Certificates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comp_InvCat_InvTemp> Comp_InvCat_InvTemp { get; set; }
        public virtual TaxAuthority TaxAuthority { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Decision_Company_Invoice> Decision_Company_Invoice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
