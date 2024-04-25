using PickMeUpApp.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PickMeUpApp.Services.Interfaces;
using PickMeUpApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService,UserService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UsersApiConnectionString")));
builder.Services.AddCors(options => options.AddPolicy("AllowAnyOrigin",
        builder => { builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader(); }));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
