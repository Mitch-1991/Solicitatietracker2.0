using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.DTOs
{
    public class UpcomingInterviewDto
    {
        public int Id { get; set; }
        public string Bedrijf { get; set; } = string.Empty;
        public string Functie { get; set; } = string.Empty;
        public string Datum { get; set; } = string.Empty;
        public string Tijd { get; set; } = string.Empty;
    }
}
