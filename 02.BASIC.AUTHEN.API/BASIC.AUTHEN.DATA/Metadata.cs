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
    
    public partial class Metadata
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Metadata()
        {
            this.Metadata_Nodetype = new HashSet<Metadata_Nodetype>();
            this.Node_metadata = new HashSet<Node_metadata>();
        }
    
        public System.Guid MetadataID { get; set; }
        public string MetadataName { get; set; }
        public string MetadataFormat { get; set; }
        public System.Guid CreatedByUserID { get; set; }
        public System.DateTime CreatedOnDate { get; set; }
        public System.Guid LastModifiedByUserID { get; set; }
        public System.DateTime LastModifiedOnDate { get; set; }
        public System.Guid ApplicationID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Metadata_Nodetype> Metadata_Nodetype { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_metadata> Node_metadata { get; set; }
    }
}
