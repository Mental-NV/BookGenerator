namespace BookGenerator.Domain.Core;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
}
