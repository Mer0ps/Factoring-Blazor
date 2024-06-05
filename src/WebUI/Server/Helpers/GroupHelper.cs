namespace Mx.Blazor.DApp.Server.Helpers;

public static class GroupHelper
{
    public static string GetGroupName(long id)
    {
        return $"account_{id}";
    }
}
