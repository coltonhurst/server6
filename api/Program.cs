
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using BusinessLogicLayer.ContactBusinessLogic;
using RepositoryLayer.ContactRepository;
using RepositoryLayer.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add builder services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IContactBusinessLogic, ContactBusinessLogic>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddDbContext<InMemoryContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

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
