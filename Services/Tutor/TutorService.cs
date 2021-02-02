using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Models;
using InstantTutors.Models.Enums;
using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services.Tutor
{
    public interface ITutorService
    {
        Task<List<Tutors>> GetTutorsAsync();
        Task<Tutors> GetTutorByIdAsync(int Id);
        Task<Tutors> GetTutorByUserIdAsync(string UserId);
        Task<Tutors> UpdateTutorAsync(Tutors tutor);
        Task<TutorViewModel> GetTutorAvailabilityAsync(ApplicationUser _user);
        Task<List<TutorViewModel>> SearchTutorsAsync(SearchViewModel model);
        Task<List<TutorViewModel>> GetTutorsModelAsync(List<Tutors> _tutors);
        Task<(bool, Sessions)> ApproveDeclineSession(int SessionId, Sessions Session);
        Task<(bool, Sessions)> DeleteSession(int SessionId);
    }
    public class TutorService : ITutorService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<List<Tutors>> GetTutorsAsync()
        {
            return await _dbContext.Tutors
                .Include(x => x.User)
                .Include(x => x.TuitionSubjects)
                .Include(x => x.TutorAvailability)
                .ToListAsync();
        }
        public async Task<Tutors> GetTutorByIdAsync(int Id)
        {
            return await _dbContext.Tutors
                .Include(x => x.User)
                .Include(x => x.TuitionSubjects)
                .Include(x => x.TutorAvailability)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<Tutors> GetTutorByUserIdAsync(string UserId)
        {
            return await _dbContext.Tutors
                .Include(x => x.User)
                .Include(x => x.TuitionSubjects)
                .Include(x => x.TutorAvailability)
                .Where(x => x.UserId == UserId).FirstOrDefaultAsync();
        }
        public async Task<Tutors> UpdateTutorAsync(Tutors tutor)
        {
            var _tutor = await _dbContext.Tutors.Where(x => x.Id == tutor.Id).FirstOrDefaultAsync();
            _tutor.Experience = tutor.Experience;
            //GradeLevel = tutor.GradeLevel,
            _tutor.NameOfSchool = tutor.GradeLevel;
            //Concerns
            _tutor.PreviousSubjects = tutor.PreviousSubjects;
            _tutor.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return _tutor;
        }
        public async Task<TutorViewModel> GetTutorAvailabilityAsync(ApplicationUser _user)
        {
            //var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _tutor = await this.GetTutorByUserIdAsync(_user.Id);
            _tutor = _tutor == null ? new Tutors() : _tutor;

            TutorViewModel model = new TutorViewModel()
            {
                NameOfSchool = _tutor.NameOfSchool,
                Experience = _tutor.Experience,
                PreviousSubjects = _tutor.PreviousSubjects,
                UserId = _tutor.UserId,
                Concerns = _tutor.Concerns,
                GradeLevel = _tutor.GradeLevel,
                //CreatedDate = _tutor.CreatedDate,
                Id = _tutor.Id,
                User = _user,
                SessionRequests = new List<SessionRequestsViewModel>()
            };
            var TuitionSubjects = new List<TuitionSubjectsViewModel>()
            {
                new TuitionSubjectsViewModel {Name="K-5th", Description="K-5th", Selected = false, TutorId = _tutor.Id },
                new TuitionSubjectsViewModel {Name="6th-8th", Description="6th-8th", Selected = false, TutorId = _tutor.Id },
                new TuitionSubjectsViewModel {Name="FSA/EOC", Description="FSA/EOC", Selected = false, TutorId = _tutor.Id },
                new TuitionSubjectsViewModel {Name="World Languages (Spanish)", Description="World Languages (Spanish)", Selected = false, TutorId = _tutor.Id }
            };
            if (_tutor.TuitionSubjects.Count > 0)
            {
                foreach (var sub in _tutor.TuitionSubjects)
                {
                    var subject = TuitionSubjects.FirstOrDefault(x => x.Name == sub.Name);
                    if (subject != null)
                    {
                        subject.Id = sub.Id;
                        subject.Selected = true;
                    }
                }
            }
            model.TuitionSubjects = TuitionSubjects;

            var TutorAvailability = new List<TutorAvailabilityViewModel>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                TutorAvailability.Add(new TutorAvailabilityViewModel()
                {
                    //Id
                    TutorId = _tutor.Id,
                    Timing = Utils.Common.GetTiming(),
                    Day = day
                });
            }
            if (_tutor.TutorAvailability != null)
            {
                if (_tutor.TutorAvailability.Count > 0)
                {
                    foreach (var avail in TutorAvailability)
                    {
                        foreach (var _time in avail.Timing)
                        {
                            var availTime = _tutor.TutorAvailability.FirstOrDefault(x => x.Time == _time.AvailabilityTime && x.Day == avail.Day);
                            if (availTime != null)
                            {
                                avail.Id = availTime.Id;
                                _time.Selected = true;
                                avail.Time = availTime.Time;
                            }
                        }
                    }
                }
            }
            model.TutorAvailability = TutorAvailability;

            return model;
        }
        public async Task<List<TutorViewModel>> GetTutorsModelAsync(List<Tutors> _tutors)
        {
            //var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            //var _tutor = await this.GetTutorByUserIdAsync(_user.Id);

            List<TutorViewModel> tutorsList = new List<TutorViewModel>();
            if (_tutors == null) return tutorsList;

            foreach (var _tutor in _tutors)
            {
                TutorViewModel model = new TutorViewModel()
                {
                    NameOfSchool = _tutor.NameOfSchool,
                    Experience = _tutor.Experience,
                    PreviousSubjects = _tutor.PreviousSubjects,
                    UserId = _tutor.UserId,
                    Concerns = _tutor.Concerns,
                    GradeLevel = _tutor.GradeLevel,
                    CreatedDate = _tutor.CreatedDate,
                    Id = _tutor.Id,
                    User = _tutor.User
                };
            
                //    var TuitionSubjects = new List<TuitionSubjectsViewModel>()
                //{
                //    new TuitionSubjectsViewModel {Name="K-5th", Description="K-5th", Selected = false, TutorId = _tutor.Id },
                //    new TuitionSubjectsViewModel {Name="6th-8th", Description="6th-8th", Selected = false, TutorId = _tutor.Id },
                //    new TuitionSubjectsViewModel {Name="FSA/EOC", Description="FSA/EOC", Selected = false, TutorId = _tutor.Id },
                //    new TuitionSubjectsViewModel {Name="World Languages (Spanish)", Description="World Languages (Spanish)", Selected = false, TutorId = _tutor.Id }
                //};
                //    if (_tutor.TuitionSubjects.Count > 0)
                //    {
                //        foreach (var sub in _tutor.TuitionSubjects)
                //        {
                //            var subject = TuitionSubjects.FirstOrDefault(x => x.Name == sub.Name);
                //            if (subject != null)
                //            {
                //                subject.Id = sub.Id;
                //                subject.Selected = true;
                //            }
                //        }
                //    }
                //    model.TuitionSubjects = TuitionSubjects;

                //var TutorAvailability = new List<TutorAvailabilityViewModel>();
                //foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
                //{
                //    TutorAvailability.Add(new TutorAvailabilityViewModel()
                //    {
                //        //Id
                //        TutorId = _tutor.Id,
                //        Timing = Utils.Common.GetTiming(),
                //        Day = day
                //    });
                //}
                //if (_tutor.TutorAvailability != null)
                //{
                //    if (_tutor.TutorAvailability.Count > 0)
                //    {
                //        foreach (var avail in TutorAvailability)
                //        {
                //            foreach (var _time in avail.Timing)
                //            {
                //                var availTime = _tutor.TutorAvailability.FirstOrDefault(x => x.Time == _time.AvailabilityTime && x.Day == avail.Day);
                //                if (availTime != null)
                //                {
                //                    avail.Id = availTime.Id;
                //                    _time.Selected = true;
                //                    avail.Time = availTime.Time;
                //                }
                //            }
                //        }
                //    }
                //}
                //model.TutorAvailability = TutorAvailability;

                tutorsList.Add(model);
            }

            return tutorsList;
        }
        public async Task<List<TutorViewModel>> SearchTutorsAsync(SearchViewModel model)
        {
            List<TutorViewModel> _tutorsList = new List<TutorViewModel>();
            try
            {
                model.SearchText = model.SearchText.ToLower();
                if (string.IsNullOrEmpty(model.GradeLevel) && string.IsNullOrEmpty(model.Gender))
                {
                    var _tutors = await _dbContext.Tutors
                       .Include(x => x.User)
                       .ToListAsync();
                    _tutorsList.AddRange(await this.GetTutorsModelAsync(_tutors));
                }
                else if (!string.IsNullOrEmpty(model.GradeLevel) && !string.IsNullOrEmpty(model.Gender))
                {
                    var _tutors = await _dbContext.Tutors
                        .Include(x => x.User)
                        //.Include(x => x.TuitionSubjects).Include(x => x.TutorAvailability)
                        .Where(x => (x.User.Gender == null ? false : x.User.Gender == model.Gender) && (x.GradeLevel == null ? false : x.GradeLevel.ToLower().Contains(model.GradeLevel.ToLower()))).ToListAsync();
                    _tutorsList.AddRange(await this.GetTutorsModelAsync(_tutors));
                }
                else if (!string.IsNullOrEmpty(model.GradeLevel))
                {
                    var _tutors = await _dbContext.Tutors
                        .Include(x => x.User)
                        //.Include(x => x.TuitionSubjects).Include(x => x.TutorAvailability)
                        .Where(x => x.GradeLevel == null ? false : x.GradeLevel.ToLower().Contains(model.GradeLevel.ToLower())).ToListAsync();
                    _tutorsList.AddRange(await this.GetTutorsModelAsync(_tutors));
                }
                else if (!string.IsNullOrEmpty(model.Gender))
                {
                    var _tutors = await _dbContext.Tutors
                        .Include(x => x.User)
                        //.Include(x => x.TuitionSubjects).Include(x => x.TutorAvailability)
                        .Where(x => x.User.Gender == null ? false : x.User.Gender == model.Gender).ToListAsync();
                    _tutorsList.AddRange(await this.GetTutorsModelAsync(_tutors));
                }
                //if (!string.IsNullOrEmpty(model.Day))
                //{
                //    var _tutors = await _dbContext.Tutors
                //        .Include(x => x.User)
                //        //.Include(x => x.TuitionSubjects)
                //        .Include(x => x.TutorAvailability.Where(y => y.Day == null ? false : y.Day.ToString() == model.Day))
                //        .ToListAsync();
                //    _tutors = _tutors.Where(x => x.TutorAvailability == null ? false : x.TutorAvailability.Count > 0).ToList();
                //    _tutorsList.AddRange(await this.GetTutorsModelAsync(_tutors));
                //}

                //_tutorsList = _tutorsList.Where(x => 
                //        x.User.FirstName == null ? false : x.User.FirstName.ToLower().Contains(model.SearchText)
                //        || x.User.LastName == null ? false : x.User.LastName.ToLower().Contains(model.SearchText))
                //    .OrderBy(x => x.User.FirstName).ToList();

                List<TutorViewModel> __tutorsList = new List<TutorViewModel>();
                foreach (var t in _tutorsList)
                {
                    if (t.User.FirstName == null ? false : t.User.FirstName.ToLower().Contains(model.SearchText))
                    {
                        __tutorsList.Add(t);
                    }
                    else if (t.User.LastName == null ? false : t.User.LastName.ToLower().Contains(model.SearchText))
                    {
                        __tutorsList.Add(t);
                    }
                    else if (t.NameOfSchool == null ? false : t.NameOfSchool.Contains(model.SearchText))
                    {
                        __tutorsList.Add(t);
                    }
                }
                return __tutorsList;
            }
            catch (Exception ex)
            {
                //throw error
            }
            return _tutorsList;
        }

        public async Task<(bool, Sessions)> ApproveDeclineSession(int SessionId, Sessions Session)
        {
            var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == SessionId).FirstOrDefaultAsync();
            if (_SessionDB == null)
                return (false, _SessionDB);

            _SessionDB.Status = Session.Status;
            _SessionDB.ApproveDeclineBy = Session.ApproveDeclineBy;
            _SessionDB.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return (true, _SessionDB);
        }

        public async Task<(bool, Sessions)> DeleteSession(int SessionId)
        {

            var ___SessionDB = await _dbContext.Sessions.Where(x => string.IsNullOrEmpty(x.TutorUserId)).ToListAsync();
            _dbContext.Sessions.RemoveRange(___SessionDB);
            await _dbContext.SaveChangesAsync();


            var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == SessionId).FirstOrDefaultAsync();
            if (_SessionDB == null)
                return (false, _SessionDB);

            var _SessionScheduleDB = await _dbContext.SessionSchedule.Where(x => x.SessionId == SessionId).ToListAsync();
            _dbContext.SessionSchedule.RemoveRange(_SessionScheduleDB);
            await _dbContext.SaveChangesAsync();

            _dbContext.Sessions.Remove(_SessionDB);
            await _dbContext.SaveChangesAsync();
            return (true, _SessionDB);
        }
    }
}