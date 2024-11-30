using dotnet_web_api.Database;
using dotnet_web_api.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace dotnet_web_api.Services
{
    public class LoggingService {
        private readonly ApplicationDbContext _context;

        public LoggingService(ApplicationDbContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        } 
        
        public async Task AddAuthLog(AuthLog authLog) {
            if (authLog is null) {
                throw new ArgumentNullException("AuthLog shouldn't null object.");
            }

            try
            {
                _context.AuthLogs?.Add(authLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AuthLog>> GetAuthLog() {
            var logData = await _context.AuthLogs.ToListAsync();
            return logData.Any() ? logData : throw new Exception("No logs found.");
        }

        public async Task<List<AuthLog>> GetAuthLogWithAction(string action) {
            var logData = await _context.AuthLogs.Where(al => al.action == action || al.action == null).ToListAsync();
            return logData.Any() ? logData : throw new Exception("No logs found.");
        }
    }
}
