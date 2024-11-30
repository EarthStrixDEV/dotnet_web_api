using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace dotnet_web_api.Model
{
    public class AuthLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime timestamp { get; set; } = DateTime.Now;
        public string? userId { get; set; }
        public string? action { get; set; }
        public string? description { get; set; }
        public string? ip_address { get; set; }
        public string? userRole { get; set; }
    }
}
