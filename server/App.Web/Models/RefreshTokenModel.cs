using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App.Web.Models
{
    public class RefreshTokenModel
    {
        [JsonPropertyName("refreshToken")]
        [Required]
        public string RefreshToken { get; set; }
    }
}
