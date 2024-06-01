//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Main
{
    using System;
    using System.Collections.Generic;
    
    public partial class Products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Products()
        {
            this.ProductsHistory = new HashSet<ProductsHistory>();
            this.ProductVATChange = new HashSet<ProductVATChange>();
        }
    
        public int Id { get; set; }
        public int IdType { get; set; }
        public string Name { get; set; }
        public string Measure { get; set; }
        public decimal PricePerUnit { get; set; }
        public int IdVAT { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductsHistory> ProductsHistory { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual ProductVAT ProductVAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductVATChange> ProductVATChange { get; set; }
       
    }
}
