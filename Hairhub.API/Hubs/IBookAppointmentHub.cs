namespace Hairhub.API.Hubs
{
    public interface IBookAppointmentHub
    {
        Task RecieveMessage(string message);
    }
}
