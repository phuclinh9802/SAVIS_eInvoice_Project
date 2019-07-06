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
    
    public partial class Catalog_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Catalog_Master()
        {
            this.Catalog_Field = new HashSet<Catalog_Field>();
        }
    
        public System.Guid CatalogMasterId { get; set; }
        public System.Guid MetadataTemplateId { get; set; }
        public System.Guid MappedTermId { get; set; }
        public System.Guid MappedVocabularyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<System.Guid> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.Guid> LastModifiedByUserId { get; set; }
        public Nullable<System.DateTime> LastModifiedOnDate { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Catalog_Field> Catalog_Field { get; set; }
        public virtual Taxonomy_Vocabulary Taxonomy_Vocabulary { get; set; }
        public virtual Metadata_Template Metadata_Template { get; set; }
        public virtual Taxonomy_Term Taxonomy_Term { get; set; }
    }
}
