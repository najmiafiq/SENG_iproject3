// GameAPI.Tests/FightersControllerTests.cs

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TEMMU.API.Controllers;
using TEMMU.API.Models;
using TEMMU.Core.Entities;
using TEMMU.Core.Interfaces;

[TestFixture]
public class FightersControllerTests
{
    private Mock<IFighterRepository> _mockRepository;
    private Mock<IMapper> _mockMapper;
    private FightersController _controller;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IFighterRepository>();
        IMapper realMapper = TestMocks.GetRealMapper(); // Use the mock mapper helper

        // Setup initial data for read/delete tests
        _mockRepository.Setup(r => r.getAllFightersAsync()).ReturnsAsync(new List<FighterCharacter>
        {
            new FighterCharacter { Id = 1, name = "Ryu", style = "Karate" },
            new FighterCharacter { Id = 2, name = "Ken", style = "Karate" }
        });
        _mockRepository.Setup(r => r.getFighterByIdAsync(1)).ReturnsAsync(new FighterCharacter { Id = 1, name = "Ryu", style = "Karate" });
        _mockRepository.Setup(r => r.getFighterByIdAsync(3)).ReturnsAsync((FighterCharacter)null); // Non-existent ID

        _controller = new FightersController(_mockRepository.Object, realMapper);
    }

    // --- R: READ (GET) Tests ---

    [Test]
    public async Task GetAll_ReturnsOkWithData()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var data = (result.Result as OkObjectResult).Value as IEnumerable<FighterCharacterReadDTO>;
        Assert.That(data.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var fighter = (result.Result as OkObjectResult).Value as FighterCharacterReadDTO;
        Assert.That(fighter.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetById(3);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    // --- C: CREATE (POST) Tests ---

    [Test]
    public async Task Create_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var model = new FighterCharacterCreationDTO { name = "NewFighter", style = "Boxing", healthBase = 1000 };
        _mockRepository.Setup(r => r.addFighterAsync(It.IsAny<FighterCharacter>()))
                       .Callback<FighterCharacter>(c => c.Id = 4); // Simulate ID assignment

        // Act
        var result = await _controller.Create(model);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var created = (result.Result as CreatedAtActionResult).Value as FighterCharacterReadDTO;
        Assert.That(created.Id, Is.EqualTo(4));
        _mockRepository.Verify(r => r.saveChangesAsync(), Times.Once);
    }

    // --- U: UPDATE (PUT) Tests ---

    [Test]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var model = new FighterCharacterCreationDTO { name = "UpdatedRyu", style = "Karate", healthBase = 1050 };

        // Act
        var result = await _controller.Update(1, model);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockRepository.Verify(r => r.saveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var model = new FighterCharacterCreationDTO { name = "Ghost", style = "Jutsu" };

        // Act
        var result = await _controller.Update(3, model); // ID 3 non-existent

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        _mockRepository.Verify(r => r.saveChangesAsync(), Times.Never);
    }

    // --- D: DELETE (DELETE) Tests ---

    [Test]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockRepository.Verify(r => r.deleteFighterAsync(1), Times.Once);
        _mockRepository.Verify(r => r.saveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Delete(3);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        _mockRepository.Verify(r => r.deleteFighterAsync(It.IsAny<int>()), Times.Never);
        _mockRepository.Verify(r => r.saveChangesAsync(), Times.Never);
    }
}