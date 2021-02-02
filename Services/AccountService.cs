using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services
{
    public interface IAccountService
    {
        Task<Tutors> CreateTutorsAsync(Tutors _tutor);
        Task<List<Tutors>> GetTutorsAsync();
        Task<List<TuitionSubjectsViewModel>> AddTutorSubjectAsync(List<TuitionSubjectsViewModel> _subjects, int _tutorId);
    }
    public class AccountService: IAccountService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<Tutors> CreateTutorsAsync(Tutors _tutor)
        {
            _tutor.CreatedDate = DateTime.Now;
            _dbContext.Tutors.Add(_tutor);
            await _dbContext.SaveChangesAsync();
            return _tutor;
        }

        public async Task<List<Tutors>> GetTutorsAsync()
        {
            return await _dbContext.Tutors.ToListAsync();
        }

        public async Task<List<TuitionSubjectsViewModel>> AddTutorSubjectAsync(List<TuitionSubjectsViewModel> _subjects, int _tutorId)
        {
            foreach (var sub in _subjects)
            {
                if (sub.Selected)
                {
                    _dbContext.TuitionSubjects.Add(new TuitionSubjects()
                    {
                        Name = sub.Name,
                        Description = sub.Description,
                        TutorId = _tutorId,
                        CreatedDate = DateTime.Now
                    });
                    await _dbContext.SaveChangesAsync();
                }
            }
            return _subjects;
        }
    }
}