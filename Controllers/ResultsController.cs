using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // Organisers can view results for their events
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            int organiserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Where(r => r.Event!.OrganiserId == organiserId)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/my
        // Participants can view their own results
        [Authorize(Roles = "Participant")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            int participantId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var results = await _context.Results
                .Include(r => r.Event)
                .Where(r => r.ParticipantId == participantId)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            bool isOrganiser =
                User.IsInRole("Organiser") &&
                result.Event?.OrganiserId == userId;

            bool isParticipant =
                User.IsInRole("Participant") &&
                result.ParticipantId == userId;

            if (!isOrganiser && !isParticipant)
            {
                return Forbid();
            }

            return Ok(result);
        }

        // POST: api/Results
        // Organisers record participant results
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateResult(
    [FromForm] CreateResultDto request)
        {
            var raceEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == request.EventId);

            if (raceEvent == null)
                return NotFound("Event not found.");

            var participant = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == request.ParticipantId &&
                    u.Role == "Participant");

            if (participant == null)
                return NotFound("Participant not found.");

            var result = new Result
            {
                EventId = request.EventId,
                ParticipantId = request.ParticipantId,
                FinishingPosition = request.FinishingPosition,
                FinishTime = request.FinishTime
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultId },
                result);
        }

        // PUT: api/Results/5
        // Organisers can update results for their events
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(
            int id,
            Result updatedResult)
        {
            int organiserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _context.Results
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            if (result.Event?.OrganiserId != organiserId)
            {
                return Forbid();
            }

            result.FinishingPosition = updatedResult.FinishingPosition;
            result.FinishTime = updatedResult.FinishTime;

            await _context.SaveChangesAsync();

            return Ok(result);
        }

        // DELETE: api/Results/5
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            int organiserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _context.Results
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            if (result.Event?.OrganiserId != organiserId)
            {
                return Forbid();
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok("Result deleted successfully.");
        }
    }
}
