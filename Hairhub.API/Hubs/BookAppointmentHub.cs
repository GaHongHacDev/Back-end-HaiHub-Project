using Microsoft.AspNetCore.SignalR;

namespace Hairhub.API.Hubs
{
    public sealed class BookAppointmentHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var message = $"{Context.ConnectionId} has joined";
            var timestamp = DateTime.Now;
            await Clients.All.SendAsync("ReceiveMessage", message, timestamp);
        }

        public async Task SendMessage(string message)
        {
            var timestamp = DateTime.Now;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}", timestamp);
        }
    }
}
