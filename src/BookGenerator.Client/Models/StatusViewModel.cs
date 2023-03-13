using BookGenerator.Domain.Core;
using System;

namespace BookGenerator.Client.Models;

public class StatusViewModel
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; }
    public BookCreatingStatus Status { get; set; }
}
