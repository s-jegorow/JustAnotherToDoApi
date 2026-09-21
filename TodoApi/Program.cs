using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//singleton -> scoped geändert (lifetime-konflikt im vergleich zum alten file-handling)
builder.Services.AddScoped<ITodoRepository, SqliteTodoRepository>();
builder.Services.AddDbContext<TodoDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer(); //swagger
builder.Services.AddSwaggerGen(); //swagger
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/todos", async (ITodoRepository repo) => Results.Ok(await repo.GetAllAsync()));

app.MapGet("/todos/{id}", async (int id, ITodoRepository repo) =>
{
    var todo = await repo.GetByIdAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todo);
});

app.MapPost("/todos", async (CreateTodoRequest request, ITodoRepository repo) => 
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title shouldn't be empty.");
    }

    var created = await repo.AddAsync(request.Title);
    return Results.Created($"/todos/{created.Id}", created);
});

app.MapPut("/todos/{id}", async (int id, UpdateTodoRequest request, ITodoRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title shouldn't be empty.");
    }

    var updated = await repo.UpdateAsync(id, request.Title, request.IsCompleted);
    if (updated is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updated);
});

app.MapDelete("/todos/{id}", async (int id, ITodoRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run("http://localhost:5555");