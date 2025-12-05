using Microsoft.EntityFrameworkCore;
using TEMMU.Core.Entities;
using TEMMU.Core.Interfaces;


public class FighterRepository : IFighterRepository
    {
        private readonly GameDBContext _context;

        public FighterRepository(GameDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FighterCharacter>> getAllFightersAsync()
        {
            return await _context.Fighters.ToListAsync();
        }
        public async Task<FighterCharacter?> getFighterByIdAsync(int id)
        {
            return await _context.Fighters.FindAsync(id);
        }
        public async Task addFighterAsync(FighterCharacter fighter)
        {
            await _context.Fighters.AddAsync(fighter);
        }
        public async Task updateFighterAsync(FighterCharacter fighter)
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

