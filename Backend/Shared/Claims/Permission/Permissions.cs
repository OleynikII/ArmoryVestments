namespace Shared.Claims.Permission;

public static class Permissions
{
    public static class Users
    {
        public const string Get = "Permission.Users.Get";
        public const string Create = "Permission.Users.Create";
        public const string Update = "Permission.Users.Update";
        public const string Delete = "Permission.Users.Delete";
        
        public const string Export = "Permission.Users.Export";
        public const string Import = "Permission.Users.Import";
    }

    
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                     c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue != null) permissions.Add(propertyValue.ToString());
        }
        return permissions;
    }
}