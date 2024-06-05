using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mx.Blazor.DApp.Server.Services;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var accountId = Context.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"account_{accountId}");
        await base.OnConnectedAsync();
    }
}
