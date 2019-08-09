﻿using AldoGiovanniGiacomo.API.Contexts;
using AldoGiovanniGiacomo.API.Models;
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
    [Route("api/actors")]
    [Produces("application/json")]
    [ApiController]
    public class ActorController : Controller
    {
        private readonly AldoGiovanniGiacomoAPIContext _context;
        private readonly ILogger _logger;

        public ActorController(AldoGiovanniGiacomoAPIContext context, ILogger<ActorController> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Retrieve anagraphic infos about the actors Aldo, Giovanni e Giacomo
        /// </summary>
        /// <returns>An array with details of every actor</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Actor), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetActors()
        {
            _logger.LogInformation("Getting collection of every actor @ {DATE}", DateTime.UtcNow);
            List<Actor> actors = new List<Actor>();
            foreach (var actorDTO in await _context.Actors.ToListAsync())
                actors.Add(new Actor
                {
                    Name = actorDTO.Name,
                    Nickname = actorDTO.Nickname,
                    Surname = actorDTO.Surname,
                    Birth = actorDTO.Birth,
                    BirthPlace = actorDTO.BirthPlace
                });
            return Ok(actors);
        }

        /// <summary>
        /// Retrieve anagraphic details about an actor
        /// </summary>
        /// <param name="id">Actor identifier</param>
        /// <returns>Anagraphic details about the specified actor</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Actor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetActor(int id)
        {
            _logger.LogInformation("Getting actor with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
            var actorDTO = await _context.Actors.FindAsync(id);

            if (actorDTO == null)
            {
                _logger.LogWarning("Not found actor with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
                return NotFound();
            }
                
            Actor actorVO = new Actor
            {
                Name = actorDTO.Name,
                Nickname = actorDTO.Nickname,
                Surname = actorDTO.Surname,
                BirthPlace = actorDTO.BirthPlace,
                Birth = actorDTO.Birth
            };

            return Ok(actorVO);
        }

        /// <summary>
        /// Gets every quote, taken from every movie, said by the specified actor
        /// </summary>
        /// <param name="id">Actor identifier</param>
        /// <returns>A list of quotes associated to the specified actor</returns>
        [HttpGet("{id}/quotes")]
        [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetActorQuotes(int id)
        {
            _logger.LogInformation("Getting quotes of actor with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
            var actorDTO = await _context.Actors.FindAsync(id);

            if (actorDTO == null)
            {
                _logger.LogWarning("Not found actor with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
                return NotFound();
            }

            var quotesVO = new List<Quote>();
            foreach (var quoteDTO in actorDTO.Quotes)
            {
                quotesVO.Add(new Quote
                {
                    Content = quoteDTO.Content,
                    Movie = quoteDTO.Movie.Title,
                    Year = quoteDTO.Movie.Year
                });
            }

            return Ok(quotesVO);
        }

        /// <summary>
        /// Get a random quote, from a random movie, said by the specified actor
        /// </summary>
        /// <param name="id">Actor identifier</param>
        /// <returns>A random quote</returns>
        [HttpGet("{id}/quotes/random")]
        [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetRandomQuote(int id)
        {
            _logger.LogInformation("Getting random quote of actor with Id: {ID} @ {DATE}", id, DateTime.UtcNow);
            var actorDTO = await _context.Actors.FindAsync(id);

            if (actorDTO == null || actorDTO.Quotes.Count() == 0)
            {
                _logger.LogWarning("Not found actor with Id: {ID} @ {DATE}", id, new DateTime());
                return NotFound();
            }

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int randomIndex = rnd.Next(actorDTO.Quotes.Count());
            var randomQuoteDTO = actorDTO.Quotes.ToArray()[randomIndex];

            var randomQuoteVO = new Quote
            {
                Content = randomQuoteDTO.Content,
                Movie = randomQuoteDTO.Movie.Title,
                Year = randomQuoteDTO.Movie.Year
            };

            return Ok(randomQuoteVO);
        }
    }
}
