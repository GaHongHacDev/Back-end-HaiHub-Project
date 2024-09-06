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

        public async Task SendMessage(string message,string dateappointment, List<string> serviceIds, string salonId)
        {
            var timestamp = DateTime.Now;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}",dateappointment, timestamp, salonId, serviceIds);
        }
    }
}
