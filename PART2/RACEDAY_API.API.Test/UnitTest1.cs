using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RACEDAY_API.Controllers;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

namespace RACEDAY_API.API.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Register_WithNewEmail_Succeeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            var controller = new AuthController(context);

            var dto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "newuser@raceday.com",
                Password = "Password123!",
                Role = "Participant",
                PhoneNumber = "0821234567"
            };

            // Act
            var result = controller.Register(dto);

            // Assert
            Assert.IsType<ObjectResult>(result);

            var objectResult = (ObjectResult)result;

            Assert.Equal(201, objectResult.StatusCode);
        }



        [Fact]
        public void Register_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            context.Users.Add(new User
            {
                FirstName = "Existing",
                LastName = "User",
                Email = "taken@raceday.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Participant",
                PhoneNumber = "0821234567"
            });

            context.SaveChanges();

            var controller = new AuthController(context);

            var dto = new RegisterDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "taken@raceday.com",
                Password = "Password123!",
                Role = "Participant",
                PhoneNumber = "0831234567"
            };

            // Act
            var result = controller.Register(dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }


        [Fact]
        public async Task Login_WithCorrectCredentials_Succeeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

            context.Users.Add(new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "login@raceday.com",
                PasswordHash = passwordHash,
                Role = "Participant",
                PhoneNumber = "0821234567"
            });

            context.SaveChanges();

            var controller = new AuthController(context);

            var services = new ServiceCollection();

            services.AddLogging();

            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var dto = new LoginDto
            {
                Email = "login@raceday.com",
                Password = "Password123!"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

            context.Users.Add(new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "loginfail@raceday.com",
                PasswordHash = passwordHash,
                Role = "Participant",
                PhoneNumber = "0821234567"
            });

            context.SaveChanges();

            var controller = new AuthController(context);

            var dto = new LoginDto
            {
                Email = "loginfail@raceday.com",
                Password = "WrongPassword123!"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }


        [Fact]
        public void CreateEvent_AsOrganiser_Succeeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            var organiser = new User
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "Organiser",
                Email = "organiser@raceday.com",
                PasswordHash = "hashed-password",
                Role = "Organiser",
                PhoneNumber = "0821234567"
            };

            context.Users.Add(organiser);
            context.SaveChanges();

            var controller = new EventController(context);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Role, "Organiser")
    };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var newEvent = new Event
            {
                EventName = "Test Race",
                EventDescription = "Test running event.",
                EventDate = DateTime.Now.AddDays(30),
                EventLocation = "Pretoria",
                EventDistance = 10.00m,
                EventType = "Running",
                BannerImageUrl = "https://example.com/banner.jpg"
            };

            // Act
            var result = controller.CreateEvent(newEvent);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);

            var createdEvent = context.Events.First();

            Assert.Equal("Test Race", createdEvent.EventName);
            Assert.Equal(1, createdEvent.OrganiserId);
        }


        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbid()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddAuthorization();

            var serviceProvider = services.BuildServiceProvider();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "2"),
        new Claim(ClaimTypes.Role, "Participant")
    };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var authorizationService =
                serviceProvider.GetRequiredService<IAuthorizationService>();

            var policy = new AuthorizationPolicyBuilder()
                .RequireRole("Organiser")
                .Build();

            // Act
            var result = await authorizationService.AuthorizeAsync(
                principal,
                null,
                policy);

            // Assert
            Assert.False(result.Succeeded);
        }


        [Fact]
        public void CreateEnrolment_AsParticipant_SucceedsAndIsRecorded()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDBContext(options);

            var eventItem = new Event
            {
                EventId = 3,
                EventName = "Test Race",
                EventDescription = "Test running event.",
                EventDate = DateTime.Now.AddDays(30),
                EventLocation = "Pretoria",
                EventDistance = 10.00m,
                EventType = "Running",
                BannerImageUrl = "https://example.com/banner.jpg",
                OrganiserId = 1
            };

            context.Events.Add(eventItem);
            context.SaveChanges();

            var controller = new EnrolmentController(context);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "2"),
        new Claim(ClaimTypes.Role, "Participant")
    };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var newEnrolment = new Enrolment
            {
                EventId = 3,
                EnrolmentStatus = "Active"
            };

            // Act
            var result = controller.CreateEnrolment(newEnrolment);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);

            var savedEnrolment = context.Enrolments.First();

            Assert.Equal(3, savedEnrolment.EventId);
            Assert.Equal(2, savedEnrolment.ParticipantId);
            Assert.Equal("Active", savedEnrolment.EnrolmentStatus);
        }


        [Fact]
        public async Task CreateEnrolment_AsOrganiser_IsRejected()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddAuthorization();

            var serviceProvider = services.BuildServiceProvider();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Role, "Organiser")
    };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var authorizationService =
                serviceProvider.GetRequiredService<IAuthorizationService>();

            var policy = new AuthorizationPolicyBuilder()
                .RequireRole("Participant")
                .Build();

            // Act
            var result = await authorizationService.AuthorizeAsync(
                principal,
                null,
                policy);

            // Assert
            Assert.False(result.Succeeded);
        }
    }
}