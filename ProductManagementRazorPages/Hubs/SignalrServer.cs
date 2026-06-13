using Microsoft.AspNetCore.SignalR;

namespace ProductManagementRazorPages.Hubs;

public class SignalrServer : Hub
{
    public async Task SendMessage()
    {
        await Clients.All.SendAsync("LoadAllItems");
    }
}
