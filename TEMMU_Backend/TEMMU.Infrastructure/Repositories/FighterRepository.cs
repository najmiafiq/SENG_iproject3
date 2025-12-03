using TEMMU.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TEMMU.Infrastructure.Repositories
{
    public class FighterRepository : IFighterRepository
    {
        private readonly Data.GameDBContext _context;

        public FighterRepository(Data.GameDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Core.Entities.FighterCharacter>> getAllFightersAsync()
        {
            return await _context.Fighters.ToListAsync();
        }
        public async Task<Core.Entities.FighterCharacter?> getFighterByIdAsync(int id)
        {
            return await _context.Fighters.FindAsync(id);
        }
        public async Task addFighterAsync(Core.Entities.FighterCharacter fighter)
        {
            await _context.Fighters.AddAsync(fighter);
        }
        public async Task updateFighterAsync(Core.Entities.FighterCharacter fighter)
        {
            _context.Fighters.Update(fighter);
        }
        public async Task deleteFighterAsync(int id)
        {
            var fighter = await getFighterByIdAsync(id);
            if (fighter != null)
            {
                _context.Fighters.Remove(fighter);
            }
        }
        public async Task<bool> saveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
