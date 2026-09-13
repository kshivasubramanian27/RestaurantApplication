namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IPermissionRepository
    {
        public Task<IEnumerable<string>> GetPermissionsForRolesAsync(IEnumerable<string> roles);
    }
}