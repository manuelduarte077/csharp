using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Data;
using MovieApi.MapperMovie;
using MovieApi.Models;
using MovieApi.OpenApi;
using MovieApi.Repository;
using MovieApi.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default", new CacheProfile()
    {
        Duration = 60
    });
});

// .NET Identity
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


// Cache
builder.Services.AddResponseCaching();

// Add repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var key = builder.Configuration.GetValue<string>("AppSettings:KeyScret");

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }
);

apiVersioningBuilder.AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    }
);

builder.Services.AddOpenApi("v1", options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddOpenApi("v2", options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Add mappers
builder.Services.AddAutoMapper(cfg => { }, typeof(MapperMovies));

// Auth
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MovieAPI v1");
        options.SwaggerEndpoint("/openapi/v2.json", "MovieAPI v2");
    });
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();