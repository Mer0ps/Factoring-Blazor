namespace Mx.Blazor.DApp.Server.Services.Interfaces;

public interface IBotService
{
    Task Execute();
    Task GetEuriborRate();
    Task CalculateScore();
}
