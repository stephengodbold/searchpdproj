//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace searchpd
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int ProductID { get; set; }
        public int NodeID { get; set; }
        public int CategoryID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
