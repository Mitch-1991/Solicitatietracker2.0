using SolicitatieTracker.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.DTOs
{
    public interface IDashboardService
    {
        Task<DashboardKPIDto> GetKPIAsync();
    }
}
