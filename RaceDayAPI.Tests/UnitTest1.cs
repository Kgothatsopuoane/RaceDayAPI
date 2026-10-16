


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EventsControllerTests
    {
        private RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task GetEvents_ReturnsOk()
        {
            using var context = CreateContext();

            var controller = new EventsController(context);

            var result = await controller.GetEvents();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
        {
            using var context = CreateContext();

            var controller = new EventsController(context);

            var result = await controller.GetEvent(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_WhenEventExists_ReturnsOk()
        {
            using var context = CreateContext();

            var raceEvent = new Event
            {
                EventId = 1,
                EventName = "Johannesburg City Run",
                Description = "A community road running event.",
                EventDate = new DateTime(2027, 5, 1),
                Location = "Johannesburg",
                DistanceKm = 10,
                EventType = "Running",
                OrganiserId = 1
            };

            context.Events.Add(raceEvent);
            await context.SaveChangesAsync();

            // Verify the event was saved.
            var savedEvent = await context.Events
                .FirstOrDefaultAsync(e => e.EventId == 1);

            Assert.NotNull(savedEvent);

            var controller = new EventsController(context);

            var result = await controller.GetEvent(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}

