using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Security.Claims;
using TEMMU.API.Models;
using TEMMU.API.Profiles;
using TEMMU.Core.Entities;


public static class TestMocks
{
    
    // Mock for IMapper
    public static IMapper GetRealMapper()
    {
        ILoggerFactory loggerFactory = NullLoggerFactory.Instance;
        // Define the mapping configuration, including all profiles
        var config = new MapperConfiguration(cfg =>
        {
            // Add all your mapping profiles here
            cfg.AddProfile<FighterProfile>();
            
        }, loggerFactory);

        // Ensure configuration is valid (optional, but good practice)
        config.AssertConfigurationIsValid();

        return config.CreateMapper();
    }

    // Mock for UserManager (simplified for AuthController tests)
    public static Mock<UserManager<ApplicationUser>> GetMockUserManager()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        // Setup successful registration
        userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                   .ReturnsAsync(IdentityResult.Success);

        // Setup successful login (Find User)
        userManager.Setup(u => u.FindByEmailAsync("test@example.com"))
                   .ReturnsAsync(new ApplicationUser { UserName = "TestUser", Email = "test@example.com", Id = "user_id_1" });

        // Setup successful login (Check Password)
        userManager.Setup(u => u.CheckPasswordAsync(It.Is<ApplicationUser>(u => u.Email == "test@example.com"), "Password123!"))
                   .ReturnsAsync(true);

        // Setup failed login (Find User)
        userManager.Setup(u => u.FindByEmailAsync("missing@user.com")).ReturnsAsync((ApplicationUser)null);


        return userManager;
    }
}