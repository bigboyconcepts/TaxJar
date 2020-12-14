using System.Collections.Generic;

namespace TaxJar.Models
{
    public class TaxJarSummaryRate
    {
        public string country_code { get; set; }
        public string country { get; set; }
        public string region_code { get; set; }
        public string region { get; set; }
        public dynamic minimum_rate { get; set; }
        public dynamic average_rate { get; set; }
    }

    public class TaxJarSummaryRatesContainer
    {
        public List<TaxJarSummaryRate> summary_rates { get; set; }
    }
}