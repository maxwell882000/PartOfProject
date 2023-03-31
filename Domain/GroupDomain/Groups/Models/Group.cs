using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VticLibrary.Models;

namespace VitcAuth.Models;
public enum GroupStatus
{
    OLD_VERSION = 0,
    NEW_VERSION = 1,

    IS_ACTIVE = 1,

    IS_NOT_ACTIVE = 0,

    IS_FINISHED = 3,
}

public partial class Group : IModel
{
    public long Id { get; set; }

    public long CategoryId { get; set; }

    public long BranchId { get; set; }

    public long CompanyId { get; set; }

    public long OrganizationId { get; set; }

    public string GroupName { get; set; } = null!;

    public long PeriodId { get; set; }

    public int Status { get; set; }

    public long? MainTeacher { get; set; }

    public long? MainAssistantTeacher { get; set; }

    public long? MedTeacher { get; set; }

    public long? PracticeTeacher { get; set; }

    public long? PracticeAssistantTeacher { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public double? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LessonsInWeek { get; set; }

    public bool Corrected { get; set; }

    public long? SubBranchId { get; set; }

    public int LessonVersion { get; set; }

    public int? NumLesson { get; set; }

    public virtual IList<User> Students { get; set; } = new List<User>();


    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("PeriodId")]
    public virtual StudyPeriod Period { get; set; }


    public virtual List<GroupCurriculum> GroupCurriculum { get; set; }

    public virtual RegionOrganization? Organization { get; set; }

    public virtual Company? Company { get; set; }
    public virtual GroupActivationRequest GroupActivationRequest { get; set; }
    private ILazyLoader LazyLoader { get; set; }

    public Group()
    {
    }
    public Group(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }

    public User? MainTeacherEntity { get; set; }

    public User? MainAssistantTeacherEntity { get; set; }

    public User? MedTeacherEntity { get; set; }

    public User? PracticeTeacherEntity { get; set; }

    public User? PracticeAssistantTeacherEntity { get; set; }

    public virtual ICollection<User> PracticeTeachers { get; set; }

    public virtual IEnumerable<GroupPracticeTeacher> Practice { get; set; }


    public void setVersion()
    {
        LessonVersion = (StartDate < DateOnly.Parse("2022-12-1") ? (int)GroupStatus.OLD_VERSION :
             (int)GroupStatus.NEW_VERSION);
    }
}
