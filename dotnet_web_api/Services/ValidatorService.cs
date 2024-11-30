using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using dotnet_web_api.Database;
namespace dotnet_web_api.Services
{
    public class ValidatorService
    {
        private readonly ApplicationDbContext? _context;

        private readonly string EmailFormat = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        private readonly string StrongPasswordFormat = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$\r\n";

        public ValidatorService(ApplicationDbContext context) {
            _context = context;
        }

        public bool UserIsExist(int id) {
            return _context.Users.Any(u => u.Id == id);
        }

        public bool IsEmailFormat(string email) {
            return Regex.IsMatch(email, EmailFormat);
        }

        public bool IsPasswordStrong(string password) {
            return Regex.IsMatch(password, StrongPasswordFormat);
        }

        public bool IsBodyAuthToNull(string[] Body) {
            if (Body.Length == 0) {
                return true;
            } 
            foreach (var item in Body)
            {
                if (String.IsNullOrEmpty(item)) {
                    return true; 
                }
            }
            return false;
        }
    }
}
