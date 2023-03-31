using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VticLibrary.Models;

namespace VitcAuth.Models;

public partial class ProfessionGroup : IModel
{
    public long Id { get; set; }

    public long JobId { get; set; }

    public string GroupName { get; set; } = null!;

    public long BranchId { get; set; }

    public int CompanyId { get; set; }

    public int OrganizationId { get; set; }

    public int? Period { get; set; }

    public int Status { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual List<StudentJob> Students { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("JobId")]
    public virtual Job Job { get; set; }



    private ILazyLoader LazyLoader { get; set; }

    public ProfessionGroup()
    {
    }
    public ProfessionGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }
}
