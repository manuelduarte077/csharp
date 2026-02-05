using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.MapperMovie;
using MovieApi.Repository;
using MovieApi.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add mappers
builder.Services.AddAutoMapper(cfg => { }, typeof(MapperMovies));


// CORS
builder.Services.AddCors(p =>
    p.AddPolicy("CorsPolicy", build =>
        build.WithOrigins("http://localhost:5205")
            .AllowAnyMethod()
            .AllowAnyHeader())
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "MovieApi v1"); });
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();