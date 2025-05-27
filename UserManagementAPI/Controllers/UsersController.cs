using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> users = new List<User>();
    private static int nextId = 1;
    private static readonly object _lock = new object();
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        lock (_lock)
        {
            _logger.LogInformation("Retrieving all users.");
            return Ok(users.ToList());
        }
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetById(int id)
    {
        lock (_lock)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }
            _logger.LogInformation("Retrieved user with ID {UserId}.", id);
            return Ok(user);
        }
    }

    [HttpPost]
    public ActionResult<User> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid user model received for creation.");
            return BadRequest(ModelState);
        }

        lock (_lock)
        {
            user.Id = nextId++;
            users.Add(user);
            _logger.LogInformation("Created user with ID {UserId}.", user.Id);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, User updatedUser)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid user model received for update.");
            return BadRequest(ModelState);
        }

        lock (_lock)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", id);
                return NotFound();
            }
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            _logger.LogInformation("Updated user with ID {UserId}.", id);
            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        lock (_lock)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
                return NotFound();
            }
            users.Remove(user);
            _logger.LogInformation("Deleted user with ID {UserId}.", id);
            return NoContent();
        }
    }
    
    [HttpGet("route-counts")]
    public ActionResult<IDictionary<string, int>> GetRouteCounts()
    {
        return Ok(UserManagementAPI.Middleware.ApiCallCounterMiddleware.GetRouteCounts());
    }
}