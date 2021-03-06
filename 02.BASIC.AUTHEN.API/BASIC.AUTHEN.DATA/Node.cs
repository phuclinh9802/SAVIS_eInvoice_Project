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
    
    public partial class Node
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Node()
        {
            this.Folders = new HashSet<Folder>();
            this.Node_access1 = new HashSet<Node_access>();
            this.Node_files1 = new HashSet<Node_files>();
            this.Node_metadata1 = new HashSet<Node_metadata>();
            this.Node_ref1 = new HashSet<Node_ref>();
            this.Node_ref2 = new HashSet<Node_ref>();
            this.Node_tags1 = new HashSet<Node_tags>();
            this.Node_WorkFlow = new HashSet<Node_WorkFlow>();
            this.Node_WorkFlow_TransitionHistory = new HashSet<Node_WorkFlow_TransitionHistory>();
            this.Nodes1 = new HashSet<Node>();
        }
    
        public System.Guid NodeID { get; set; }
        public Nullable<System.Guid> ParentNodeID { get; set; }
        public System.Guid NodeTypeID { get; set; }
        public string NodePath { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public int Comment { get; set; }
        public System.Guid CreatedByUserID { get; set; }
        public System.DateTime CreatedOnDate { get; set; }
        public System.Guid LastModifiedByUserID { get; set; }
        public System.DateTime LastModifiedOnDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.Guid> ModerateUserID { get; set; }
        public string Node_metadata { get; set; }
        public string Node_tags { get; set; }
        public string Node_ref { get; set; }
        public string Node_files { get; set; }
        public string Node_access { get; set; }
        public System.Guid ApplicationID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Folder> Folders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_access> Node_access1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_files> Node_files1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_metadata> Node_metadata1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_ref> Node_ref1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_ref> Node_ref2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_tags> Node_tags1 { get; set; }
        public virtual Node_type Node_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_WorkFlow> Node_WorkFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_WorkFlow_TransitionHistory> Node_WorkFlow_TransitionHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node> Nodes1 { get; set; }
        public virtual Node Node1 { get; set; }
    }
}
