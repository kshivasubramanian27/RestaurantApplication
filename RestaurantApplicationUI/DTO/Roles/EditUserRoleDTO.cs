namespace RestaurantApplicationUI.DTO.Roles
{
    public class EditUserRoleDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NormalisedName { get; set; }

        public IDictionary<Guid, string> Permissions { get; set; } = new Dictionary<Guid, string>();
    }
}