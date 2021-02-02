using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace InstantTutors.Services.Student
{
    public interface IStudentService
    {
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId);
        Task<List<StudentViewModel>> SearchStudentsAsync(SearchViewModel model, bool ShowAll = false);
    }
    public class StudentService: IStudentService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<StudentViewModel>> SearchStudentsAsync(SearchViewModel model, bool ShowAll = false)
        {
            List<StudentViewModel> _studentsList = new List<StudentViewModel>();
            try
            {
                model.SearchText = model.SearchText.ToLower();
                var _role = await _dbContext.Roles.Where(x => x.Name == "Student").FirstOrDefaultAsync();
                var _students = new List<ApplicationUser>();

                if (ShowAll)
                {
                    _students = await _dbContext.Users
                        .Where(x => x.Roles.Select(y => y.RoleId).Contains(_role.Id))
                        .ToListAsync();
                }
                else
                {
                    _students = await _dbContext.Users
                        .Where(x => x.Roles.Select(y => y.RoleId).Contains(_role.Id))
                        .Where(z => (z.FirstName == null ? false : z.FirstName.ToLower().Contains(model.SearchText))
                            || (z.LastName == null ? false : z.LastName.ToLower().Contains(model.SearchText)))
                        .ToListAsync();
                }                   

                foreach (var student in _students)
                {
                    _studentsList.Add(new StudentViewModel()
                    {
                        UserId = student.Id,
                        User = student,
                        SessionsList = await GetSessionByUserIdAsync(student.Id)
                    });
                }
                return _studentsList;
            }
            catch (Exception ex)
            {
                //throw error
            }
            return _studentsList;
        }
    }
}