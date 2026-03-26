using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.DTOs
{
    public class DashboardKPIDto
    {
        public int LopendeSollicitaties { get; set; }
        public int GesprekkenGepland { get; set; }
        public int Afgewezen {  get; set; }
        public int Aanbiedingen { get; set; }
    }
}
