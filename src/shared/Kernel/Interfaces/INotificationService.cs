namespace Kernel;

public interface INotificationService
{
    Task SendAsync(object notification);
}
