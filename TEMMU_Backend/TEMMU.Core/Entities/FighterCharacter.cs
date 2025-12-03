using System.ComponentModel.DataAnnotations;

namespace TEMMU.Core.Entities
{
    public class FighterCharacter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        [MaxLength(50)]
        public string style { get; set; }

        public int healthBase { get; set; }

        // Use double for more precision in stats
        public double attackMultiplier { get; set; }
        public double defenseMultiplier { get; set; }
        public int speed { get; set; }
        
        // Progression Data
        public int matchesPlayed { get; set; }
        public int wins { get; set; }
    }
}
