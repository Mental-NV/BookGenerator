namespace BookGenerator.Domain.Core;

public class BookFile
{
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
}