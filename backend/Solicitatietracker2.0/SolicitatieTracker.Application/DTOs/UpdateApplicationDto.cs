using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.DTOs
{
    public class UpdateApplicationDto
    {
        public string Bedrijf { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? JobUrl { get; set; }
        public string? Location { get; set; }
        public Status Status { get; set; }
        public string? Priority { get; set; }
        public DateOnly AppliedDate { get; set; }
        public string? NextStep { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Omschrijving { get; set; }
    }
}