using AldoGiovanniGiacomo.API.Contexts;
using AldoGiovanniGiacomo.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AldoGiovanniGiacomo.API.Controllers
{
    [Route("api/quotes")]
    [Produces("application/json")]
    [ApiController]
    public class QuoteController : Controller
    {
        private readonly AldoGiovanniGiacomoAPIContext _context;
        private readonly ILogger _logger;

        public QuoteController(AldoGiovanniGiacomoAPIContext context, ILogger<QuoteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets every quote, from every movie, said by Aldo, Giovanni e Giacomo
        /// </summary>
        /// <returns>A list of quotes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(QuoteDTO), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetQuotes()
        {
            _logger.LogInformation("Getting collection of every quote @ {DATE}", DateTime.UtcNow);
            List<QuoteDTO> quotesDTO = new List<QuoteDTO>();
            foreach (var quote in await _context.Quotes.ToListAsync())
            {
                quotesDTO.Add(new QuoteDTO
                {
                    Id = quote.Id,
                    Actor = quote.Actor.Name + ' ' + quote.Actor.Surname,
                    Content = quote.Content,
                    Movie = quote.Movie.Title,
                    Year = quote.Movie.Year
                });
            }

            return Ok(quotesDTO);
        }

        /// <summary>
        /// Gets a specific quote
        /// </summary>
        /// <param name="id">Quote identifier</param>
        /// <returns>The selected quote</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuoteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetQuote(int id)
        {
            _logger.LogInformation("Getting quote with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                _logger.LogWarning("Not found quote with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
                return NotFound();
            }

            var quoteDTO = new QuoteDTO
            {
                Id = quote.Id,
                Actor = quote.Actor.Name + ' ' + quote.Actor.Surname,
                Content = quote.Content,
                Movie = quote.Movie.Title,
                Year = quote.Movie.Year
            };

            return Ok(quoteDTO);
        }

        /// <summary>
        /// Gets a random quote from a random movie
        /// </summary>
        /// <returns>A random quote</returns>
        [HttpGet("random")]
        [ProducesResponseType(typeof(QuoteDTO), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetRandomQuote()
        {
            _logger.LogInformation("Getting a random quote @ {DATE}", DateTime.UtcNow);
            var _random = new Random(Guid.NewGuid().GetHashCode());
            int randomIndex = _random.Next(1, _context.Quotes.Count());
            var randomQuote = await _context.Quotes.FindAsync(randomIndex);

            var randomQuoteDTO = new QuoteDTO
            {
                Id = randomQuote.Id,
                Actor = randomQuote.Actor.Name + ' ' + randomQuote.Actor.Surname,
                Content = randomQuote.Content,
                Movie = randomQuote.Movie.Title,
                Year = randomQuote.Movie.Year
            };

            return Ok(randomQuoteDTO);
        }
    }
}
