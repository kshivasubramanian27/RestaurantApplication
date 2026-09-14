namespace RestaurantApplicationAPI.Utilities
{
    public static class RoleHierarchy
    {
        public static readonly Dictionary<string, int> RoleHierarchyDict = 
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Staff", 1 },
                { "Manager", 2 },
                { "Admin", 3 },
                { "Super Admin", 4 }
            };
    }
}