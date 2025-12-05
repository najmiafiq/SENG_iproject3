// GameAPI.Tests/AuthControllerTests.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TEMMU.API.Controllers;
using TEMMU.API.Models;
using TEMMU.API.Services;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private Mock<ITokenService> _mockTokenService;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        // 1. Setup Mock User Manager (from TestMocks helper)
        _mockUserManager = TestMocks.GetMockUserManager();

        // 2. Mock Token Service
        _mockTokenService = new Mock<ITokenService>();
        _mockTokenService.Setup(t => t.CreateToken(It.IsAny<ApplicationUser>()))
                         .Returns("fake_jwt_token_12345");

        // 3. Initialize Controller with Mocks
        _controller = new AuthController(_mockUserManager.Object, _mockTokenService.Object);
    }

    // --- Registration Tests ---

    [Test]
    public async Task Register_ValidModel_ReturnsOk()
    {
        // Arrange
        var model = new RegistrationDTO { Email = "new@user.com", Password = "ValidPassword1!", Username = "NewUser" };

        // Act
        var result = await _controller.Register(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var response = (result as OkObjectResult).Value as AuthResponseDTO;
        Assert.That(response.isSuccess, Is.True);
    }

    // --- Login Tests ---

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var model = new LoginDTO { Email = "test@example.com", Password = "Password123!" };

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var response = (result.Result as OkObjectResult).Value as AuthResponseDTO;
        Assert.That(response.isSuccess, Is.True);
        Assert.That(response.token, Is.EqualTo("fake_jwt_token_12345"));

        // Verify that token creation was called
        _mockTokenService.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Once);
    }

    [Test]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginDTO { Email = "test@example.com", Password = "WrongPassword" };

        // Setup mock to fail password check
        _mockUserManager.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), "WrongPassword")).ReturnsAsync(false);

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginDTO { Email = "missing@user.com", Password = "Password123!" };

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }
}