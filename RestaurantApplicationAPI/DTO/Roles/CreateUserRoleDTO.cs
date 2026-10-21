namespace RestaurantApplicationAPI.DTO.Roles
{
    public class CreateUserRoleDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string NormalisedName { get; set; } = string.Empty;

        public IList<string> Permissions { get; set; } = new List<string>();
    }
}