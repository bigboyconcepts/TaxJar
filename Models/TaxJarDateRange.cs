using System;

namespace TaxJar.Models
{
    public class TaxJarDateRange
    {
        public DateTime? transaction_date { get; set; }
        public DateTime? from_transaction_date { get; set; }
        public DateTime? to_transaction_date { get; set; }
    }
}