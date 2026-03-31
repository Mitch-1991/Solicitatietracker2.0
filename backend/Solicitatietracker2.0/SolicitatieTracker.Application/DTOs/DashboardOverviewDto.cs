using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.DTOs
{
    public class DashboardOverviewDto
    {
        public int Id { get; set; }
        public string Bedrijf {  get; set; } = string.Empty;
        public string Functie { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly AppliedDate { get; set; }
        public string? nextStep { get; set; }
    }
}
