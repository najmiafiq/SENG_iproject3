using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEMMU.API.Models;
using TEMMU.Core.Entities;
using TEMMU.Core.Interfaces;


namespace TEMMU.API.Controllers
{
    [Route("api/fighters")]
    [ApiController]
    [Authorize]
    public class FightersController : ControllerBase
    {
        private readonly IFighterRepository _repository;
        private readonly IMapper _mapper;

        public FightersController(IFighterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // --- R: READ (GET) ---
        // GET api/fighters
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FighterCharacterReadDTO>>> GetAll()
        {
            var fighters = await _repository.getAllFightersAsync();
            // Map domain entities to Read DTOs before returning
            return Ok(_mapper.Map<IEnumerable<FighterCharacterReadDTO>>(fighters));
        }

        // GET api/fighters/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FighterCharacterReadDTO>> GetById(int id)
        {
            var fighter = await _repository.getFighterByIdAsync(id);
            if (fighter == null)
                return NotFound();

            return Ok(_mapper.Map<FighterCharacterReadDTO>(fighter));
        }

        // --- C: CREATE (POST) ---
        // POST api/fighters
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FighterCharacterReadDTO>> Create(FighterCharacterCreationDTO dto)
        {
            // Model validation checks [Required], [StringLength], etc., on the DTO.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newFighter = _mapper.Map<FighterCharacter>(dto);
            await _repository.addFighterAsync(newFighter);
            await _repository.saveChangesAsync();

            var readDto = _mapper.Map<FighterCharacterReadDTO>(newFighter);

            // Use CreatedAtAction for REST compliance (HTTP 201)
            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        // --- U: UPDATE (PUT) ---
        // PUT api/fighters/{id}
        // This expects the *full* resource to be sent, overwriting the existing one.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, FighterCharacterCreationDTO dto)
        {
            // 1. Basic Validation (ID match)
            // Although the DTO doesn't have an ID, we check the route ID.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. Check if the entity exists
            var existingFighter = await _repository.getFighterByIdAsync(id);
            if (existingFighter == null)
            {
                return NotFound();
            }

            // 3. Map DTO data onto the existing entity
            // AutoMapper should be configured to only map the editable properties from the DTO.
            // For simplicity here, we manually update the properties that come from the DTO:
            _mapper.Map(dto, existingFighter);

            // Ensure the ID remains correct (essential if you map using AutoMapper)
            existingFighter.Id = id;

            // 4. Update and Save
            // The repository update logic will mark the entity as modified in EF Core.
            await _repository.updateFighterAsync(existingFighter);
            await _repository.saveChangesAsync();

            // Return 204 No Content, which is standard for a successful PUT update
            return NoContent();
        }

        // --- D: DELETE (DELETE) ---
        // DELETE api/fighters/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingFighter = await _repository.getFighterByIdAsync(id);

            if (existingFighter == null)
            {
                return NotFound();
            }

            await _repository.deleteFighterAsync(id);
            await _repository.saveChangesAsync();

            // Return 204 No Content, which is standard for a successful DELETE
            return NoContent();
        }
    }
}
