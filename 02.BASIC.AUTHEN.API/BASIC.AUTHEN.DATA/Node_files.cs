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
    
    public partial class Node_files
    {
        public System.Guid ID { get; set; }
        public System.Guid NodeID { get; set; }
        public System.Guid FileID { get; set; }
        public string Title { get; set; }
        public string AbsolutePath { get; set; }
        public string RelativePath { get; set; }
        public string FileSrc { get; set; }
        public int Status { get; set; }
    
        public virtual File File { get; set; }
        public virtual Node Node { get; set; }
    }
}
