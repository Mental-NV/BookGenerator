﻿using BookGenerator.Domain.Primitives;

namespace BookGenerator.Domain.Core;

public class Book : Entity
{
    public Book(string title)
        : base(Guid.NewGuid())
    {
        this.Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Book"/> class.
    /// </summary>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    private Book()
    { 
    }

    public string Title { get; set; }
    public virtual List<Chapter> Chapters { get; set; } = new List<Chapter>();
}
