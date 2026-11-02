using CrudOperations;

namespace Shared.Kernel.Interfaces;

public interface ISmsService
{
    Task<Response> SendAsync(string mobileNo, string message, string templateId);
}
