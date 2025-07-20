using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookQuotesApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 0) Registrera dina controllers
builder.Services.AddControllers();

// 0.1) CORS
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy
         // Under dev är det oftast enklast att tillåta alla origin:
         .AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();

         // Om du vill vara lite strängare, men ändå tillåta Angular på både http/https:
         //.WithOrigins("http://localhost:4200", "https://localhost:4200") // LÄGG TILL MIN FRONTENDAPP SENARE
         //.AllowAnyHeader()
         //.AllowAnyMethod();
    });
});

// 1) EF Core
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// 2) JWT-autentisering
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts =>
  {
      opts.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,

          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
)
      };
  });

// 3) Authorization
builder.Services.AddAuthorization();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 1) Lägg till en säkerhetsdefinition för Bearer-token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv: Bearer {din JWT-token}"
    });

    // 2) Ställ in att alla endpoints kräver denna security
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Swagger i dev
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Tillåta swagger i prod.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 1.1) CORS måste ligga före Auth
// Aktivera CORS
app.UseCors();  // använder DefaultPolicy

// Aktivera JWT-pipeline
app.UseAuthentication();
app.UseAuthorization();

// Koppla in alla [ApiController]-routes
app.MapControllers();

// TEST root API
app.MapGet("/", () => "API is running!");

app.Run();
