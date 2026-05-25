namespace CRM.API.Helpers;

public static class DeviceIdHeaderHelper
{
    public const string HeaderName = "X-Device-Id";

    public static string? GetDeviceId(HttpRequest request) =>
        request.Headers[HeaderName].FirstOrDefault();
}
