using System.ComponentModel.DataAnnotations;

namespace TEMMU.API.Models
{
    public class AuthResponseDTO
    {
        public bool isSuccess { get; set; }
        public string? token { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
