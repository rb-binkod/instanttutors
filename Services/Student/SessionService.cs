using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Services.Student
{
    public interface ISessionService
    {
        Task<List<Sessions>> GetSessionsAsync();
        Task<Sessions> GetSessionByIdAsync(int Id);
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId);
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId, int limit);
        Task<List<SelectListItem>> GetTutorsAsync();
        Task<List<SelectListItem>> GetStudentsAsync();
        Task<List<Sessions>> GetAllSessionsAsync();
    }
    public class SessionService : ISessionService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<List<Sessions>> GetSessionsAsync()
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .ToListAsync();
        }
        public async Task<Sessions> GetSessionByIdAsync(int Id)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<List<Sessions>> GetAllSessionsAsync()
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                //.Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId, int limit)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(limit).ToListAsync();
        }
        public async Task<List<SelectListItem>> GetTutorsAsync()
        {
            var _tRole = await _dbContext.Roles.Where(x => x.Name == "Tutor").FirstOrDefaultAsync();
            return await _dbContext.Users
                .Where(x => x.EmailConfirmed == true && x.Roles.Select(y => y.RoleId).Contains(_tRole.Id))
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.FirstName + " " + x.LastName + " (" + x.Gender + ") - " + x.Email
                }).ToListAsync();
        }
        public async Task<List<SelectListItem>> GetStudentsAsync()
        {
            var _tRole = await _dbContext.Roles.Where(x => x.Name == "Student").FirstOrDefaultAsync();
            return await _dbContext.Users
                .Where(x => x.EmailConfirmed == true && x.Roles.Select(y => y.RoleId).Contains(_tRole.Id))
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.FirstName + " " + x.LastName + " (" + x.Gender + ") - " + x.Email
                }).ToListAsync();
        }
    }
}