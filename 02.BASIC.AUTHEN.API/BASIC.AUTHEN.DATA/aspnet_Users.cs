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
    
    public partial class aspnet_Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public aspnet_Users()
        {
            this.aspnet_PersonalizationPerUser = new HashSet<aspnet_PersonalizationPerUser>();
            this.aspnet_UsersInRoles = new HashSet<aspnet_UsersInRoles>();
            this.bsd_System_Tracking = new HashSet<bsd_System_Tracking>();
            this.aspnet_UsersInOrganization = new HashSet<aspnet_UsersInOrganization>();
            this.Customers = new HashSet<Customer>();
            this.FileExtensions = new HashSet<FileExtension>();
            this.FileExtensions1 = new HashSet<FileExtension>();
            this.FileThumbnails = new HashSet<FileThumbnail>();
            this.FileThumbnails1 = new HashSet<FileThumbnail>();
            this.Logs = new HashSet<Log>();
            this.idm_RightsOfUser = new HashSet<idm_RightsOfUser>();
            this.Workflow_Document = new HashSet<Workflow_Document>();
            this.Workflow_Document1 = new HashSet<Workflow_Document>();
            this.Workflow_DocumentTransitionHistory = new HashSet<Workflow_DocumentTransitionHistory>();
        }
    
        public System.Guid ApplicationId { get; set; }
        public System.Guid UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public System.DateTime LastActivityDate { get; set; }
        public Nullable<System.Guid> CmsSiteSiteId { get; set; }
    
        public virtual aspnet_Membership aspnet_Membership { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual aspnet_Profile aspnet_Profile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<aspnet_UsersInRoles> aspnet_UsersInRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bsd_System_Tracking> bsd_System_Tracking { get; set; }
        public virtual aspnet_Users aspnet_Users1 { get; set; }
        public virtual aspnet_Users aspnet_Users2 { get; set; }
        public virtual cms_Site cms_Site { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<aspnet_UsersInOrganization> aspnet_UsersInOrganization { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileExtension> FileExtensions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileExtension> FileExtensions1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileThumbnail> FileThumbnails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileThumbnail> FileThumbnails1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<idm_RightsOfUser> idm_RightsOfUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow_Document> Workflow_Document { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow_Document> Workflow_Document1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow_DocumentTransitionHistory> Workflow_DocumentTransitionHistory { get; set; }
    }
}
