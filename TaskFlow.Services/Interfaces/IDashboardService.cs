using TaskFlow.Services.Models;

namespace TaskFlow.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(string userId);
}