using Marcador.Api.Infra;
using Marcador.Api.Realtime;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MarcadorDb")));
builder.Services.AddSignalR();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("ng", p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); 
}

app.UseCors("ng");
app.MapControllers();
app.MapHub<ScoreHub>("/hub/score");
app.Run();
