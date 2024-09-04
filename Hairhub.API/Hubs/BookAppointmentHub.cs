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

        // Thay đổi serviceId từ string sang List<string>
        public async Task SendMessage(string message, List<string> serviceIds)
        {
            var timestamp = DateTime.Now;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}", timestamp, serviceIds);
        }
    }
}
