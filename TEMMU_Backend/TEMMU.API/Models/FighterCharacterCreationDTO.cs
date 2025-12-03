using System.ComponentModel.DataAnnotations;

namespace TEMMU.API.Models
{
    public class FighterCharacterCreationDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Style is required.")]
        public string style { get; set; }

        [Range(500, 1500, ErrorMessage = "Base Health must be between 500 and 1500.")]
        public int healthBase { get; set; } = 1000;
    }

    public class FighterCharacterReadDTO : FighterCharacterCreationDTO
    {
        public int  Id { get; set; }
        public double winRate { get; set; }
    }
}
