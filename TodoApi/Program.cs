using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.OpenApi.Models;

using TodoApi; // Namespace containing your ToDoDbContext and Item classes

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
                // builder.WithOrigins("http://localhost:3000")

               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services and configure database context
builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), new MariaDbServerVersion(new Version(8, 0, 36)));
});

// Enable CORS
builder.Services.AddEndpointsApiExplorer();

// Enable Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Enable CORS
app.UseCors();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));

// Map routes
app.MapGet("/tasks", async (ToDoDbContext dbContext) =>
{
    var tasks = await dbContext.Items.ToListAsync();
    return Results.Ok(tasks);
});

app.MapPost("/tasks", async (ToDoDbContext dbContext, Item newItem) =>
{
    newItem.IsComplete=false;
    dbContext.Items.Add(newItem);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/tasks/{newItem.Id}", newItem);
    
});

app.MapPut("/tasks/{taskId}", async (ToDoDbContext dbContext, int taskId, Item updatedItem) =>
{
    var existingItem = await dbContext.Items.FindAsync(taskId);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;

    await dbContext.SaveChangesAsync();

    return Results.Ok(existingItem);
});

app.MapDelete("/tasks/{taskId}", async (ToDoDbContext dbContext, int taskId) =>
{
    var existingItem = await dbContext.Items.FindAsync(taskId);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    dbContext.Items.Remove(existingItem);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
