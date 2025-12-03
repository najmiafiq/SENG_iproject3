namespace TEMMU.Core.Interfaces
{
    public interface IFighterRepository
    {
        Task<IEnumerable<Entities.FighterCharacter>> getAllFightersAsync();
        Task<Entities.FighterCharacter?> getFighterByIdAsync(int id);
        Task addFighterAsync(Entities.FighterCharacter fighter);
        Task updateFighterAsync(Entities.FighterCharacter fighter);
        Task deleteFighterAsync(int id);
        Task<bool> saveChangesAsync();
    }
}
