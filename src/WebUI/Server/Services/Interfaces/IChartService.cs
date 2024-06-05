using Application.Invoices.Queries;

namespace Mx.Blazor.DApp.Server.Services.Interfaces;

public interface IChartService
{
    Task<IEnumerable<TotalFinancedDto>> GetTotalFinanced(long? accountId);
    Task<IEnumerable<TotalCollectedFeeDto>> GetTotalCollectedFee(long? accountId);
}
