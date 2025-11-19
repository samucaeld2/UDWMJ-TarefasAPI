using Microsoft.EntityFrameworkCore;
using TarefasApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TarefaContext>(options =>
    options.UseInMemoryDatabase("TarefaList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tarefas", async (TarefaContext db) => await db.TB_TAREFAS.ToListAsync());

app.MapGet("/tarefas/{id}", async (int id, TarefaContext db) =>
{
    return await db.TB_TAREFAS.FindAsync(id)
        is TarefasApi.Models.Tarefa tarefa
            ? Results.Ok(tarefa)
            : Results.NotFound();
});
 
 

app.MapPost("/tarefas", async (TarefasApi.Models.Tarefa tarefa, TarefaContext db) =>
{
    db.TB_TAREFAS.Add(tarefa);
    await db.SaveChangesAsync();
 
    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});
 
app.MapPut("/tarefas/{id}", async (int id, TarefasApi.Models.Tarefa updatedTarefa, TarefaContext db) =>
{
    var tarefa = await db.TB_TAREFAS.FindAsync(id);
 
    if (tarefa is null) return Results.NotFound();
 
    tarefa.Nome = updatedTarefa.Nome;
    tarefa.FoiConcluida = updatedTarefa.FoiConcluida;
 
    await db.SaveChangesAsync();
 
    return Results.NoContent();
});
 
app.MapDelete("/tarefas/{id}", async (int id, TarefaContext db) =>
{
    if (await db.TB_TAREFAS.FindAsync(id) is TarefasApi.Models.Tarefa tarefa)
    {
        db.TB_TAREFAS.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.Ok(tarefa);
    }
 
    return Results.NotFound();
});
 



app.Run();