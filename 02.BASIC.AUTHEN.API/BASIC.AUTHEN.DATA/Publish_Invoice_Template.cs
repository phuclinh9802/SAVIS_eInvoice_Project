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
    
    public partial class Publish_Invoice_Template
    {
        public System.Guid PublishInvoiceTemplateId { get; set; }
        public Nullable<System.Guid> PublishAnnouncementId { get; set; }
        public Nullable<System.Guid> CompanyId { get; set; }
        public Nullable<System.Guid> InvoiceCategoryId { get; set; }
        public Nullable<System.Guid> InvoiceTemplateId { get; set; }
        public string InvoiceSeries { get; set; }
        public Nullable<int> TotalNumber { get; set; }
        public string FromNumber { get; set; }
        public string ToNumber { get; set; }
        public string CurrentNumber { get; set; }
        public Nullable<int> RestNumber { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.Guid> LastModifiedByUserId { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
    
        public virtual Publish_Announcement Publish_Announcement { get; set; }
    }
}
