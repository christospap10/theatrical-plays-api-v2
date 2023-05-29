using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Theatrical.Api.Swagger;
using Theatrical.Data.Context;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services;
using Theatrical.Services.Jwt;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var jwtOptions = config.GetSection("JwtOptions").Get<JwtOptions>();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOptions!.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Swagger UI authorization support
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

//dbconnection
builder.Services.AddDbContext<TheatricalPlaysDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnString")));

//services registering
//performer services
builder.Services.AddTransient<IPerformerRepository, PerformerRepository>();
builder.Services.AddTransient<IPerformerService, PerformerService>();
builder.Services.AddTransient<IPerformerValidationService, PerformerValidationService>();

//role services
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IRoleValidationService, RoleValidationService>();

//organizer services
builder.Services.AddTransient<IOrganizerRepository, OrganizerRepository>();
builder.Services.AddTransient<IOrganizerService, OrganizerService>();
builder.Services.AddTransient<IOrganizerValidationService, OrganizerValidationService>();

//Venue services
builder.Services.AddTransient<IVenueRepository, VenueRepository>();
builder.Services.AddTransient<IVenueService, VenueService>();
builder.Services.AddTransient<IVenueValidationService, VenueValidationService>();

//User services
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserValidationService, UserValidationService>();
builder.Services.AddTransient<IUserService, UserService>();

//Production services
builder.Services.AddTransient<IProductionRepository, ProductionRepository>();
builder.Services.AddTransient<IProductionValidationService, ProductionValidationService>();
builder.Services.AddTransient<IProductionService, ProductionService>();

//Event services
builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<IEventValidationService, EventValidationService>();
builder.Services.AddTransient<IEventService, EventService>();

//Contribution services
builder.Services.AddTransient<IContributionRepository, ContributionRepository>();
builder.Services.AddTransient<IContributionValidationService, ContributionValidationService>();
builder.Services.AddTransient<IContributionService, ContributionService>();

//Jwt Token service
builder.Services.AddTransient<ITokenService, TokenService>();

//Serilog Console log styling
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();