using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? WebsiteURL { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
    }
}
