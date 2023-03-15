using BookGenerator.Domain.Core;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BookGenerator.Client.Models;

public class DownloadViewModel
{
    public BookFile Book { get; set; }
    public string BookTitle { get; set; }
    public string Content
    {
        get
        {
            return Encoding.UTF8.GetString(Book.Content);
        }
    }
    public string SafeFileName
    {
        get
        {
            return SanitizeFileName(BookTitle) + ".md";
        }
    }
    private static string SanitizeFileName(string fileName)
    {
        // Define the list of illegal characters for file names
        char[] illegalChars = Path.GetInvalidFileNameChars();

        // Replace each illegal character with an underscore
        foreach (char c in illegalChars)
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName;
    }
}
