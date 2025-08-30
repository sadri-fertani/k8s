namespace MonAppK8s.Services;

public interface IRedisService
{
    public Task SetValueAsync(string key, string value);

    public Task<string?> GetValueAsync(string key);
}
