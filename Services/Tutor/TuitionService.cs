using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services.Tutor
{
    public interface ITuitionService
    {
        Task<(bool, TutorViewModel)> SetAvailabilityAsync(TutorViewModel _tutor);
        Task<List<TutorViewModel>> GetTutorAvailabilityAsync();
        Task<List<TutorAvailability>> GetTutorAvailabilityByTutor(int TutorId);
        Task<List<TuitionSubjects>> GetTuitionSubjectsByTutor(int TutorId);
        Task<List<Sessions>> GetSessionByTutorUserIdAsync(string UserId);
        Task<List<Sessions>> GetSessionByTutorUserIdAsync(string UserId, int limit);
    }
    public class TuitionService : ITuitionService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<(bool, TutorViewModel)> SetAvailabilityAsync(TutorViewModel _tutor)
        {
            var _tutorDB = await _dbContext.Tutors.FirstOrDefaultAsync(x => x.Id == _tutor.Id);
            if (_tutorDB == null) return (false, null);

            _tutorDB.NameOfSchool = _tutor.NameOfSchool;
            _tutorDB.Experience = _tutor.Experience;
            _tutorDB.PreviousSubjects = _tutor.PreviousSubjects;
            _tutorDB.Concerns = _tutor.Concerns;
            _tutorDB.GradeLevel = _tutor.GradeLevel;
            _tutorDB.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            var _tuitionSubjects = await _dbContext.TuitionSubjects.Where(x => x.TutorId == _tutorDB.Id).ToListAsync();
            _dbContext.TuitionSubjects.RemoveRange(_tuitionSubjects);
            await _dbContext.SaveChangesAsync();

            foreach (var sub in _tutor.TuitionSubjects)
            {
                if (sub.Selected)
                {
                    _dbContext.TuitionSubjects.Add(new TuitionSubjects()
                    {
                        Name = sub.Name,
                        Description = sub.Description,
                        TutorId = _tutorDB.Id,
                        CreatedDate = DateTime.Now
                    });
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (_tutor.TutorAvailability != null)
            {
                var _tutorAvailability = await _dbContext.TutorAvailability.Where(x => x.TutorId == _tutorDB.Id).ToListAsync();
                _dbContext.TutorAvailability.RemoveRange(_tutorAvailability);
                await _dbContext.SaveChangesAsync();

                foreach (var avail in _tutor.TutorAvailability)
                {
                    foreach (var time in avail.Timing)
                    {
                        if (time.Selected)
                        {
                            _dbContext.TutorAvailability.Add(new TutorAvailability()
                            {
                                Day = avail.Day,
                                Time = time.AvailabilityTime,
                                TutorId = _tutorDB.Id,
                                CreatedDate = DateTime.Now
                            });
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }

            return (true, _tutor);
        }

        public async Task<List<TutorViewModel>> GetTutorAvailabilityAsync()
        {
            return new List<TutorViewModel>(); //await _dbContext.Tutors.ToListAsync();
        }

        public async Task<List<TutorAvailability>> GetTutorAvailabilityByTutor(int TutorId)
        {
            return await _dbContext.TutorAvailability
                .Include(x => x.Tutor)
                .Where(x => x.TutorId == TutorId).ToListAsync();
        }
        
        public async Task<List<TuitionSubjects>> GetTuitionSubjectsByTutor(int TutorId)
        {
            return await _dbContext.TuitionSubjects
                .Include(x => x.Tutor)
                .Where(x => x.TutorId == TutorId).ToListAsync();
        }

        public async Task<List<Sessions>> GetSessionByTutorUserIdAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Include(y => y.User)
                .Where(x => x.TutorUserId == UserId)
                .OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
        public async Task<List<Sessions>> GetSessionByTutorUserIdAsync(string UserId, int limit)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Include(y => y.User)
                .Where(x => x.TutorUserId == UserId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(limit).ToListAsync();
        }
    }
}