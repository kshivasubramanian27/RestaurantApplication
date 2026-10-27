using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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

        #region GetPermissionByNameAsync

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
        public async Task GetpermissionByNameAsync_WhenPermissionErrorOccurs_ReturnsFalse()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Error Occured", ServiceResultStatus.Error);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync("User.View")).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByNameAsync("User.View");

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("Failed to get the permission data"));
        }

        [Test]
        public async Task GetPermissionByNameAsync_WhenRepositoryReturnsNullData_ReturnsConflict()
        {
            var repositoryResult = ServiceResult<Permission>.SuccessResult(null);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync("User.View")).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByNameAsync("User.View");

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Conflict));
            Assert.That(result.Error, Is.EqualTo("Permission doesn't exist"));
        }

        [Test]
        public async Task GetPermissionByNameAsync_WhenRepositoryReturnsConflict_ReturnsConflict()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Permission conflict", ServiceResultStatus.Conflict);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync("User.View")).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByNameAsync("User.View");

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Conflict));
            Assert.That(result.Error, Is.EqualTo("Permission doesn't exist"));
        }

        #endregion

        #region GetPermissionByIdAsync

        [Test]
        public async Task GetPermissionByIdAsync_WhenPermissionExists_ReturnsTrue()
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "User.View",
                Description = "View users"
            };

            var repositoryResult = ServiceResult<Permission>.SuccessResult(permission);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(permission.Id)).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByIdAsync(permission.Id.ToString());

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.EqualTo(permission.Id));
            Assert.That(result.Data.Description, Is.EqualTo("View users"));
        }

        [Test]
        public async Task GetPermissionByIdAsync_WhenPermissionDoesNotExist_ReturnsFalse()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

            var Id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(Id)).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByIdAsync(Id.ToString());

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.NotFound));
            Assert.That(result.Error, Is.EqualTo("Permission doesn't exist with this ID"));
        }

        [Test]
        public async Task GetpermissionByIdAsync_WhenPermissionErrorOccurs_ReturnsFalse()
        {
            var repositoryResult = ServiceResult<Permission>.FailureResult("Error Occured", ServiceResultStatus.Error);

            var Id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(Id)).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByIdAsync(Id.ToString());

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("Failed to get the permission data"));
        }

        [TestCase("")]
        [TestCase("invalid-guid")]
        public async Task GetPermissionByIdAsync_WhenIdIsInvalid_ReturnsError(string Id)
        {
            var result = await _service.GetPermissionByIdAsync(Id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("Invalid permission ID"));
        }

        [Test]
        public async Task GetPermissionByIdAsync_WhenRepositoryReturnsNullData_ReturnsConflict()
        {
            var id = Guid.NewGuid();

            var repositoryResult = ServiceResult<Permission>.SuccessResult(null);

            _repositoryMock.Setup(r => r.GetPermissionDetailsAsync(id)).ReturnsAsync(repositoryResult);

            var result = await _service.GetPermissionByIdAsync(id.ToString());

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Conflict));
            Assert.That(result.Error, Is.EqualTo("Permission doesn't exist with this ID"));
        }

        #endregion

        #region UpdatePermissionAsync

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
        public async Task UpdatePermissionAsync_WhenPermissionDoesNotExist_ReturnsNotFound()
        {
            var editedUserDetails = new EditUserPermissionDTO
            {
                Id = Guid.NewGuid(),
                Name = "User.View",
                Description = "View User"
            };

            var repositoryResult = ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(repositoryResult);

            var result = await _service.UpdatePermissionAsync(editedUserDetails.Id, editedUserDetails);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.NotFound));
            Assert.That(result.Error, Is.EqualTo("Permission not found"));
        }

        [Test]
        public async Task UpdatePermissionAsync_PassesCorrectPermissionToRepository()
        {
            var id = Guid.NewGuid();

            var editedUserDetails = new EditUserPermissionDTO
            {
                Id = id,
                Name = "User.Edit",
                Description = "Edit users"
            };

            var repositoryResult = ServiceResult<Permission>.SuccessResult(new Permission
                {
                    Id = id,
                    Name = editedUserDetails.Name,
                    Description = editedUserDetails.Description
                });

            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(repositoryResult);

            await _service.UpdatePermissionAsync(id, editedUserDetails);

            _repositoryMock.Verify(r => r.UpdatePermissionAsync(
                    It.Is<Permission>(p =>
                        p.Id == id &&
                        p.Name == "User.Edit" &&
                        p.Description == "Edit users")), Times.Once);
        }

        #endregion

        #region DeletePermissionAsync

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
        public async Task DeletePermissionAsync_WhenPermissionDoesNotExist_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            var repositoryResult = ServiceResult<bool>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

            _repositoryMock.Setup(r => r.DeletePermissionAsync(id)).ReturnsAsync(repositoryResult);

            var result = await _service.DeletePermissionAsync(id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.NotFound));
            Assert.That(result.Error, Is.EqualTo("Failed to delete permission"));
        }

        [Test]
        public async Task DeletePermissionAsync_CallsRepositoryOnceWithCorrectId()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.DeletePermissionAsync(id)).ReturnsAsync(ServiceResult<bool>.SuccessResult(true));

            await _service.DeletePermissionAsync(id);

            _repositoryMock.Verify(r => r.DeletePermissionAsync(id), Times.Once);
        }

        #endregion

        #region CreatePermissionAsync

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

        #endregion

        #region GetAllPermissionsAsync

        [Test]
        public async Task GetAllPermissionsAsync_WhenPermissionsExist_ReturnsMappedPermissions()
        {
            var permissions = new List<Permission>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "User.View",
                    Description = "View users"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "User.Edit",
                    Description = "Edit users"
                }
            };

            var repositoryResult = ServiceResult<IList<Permission>>.SuccessResult(permissions);

            _repositoryMock.Setup(r => r.GetAllPermissionsAsync()).ReturnsAsync(repositoryResult);

            var result = await _service.GetAllPermissionsAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Has.Count.EqualTo(2));

            Assert.That(result.Data![0].Id, Is.EqualTo(permissions[0].Id));
            Assert.That(result.Data[0].Name, Is.EqualTo("User.View"));
            Assert.That(result.Data[0].Description, Is.EqualTo("View users"));

            Assert.That(result.Data[1].Id, Is.EqualTo(permissions[1].Id));
            Assert.That(result.Data[1].Name, Is.EqualTo("User.Edit"));
            Assert.That(result.Data[1].Description, Is.EqualTo("Edit users"));
        }

        [Test]
        public async Task GetAllPermissionsAsync_WhenRepositoryFails_ReturnsFailure()
        {
            var repositoryResult = ServiceResult<IList<Permission>>.FailureResult("DB error", ServiceResultStatus.Error);

            _repositoryMock.Setup(r => r.GetAllPermissionsAsync()).ReturnsAsync(repositoryResult);

            var result = await _service.GetAllPermissionsAsync();

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(ServiceResultStatus.Error));
            Assert.That(result.Error, Is.EqualTo("DB error"));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task GetAllPermissionsAsync_WhenNoPermissionsExist_ReturnsEmptyList()
        {
            IList<Permission> permissions = new List<Permission>();

            var repositoryResult = ServiceResult<IList<Permission>>.SuccessResult(permissions);

            _repositoryMock.Setup(r => r.GetAllPermissionsAsync()).ReturnsAsync(repositoryResult);

            var result = await _service.GetAllPermissionsAsync();

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Empty);
        }

        #endregion

    }
}