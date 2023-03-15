using BookGenerator.Application.Contracts.Books;
using BookGenerator.Client.ApiServices;
using BookGenerator.Client.Models;
using BookGenerator.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookGenerator.Client.Controllers;

public class HomeController : Controller
{
    private readonly IBookApiService bookApiService;

    public HomeController(IBookApiService bookApiService)
    {
        this.bookApiService = bookApiService ?? throw new System.ArgumentNullException(nameof(bookApiService));
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("Download/{bookId}")]
    public async Task<IActionResult> Download(Guid bookId)
    {
        BookFile bookFile = await bookApiService.GetResult(bookId);
        GetStatusResponse status = await bookApiService.GetStatusAsync(bookId);
        return View(new DownloadViewModel() { Book = bookFile, BookTitle = status.BookTitle });
    }

    [HttpGet("Status/{bookId}")]
    public async Task<IActionResult> Status(Guid bookId)
    {
        GetStatusResponse status = await bookApiService.GetStatusAsync(bookId);
        if (status.Status == BookStatus.Completed)
        {
            return RedirectToAction("Download", new { bookId = bookId });
        }
        else
        {
            return View(new StatusViewModel() { BookId = bookId, BookTitle = status.BookTitle, Status = status.Status });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Generate(string bookTitle)
    {
        ViewData["BookTitle"] = bookTitle;
        CreateBookResponse result = await bookApiService.CreateAsync(bookTitle);
        return RedirectToAction("Status", new { bookId = result.BookId });
    }
}