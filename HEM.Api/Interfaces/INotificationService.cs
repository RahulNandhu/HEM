namespace HEM.Api.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToAllAsync(string message);
    }
}
