using BookGenerator.Application.Contracts.Books;
using BookGenerator.Client.ApiServices;
using BookGenerator.Client.Models;
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("Status/{bookId}")]
    public IActionResult Status(Guid bookId)
    {
        return View(new StatusViewModel() { BookId = bookId, BookTitle = "Not set"});
    }

    [HttpPost]
    public async Task<IActionResult> Generate(string bookTitle)
    {
        CreateBookResponse result = await bookApiService.CreateAsync(bookTitle);
        return RedirectToAction("Status", new { bookId = result.BookId });
    }
}