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
    
    public partial class aspnet_Roles_Scheme_Relationships
    {
        public System.Guid ApplicationId { get; set; }
        public System.Guid Id { get; set; }
        public System.Guid RoleId { get; set; }
        public string SchemeCode { get; set; }
        public string RoleCode { get; set; }
        public string RolesName { get; set; }
        public Nullable<int> Type { get; set; }
    
        public virtual aspnet_Roles aspnet_Roles { get; set; }
        public virtual WorkflowScheme WorkflowScheme { get; set; }
    }
}
