namespace RestaurantApplicationAPI.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = string.Empty;

        public Guid PermissionId { get; set; }

        public ApplicationRole Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}