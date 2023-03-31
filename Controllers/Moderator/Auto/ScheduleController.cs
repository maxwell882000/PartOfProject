using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VitcAuth.Repository.Interfaces;
using AutoMapper;
using VitcAuth.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq.Expressions;

namespace VitcAuth.Controllers.Moderator.Auto
{

    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ScheduleController : ApiBaseController
    {
        private IGroupRepository groupRepository;
        private IMapper mapper;
        private IUserRepository userRepository;

        IGroupCurriculumRepository groupCirriculumRepository;
        ICategoryCurriculumRepository categoryCurriculumRepository;
        ICurriculumBanDayRepository banDayRepository;
        IPriceRepository priceRepository;
        public ScheduleController(IGroupRepository groupRepository,
            IUserRepository userRepository,
            IGroupCurriculumRepository groupCirriculumRepository,
            ICurriculumBanDayRepository banDayRepository,
            ICategoryCurriculumRepository categoryCurriculumRepository,
            IPriceRepository priceRepository,
            IMapper mapper)
        {
            this.groupRepository = groupRepository;
            this.userRepository = userRepository;
            this.categoryCurriculumRepository = categoryCurriculumRepository;
            this.groupCirriculumRepository = groupCirriculumRepository;
            this.banDayRepository = banDayRepository;
            this.priceRepository = priceRepository;
            this.mapper = mapper;
        }

        [HttpGet("GetLessonHours")]
        public IActionResult GetLessonHours(long CategoryId, long LessonVersion)
        {
            return Ok(new
            {
                Hours = this.categoryCurriculumRepository.GetAll().Where(e =>
            e.CategoryId == CategoryId &&
            e.LessonVersion == LessonVersion
            ).Sum(e => e.Hours)
            });
        }



        [HttpPost("SwapByDateCurriculum")]
        public IActionResult SwapByDateCurriculum(SwapByDateCurriculum swap)
        {
            var curriculumOld = this.groupCirriculumRepository.GetAll().Where(e => e.GroupId == swap.GroupId
            && e.Date == swap.Old).ToList();
            var curriculumNew = this.groupCirriculumRepository.GetAll().Where(e => e.GroupId == swap.GroupId
            && e.Date == swap.New).ToList();
            curriculumOld.ForEach(e => e.Date = swap.New);
            curriculumNew.ForEach(e => e.Date = swap.Old);
            this.groupCirriculumRepository.update(curriculumOld);
            this.groupCirriculumRepository.update(curriculumNew);
            this.groupCirriculumRepository.commit();
            return Ok(new { status = "Ok" });
        }

        [HttpPost("SwapCurriculum")]
        public IActionResult SwapCurriculum(SwapCurriculum swap)
        {
            var curriculums = this.groupCirriculumRepository
            .GetAll().Where(e => e.GroupId == swap.GroupId && swap.Date == e.Date).OrderBy(e => e.Sort).ToList();
            var Sort = 1;
            var result = curriculums.Where(e => e.Id != swap.Id).ToList();
            result.ForEach(e =>
            {
                if (Sort == swap.Sort)
                    Sort++;
                e.Sort = Sort;
                Sort++;
            });
            curriculums.Where(e => e.Id == swap.Id).ToList().ForEach(e => e.Sort = swap.Sort);
            this.groupCirriculumRepository.update(curriculums);
            this.groupCirriculumRepository.commit();
            return Ok(new
            {
                data = this.getCurriculum(e => e.GroupId == swap.GroupId && e.Date == swap.Date)[swap.Date],
                date = swap.Date
            });
        }
        private Dictionary<DateOnly, List<ScheduleResponse>> getCurriculum(
            Expression<Func<GroupCurriculum, bool>> conditions)
        {

            var dict = new Dictionary<DateOnly, List<ScheduleResponse>>();
            this.groupCirriculumRepository
            .GetAll()
            .Include(e => e.Lesson)
            .Include(e => e.Type)
            .Where(conditions)
            .OrderBy(e=> e.Date)
            .ThenBy(e => e.Sort)
            .ToList().ForEach(e =>
            {
                var entity = this.mapper.Map<ScheduleResponse>(e);
                if (dict.ContainsKey(e.Date))
                    dict[e.Date].Add(entity);
                else
                    dict[e.Date] = new List<ScheduleResponse>() { entity };
            });
            return dict;
        }

        [HttpGet("GetGroupCurricula/{GroupId}")]
        public IActionResult GetGroupCurricula(long GroupId)
        {
            var group = this.mapper.Map<GroupResponse>(this.groupRepository.GetAll().Include(e => e.Period).First(e => e.Id == GroupId));

            return Ok(new
            {
                group,
                curriculum = this.getCurriculum(e => e.GroupId == GroupId)
            });
        }

        [HttpPost("GenerateCurriculum")]
        public async Task<IActionResult> GenerateCurriculum(GenerateCurriculum generateCurriculum)
        {
            var group = this.groupRepository.GetAll().First(e => e.Id == generateCurriculum.GroupId);
            group.LessonsInWeek = JsonSerializer.Serialize(generateCurriculum.WeekDays);
            group.NumLesson = generateCurriculum.Count;
            this.groupRepository.update(group);
            this.groupRepository.commit();
            var categories = this.categoryCurriculumRepository.GetAll()
            .Include(e => e.Lesson)
            .ThenInclude(e => e.LessonTeacherType)
            .Where(e => e.CategoryId == group.CategoryId
             && e.LessonVersion == group.LessonVersion)
             .OrderBy(e => e.Sort)
             .ToList();
            var currentDate = this.getNextSafeDay((DateOnly)group.StartDate, generateCurriculum.WeekDays);
            var newCurriculumForGroup = new List<GroupCurriculum>();
            var lessonPerDay = 0;
            foreach (var curriculumInGroup in categories)
            {
                for (var i = 0; i < curriculumInGroup.Hours; i++)
                {
                    var newGroupCurriculum = this.mapper.Map<GroupCurriculum>(curriculumInGroup);
                    newGroupCurriculum.BranchId = group.BranchId;
                    newGroupCurriculum.AuthorId = AuthorId;
                    newGroupCurriculum.Sort += i + 1;
                    newGroupCurriculum.CompanyId = group.CompanyId;
                    newGroupCurriculum.GroupId = group.Id;
                    newGroupCurriculum.TeacherId = getTeacherOfParticularLesson(curriculumInGroup, group);
                    newGroupCurriculum.Date = currentDate;
                    group.EndDate = currentDate;
                    newCurriculumForGroup.Add(newGroupCurriculum);

                    lessonPerDay++;

                    if (lessonPerDay >= group.NumLesson)
                    {
                        lessonPerDay = 0;
                        currentDate = this.getNextSafeDay(currentDate.AddDays(1), generateCurriculum.WeekDays);
                    }
                }
            }
            this.groupCirriculumRepository.AddRange(newCurriculumForGroup);
            this.groupCirriculumRepository.commit();
            await this.priceRepository.setLearningAndAttendDays(group.Id);
            return Ok();
        }

        private DateOnly getNextSafeDay(DateOnly date, List<int> weakDays)
        {
            if (!weakDays.Contains((int)date.DayOfWeek) || this.banDayRepository.isBanDay(date))
            {
                return getNextSafeDay(date.AddDays(1), weakDays);
            }
            return date;
        }

        private long? getTeacherOfParticularLesson(CategoryCurriculum curriculum, Group group)
        {
            if (curriculum.Lesson != null)
            {
                var TeacherTypeId = curriculum.Lesson.LessonTeacherType.TeacherTypeId;
                switch (TeacherTypeId)
                {
                    case 1: return group.MainTeacher;
                    case 3: return group.MedTeacher;
                }
            }
            return null;
        }

        [HttpDelete("GenerateCurriculum/{GroupID}")]
        public IActionResult removeGroupSchedule(long GroupId)
        {
            this.groupCirriculumRepository.RemoveRange(this.groupCirriculumRepository.GetAll().Where(e => e.GroupId == GroupId));
            this.groupCirriculumRepository.commit();
            var group = this.groupRepository.GetById(GroupId);
            group.EndDate = null;
            this.groupRepository.update(group);
            this.groupRepository.commit();
            return Ok();
        }
    }
}