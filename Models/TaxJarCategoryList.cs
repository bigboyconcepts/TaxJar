using System.Collections.Generic;

namespace TaxJar.Models
{
    public class TaxJarCategoryList
    {
        public List<TaxJarCategory> categories { get; set; }
    }

    public class TaxJarCategory
    {
        public string name { get; set; }
        public string product_tax_code { get; set; }
        public string description { get; set; }
    }
}