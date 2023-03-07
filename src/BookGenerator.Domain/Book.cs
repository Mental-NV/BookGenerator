namespace BookGenerator.Domain;

public class Book
{
    public string Title { get; set; }
    public Guid Id { get; set; }
    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
}
