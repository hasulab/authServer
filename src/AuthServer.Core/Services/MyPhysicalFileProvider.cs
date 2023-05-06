using AuthServer.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AuthServer.Services;

class MyPhysicalFileProvider : IFileProvider, IDisposable
{
    readonly PhysicalFileProvider fileProvider;
    private readonly IHttpContextAccessor httpContextAccessor;

    public MyPhysicalFileProvider(string root, IHttpContextAccessor httpContextAccessor)
    {
        fileProvider = new PhysicalFileProvider(root);
        this.httpContextAccessor = httpContextAccessor;
    }
    public void Dispose()
    {
        fileProvider?.Dispose();
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return fileProvider.GetDirectoryContents(subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var requestContext = httpContextAccessor?.HttpContext?.GetRequestContext()!;
        if (requestContext?.HasTenantId == true)
        {

        }
        return fileProvider.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return fileProvider.Watch(filter);
    }
}