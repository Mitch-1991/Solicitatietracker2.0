using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.DTOs
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public DateOnly? AppliedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? NextStep { get; set; }
    }
}
