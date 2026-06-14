using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelQuotesApi.Data;
using TravelQuotesApi.Interfaces;
using TravelQuotesApi.Models;

[Route("api/[controller]")]
[ApiController]
public class QuotesController : ControllerBase
{
    private readonly IRepository<Quote> _quoteRepository;

    public QuotesController(IRepository<Quote> quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    // GET: api/Quotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
    {
        var quotes = await _quoteRepository.GetAllAsync();
        return Ok(quotes);
    }

    // POST: api/Quotes
    [HttpPost]
    public async Task<ActionResult<Quote>> PostQuote(Quote quote)
    {
        quote.Id = 0;
        await _quoteRepository.CreateAsync(quote);
        return Ok(quote);
    }
}