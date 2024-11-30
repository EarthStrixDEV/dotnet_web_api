using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore;
namespace dotnet_web_api.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string user_name { get; set; }
        public required string user_email { get; set; }
        public required string user_password { get; set; }
        public int user_role { get; set; }
    }
}
