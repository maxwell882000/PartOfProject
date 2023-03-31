using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VticLibrary.Models;

namespace VitcAuth.Models;

public partial class JobStudentPayment : IModel
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public int? GroupId { get; set; }

    public long BranchId { get; set; }

    public int CategoryId { get; set; }

    public int AuthorId { get; set; }

    public decimal ActualPrice { get; set; }

    public decimal? PaidPrice { get; set; }

    public DateOnly? PaidDate { get; set; }

    public long Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("StudentId")]
    public virtual StudentJob Student { get; set; }

    [ForeignKey("Status")]
    public virtual BranchStudentPaymentStatus? Statuses { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; }

    private ILazyLoader LazyLoader { get; set; }
    public JobStudentPayment()
    {

    }
    public JobStudentPayment(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }

}
