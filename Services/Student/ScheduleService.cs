using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services.Student
{
    public interface IScheduleService
    {
        Task<(bool, SessionViewModel)> CreateScheduleAsync(SessionViewModel _session);
        Task<(bool, SessionViewModel)> UpdateScheduleAsync(SessionViewModel _session);
    }
    public class ScheduleService : IScheduleService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<(bool, SessionViewModel)> CreateScheduleAsync(SessionViewModel _session)
        {
            var _SessionDB = _dbContext.Sessions.Add(new Sessions()
            {
                Title = _session.Title,
                Description = _session.Description,
                CommunicationMethod = _session.CommunicationMethod,
                Concerns = _session.Concerns,
                CreatedDate = DateTime.Now,
                UserId = _session.UserId,
                TutorUserId = _session.TutorUserId,
                CreatedBy = _session.CreatedBy
            });
            await _dbContext.SaveChangesAsync();

            if (_session.SessionSchedules != null) {
                foreach (var schedule in _session.SessionSchedules)
                {
                    foreach (var time in schedule.Timing)
                    {
                        if (time.Selected)
                        {
                            var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                            {
                                Day = schedule.Day,
                                Time = time.AvailabilityTime,
                                SessionId = _SessionDB.Id,
                                CreatedDate = DateTime.Now
                            });
                            await _dbContext.SaveChangesAsync();
                            time.Id = SessionScheduleDB.Id;
                        }
                    }
                }
            }
            _session.Id = _SessionDB.Id;
            return (true, _session);
        }
        public async Task<(bool, SessionViewModel)> UpdateScheduleAsync(SessionViewModel _session)
        {
            if (_session.Id == 0)
            {
                await this.CreateScheduleAsync(_session);
            }
            else
            {
                var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == _session.Id).FirstOrDefaultAsync();
                if (_SessionDB == null)
                {
                    return (false, _session);
                }

                _SessionDB.Title = _session.Title;
                _SessionDB.Description = _session.Description;
                _SessionDB.CommunicationMethod = _session.CommunicationMethod;
                _SessionDB.TutorUserId = _session.TutorUserId;
                _SessionDB.Concerns = _session.Concerns;
                _SessionDB.UpdatedDate = DateTime.Now;
                if (string.IsNullOrEmpty(_SessionDB.CreatedBy)) _SessionDB.CreatedBy = _session.CreatedBy;
                await _dbContext.SaveChangesAsync();

                if (_session.SessionSchedules != null)
                {
                    var sessionSchedulesExist = await _dbContext.SessionSchedule.Where(x => x.SessionId == _SessionDB.Id).ToListAsync();
                    _dbContext.SessionSchedule.RemoveRange(sessionSchedulesExist);
                    await _dbContext.SaveChangesAsync();

                    foreach (var schedule in _session.SessionSchedules)
                    {
                        foreach (var time in schedule.Timing)
                        {
                            if (time.Selected)
                            {
                                var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                                {
                                    Day = schedule.Day,
                                    Time = time.AvailabilityTime,
                                    SessionId = _SessionDB.Id,
                                    CreatedDate = DateTime.Now
                                });
                                await _dbContext.SaveChangesAsync();
                                schedule.Id = SessionScheduleDB.Id;
                            }
                        }
                    }
                }
                _session.Id = _SessionDB.Id;
            }
            return (true, _session);
        }

    }
}