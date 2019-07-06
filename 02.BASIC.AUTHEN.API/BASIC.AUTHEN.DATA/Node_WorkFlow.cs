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
    
    public partial class Node_WorkFlow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Node_WorkFlow()
        {
            this.Node_WorkFlowExt = new HashSet<Node_WorkFlowExt>();
        }
    
        public System.Guid NodeWorkFlowID { get; set; }
        public Nullable<System.Guid> NodeID { get; set; }
        public string StartByUserName { get; set; }
        public System.DateTime StartDate { get; set; }
        public string Description { get; set; }
        public string StateName { get; set; }
        public string Type { get; set; }
        public System.Guid CreatedByUserID { get; set; }
        public System.DateTime CreatedOnDate { get; set; }
        public System.Guid LastModifiedByUserID { get; set; }
        public System.DateTime LastModifiedOnDate { get; set; }
        public System.Guid ApplicationID { get; set; }
        public string ApprovalResult { get; set; }
        public Nullable<bool> IsApproveByTaper { get; set; }
        public Nullable<bool> IsApproveByManager { get; set; }
    
        public virtual Node Node { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Node_WorkFlowExt> Node_WorkFlowExt { get; set; }
    }
}
