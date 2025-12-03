using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using TEMMU.API.Models;
using TEMMU.Core.Entities;
using TEMMU.Infrastructure.Data;

public static class TestMocks
{
    // Mock for IMapper
    public static Mock<IMapper> GetMapperMock()
    {
        var mockMapper = new Mock<IMapper>();

        // Setup generic mapping behavior for DTOs
        mockMapper.Setup(m => m.Map<FighterCharacter>(It.IsAny<FighterCharacterCreationDTO>()))
                  .Returns((FighterCharacterCreationDTO src) => new FighterCharacter { name = src.name, style = src.style, healthBase = src.healthBase });

        mockMapper.Setup(m => m.Map<FighterCharacterReadDTO>(It.IsAny<FighterCharacter>()))
                  .Returns((FighterCharacter src) => new FighterCharacterReadDTO { Id = src.Id, name = src.name });

        // Setup for mapping to existing instance (PUT)
        mockMapper.Setup(m => m.Map(It.IsAny<FighterCharacterCreationDTO>(), It.IsAny<FighterCharacter>()))
                  .Callback((FighterCharacterCreationDTO src, FighterCharacter dest) =>
                  {
                      dest.name = src.name;
                      dest.style = src.style;
                  });

        return mockMapper;
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
        userManager.Setup(u => u.CheckPasswordAsync(It.Is<ApplicationUser>(u => u.Email == "test@example.com"), "Password123"))
                   .ReturnsAsync(true);

        // Setup failed login (Find User)
        userManager.Setup(u => u.FindByEmailAsync("missing@user.com")).ReturnsAsync((ApplicationUser)null);


        return userManager;
    }
}