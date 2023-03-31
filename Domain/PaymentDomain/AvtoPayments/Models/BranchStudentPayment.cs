using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VitcLibrary.Attributes;
using VticLibrary.Models;

namespace VitcAuth.Models;

public partial class BranchStudentPayment : IModel
{
    public virtual long Id { get; set; }

    public long StudentId { get; set; }

    public long? GroupId { get; set; }

    public long BranchId { get; set; }

    public long CategoryId { get; set; }

    public long AuthorId { get; set; }

    public string ActualPrice { get; set; } = null!;

    public double? PaidPrice { get; set; }

    public DateOnly? PaidDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long Status { get; set; }

    public int? PaymentType { get; set; }

    [ForeignKey("Status")]
    public virtual BranchStudentPaymentStatus? PaymentStatus { get; set; }

    [ForeignKey("StudentId")]
    public virtual User Student { get; set; } = null!;


    private ILazyLoader LazyLoader { get; set; }

    public BranchStudentPayment()
    {
    }
    public BranchStudentPayment(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }
}
