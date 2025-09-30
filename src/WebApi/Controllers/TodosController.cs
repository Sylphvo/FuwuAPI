using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/todos")]
public sealed class TodosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public Task<List<Todo>> GetAll(CancellationToken ct) => db.Todos.AsNoTracking().ToListAsync(ct);

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] string title, CancellationToken ct)
    {
        var todo = new Todo(title);
        await db.Todos.AddAsync(todo, ct);
        await db.SaveChangesAsync(ct);
        return Ok(todo.Id);
    }
}
