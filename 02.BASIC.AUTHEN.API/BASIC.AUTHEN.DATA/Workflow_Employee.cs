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
    
    public partial class Workflow_Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workflow_Employee()
        {
            this.Workflow_Roles = new HashSet<Workflow_Roles>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public System.Guid StructDivisionId { get; set; }
        public bool IsHead { get; set; }
    
        public virtual Workflow_StructDivision Workflow_StructDivision { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow_Roles> Workflow_Roles { get; set; }
    }
}
