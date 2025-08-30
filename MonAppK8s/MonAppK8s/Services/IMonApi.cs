using MonAppK8s.Payloads;
using Refit;

namespace MonAppK8s.Services;

public interface IMonApi
{
    [Get("/get-config")]
    Task<MonApiPayload> GetItemsAsync();
}
