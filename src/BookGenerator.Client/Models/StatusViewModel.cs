using BookGenerator.Domain.Core;
using System;

namespace BookGenerator.Client.Models;

public class StatusViewModel
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; }
    public BookStatus Status { get; set; }
    public int Progress { get; set; }
    public string ErrorMessage { get; set; }
}
