namespace ChatApp.Web.Interfaces.Services;

public interface IVersionService
{
    Task<string?> Get();
}
