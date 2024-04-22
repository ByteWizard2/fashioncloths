using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class SalesChartData
    {
        [Key]
        public int Id { get; set; }
        public string[] Labels { get; set; }
        public int[] SalesData { get; set; }
        public decimal[] RevenueData { get; set; }
        public int[] ProductsData { get; set; }
    }
}
