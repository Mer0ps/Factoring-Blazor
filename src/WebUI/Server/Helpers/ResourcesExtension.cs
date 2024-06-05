using Mx.Blazor.DApp.Properties;
using System.Resources;

namespace Mx.Blazor.DApp.Helpers;

public static class ResourcesExtension
{
    public static byte[] GetResourceByName(string resourceName)
    {
        ResourceManager resourceManager = Resources.ResourceManager;
        object resource = resourceManager.GetObject(resourceName);

        if (resource is byte[])
        {
            return (byte[])resource;
        }

        return null;
    }
}
