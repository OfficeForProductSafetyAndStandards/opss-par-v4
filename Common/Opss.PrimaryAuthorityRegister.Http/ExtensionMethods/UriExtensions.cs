namespace Opss.PrimaryAuthorityRegister.Http.ExtensionMethods;

public static class UriExtensions
{
    public static Uri AppendPath(this Uri uri, string path)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(path);

        var uriString = uri.ToString().TrimEnd('/');
        path = path.TrimStart('/');

        return new Uri($"{uriString}/{path}");
    }
}
