using Dashboard_MilkStore.Models.Statistics;
using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Home
{
    public class HomeViewModel
    {
        public YearlyRevenueViewModel YearlyRevenue { get; set; }
        public int CurrentYear { get; set; }

        // Nuevas propiedades para las estadísticas del dashboard
        public DashboardStats DashboardStats { get; set; }
    }
}
