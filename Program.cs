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
          // Under dev �r det oftast enklast att till�ta alla origin:
          //.AllowAnyOrigin()
          //.AllowAnyHeader()
          //.AllowAnyMethod();

        // Om du vill vara lite str�ngare, men �nd� till�ta Angular p� b�de http/https:
         .WithOrigins("http://localhost:4200", "https://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod();
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
    // 1) L�gg till en s�kerhetsdefinition f�r Bearer-token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv: Bearer {din JWT-token}"
    });

    // 2) St�ll in att alla endpoints kr�ver denna security
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1.1) CORS m�ste ligga f�re Auth
// Aktivera CORS
app.UseCors();  // anv�nder DefaultPolicy

// Aktivera JWT-pipeline
app.UseAuthentication();
app.UseAuthorization();

// Koppla in alla [ApiController]-routes
app.MapControllers();

// TEST root API
app.MapGet("/", () => "API is running!");

app.Run();
