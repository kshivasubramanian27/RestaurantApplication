using Moq;
using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.Services;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Tests.PermissionTests
{
    [TestFixture]
    public class PermissionServiceTests
    {
        private Mock<IPermissionRepository> _repositoryMock = null!;
        private PermissionService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IPermissionRepository>();
            _service = new PermissionService(_repositoryMock.Object);
        }

        [Test]
        public async Task GetPermissionByNameAsync_WhenPermissionExists_ReturnsTrue()
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "User.View",
                Description = "View users"
            };

            var repositoryResult = ServiceResult<Permission>.SuccessResult(permission);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync("User.View")).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByNameAsync("User.View");

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo("User.View"));
            Assert.That(result.Data.Description, Is.EqualTo("View users"));
        }

        [Test]
        public async Task GetPermissionByNameAsync_WhenPermissionDoesNotExist_ReturnsFalse()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync("User.View")).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByNameAsync("User.View");

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.NotFound));
            Assert.That(result.Error, Is.EqualTo("Permission doesn't exist"));
        }

        [Test]
        public async Task UpdatePermissionAsync_WhenUpdatePermissionSucceeds_ReturnsUpdatedPermission()
        {
            Permission permission = new()
            {
                Id = Guid.NewGuid(),
                Name = "Permission.View",
                Description = "View Permission"
            };

            var repositoryResult = ServiceResult<Permission>.SuccessResult(permission);

            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(repositoryResult);

            EditUserPermissionDTO oldPermission = new()
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = "Test Description"
            };

            var result = await _service.UpdatePermissionAsync(oldPermission.Id, oldPermission);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Description, Is.EqualTo("View Permission"));
        }

        [Test]
        public async Task UpdatePermissionAsync_WhenUpdatePermissionFails_ReturnsNoData()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Failed to update permission", ServiceResultStatus.Error);

            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(repositoryResult);

            EditUserPermissionDTO permission = new()
            {
                Id = Guid.NewGuid(),
                Name = "Permission.View",
                Description = "View Permission"
            };

            var result = await _service.UpdatePermissionAsync(permission.Id, permission);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("Failed to update permission"));
        }

        [Test]
        public async Task DeletePermissionAsync_WhenDeletePermissionSucceeds_ReturnsTrue()
        {
            var repositoryResult = ServiceResult<bool>.SuccessResult(true);

            var permissionIdToBeDeleted = Guid.NewGuid();

            _repositoryMock.Setup(r => r.DeletePermissionAsync(permissionIdToBeDeleted)).ReturnsAsync(repositoryResult);

            var result = await _service.DeletePermissionAsync(permissionIdToBeDeleted);

            Assert.That(result.Data, Is.True);
        }

        [Test]
        public async Task DeletePermissionAsync_WhenDeletePermissionFails_ReturnsFalse()
        {
            var repositoryResult = ServiceResult<bool>.FailureResult("Failed to delete permission", ServiceResultStatus.Error);

            var permissionIdToBeDeleted = Guid.NewGuid();

            _repositoryMock.Setup(r => r.DeletePermissionAsync(permissionIdToBeDeleted)).ReturnsAsync(repositoryResult);

            var result = await _service.DeletePermissionAsync(permissionIdToBeDeleted);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.EqualTo("Failed to delete permission"));
        }

        [Test]
        public async Task CreatePermissionAsync_WhenPermissionDoesNotExist_CreatesPermission()
        {
            var createPermissionDTO = new CreateUserPermissionDTO
            {
                Name = "Permission.View",
                Description = "View Permission"
            };

            var notFoundResult = ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

            var createdPermission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = createPermissionDTO.Name,
                Description = createPermissionDTO.Description
            };

            var createResult = ServiceResult<Permission>.SuccessResult(createdPermission);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(createPermissionDTO.Name)).ReturnsAsync(notFoundResult);

            _repositoryMock.Setup(r => r.CreatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(createResult);

            var result = await _service.CreatePermissionAsync(createPermissionDTO);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo("Permission.View"));
            Assert.That(result.Data.Description, Is.EqualTo("View Permission"));
        }

        [Test]
        public async Task CreatePermissionAsync_WhenPermissionAlreadyExists_ReturnsConflict()
        {
            var existingPermission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Permission.View",
                Description = "View Permission"
            };

            var existingResult = ServiceResult<Permission>.SuccessResult(existingPermission);

            var createPermissionDTO = new CreateUserPermissionDTO
            {
                Name = "Permission.View",
                Description = "View Permission"
            };

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(createPermissionDTO.Name)).ReturnsAsync(existingResult);

            var result = await _service.CreatePermissionAsync(createPermissionDTO);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Conflict));
            Assert.That(result.Error, Is.EqualTo("Permission Already Exists"));
        }

        [Test]
        public async Task CreatePermissionAsync_WhenCheckingExistingPermissionFails_ReturnsError()
        {
            var createPermissionDTO = new CreateUserPermissionDTO
            {
                Name = "Permission.View",
                Description = "View Permission"
            };

            var repositoryResult = ServiceResult<Permission>.FailureResult("Failed to check whether the permission exists.", ServiceResultStatus.Error);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(createPermissionDTO.Name)).ReturnsAsync(repositoryResult);

            var result = await _service.CreatePermissionAsync(createPermissionDTO);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("Failed to check whether the permission exists."));

        }
    }
}