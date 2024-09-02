using Microsoft.AspNetCore.SignalR;

namespace Hairhub.API.Hubs
{
    public sealed class BookAppointmentHub : Hub<IBookAppointmentHub>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.RecieveMessage($"{Context.ConnectionId} has joined");
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.RecieveMessage($"{Context.ConnectionId}: {message}");
        }
    }
}
