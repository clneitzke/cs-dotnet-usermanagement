namespace UserManagementAPI;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static List<User> users = new List<User>();
    private static int nextId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        return Ok(users);
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetById(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create(User user)
    {
        user.Id = nextId++;
        users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, User updatedUser)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        users.Remove(user);
        return NoContent();
    }
}