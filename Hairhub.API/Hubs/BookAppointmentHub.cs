using Microsoft.AspNetCore.SignalR;

namespace Hairhub.API.Hubs
{
    public sealed class BookAppointmentHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ConnectSignalR", $"{Context.ConnectionId} has connected SignalR. Connect SigalR successfully");
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("BookAppointmentMessage", message);
        }

    }
}
