using health_app.Common;
using health_app.Data;
using health_app.Models;
using health_app.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


#region PostgreSQL Identity JWT

// DB (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    opt.UseNpgsql(cs);
    opt.UseSnakeCaseNamingConvention(); // requer EFCore.NamingConventions (recomendado p/ Postgres)
});

// Identity (sem UI)
builder.Services
    .AddIdentityCore<AppUser>(opt =>
    {
        opt.User.RequireUniqueEmail = true;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireDigit = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PtBrIdentityErrorDescriber>();

// JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false; // em prod: true + HTTPS
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.FromSeconds(30),
        RoleClaimType = ClaimTypes.Role // deixa explícito
    };


    #region Padronização de erros JWT 401/403o.Events = new JwtBearerEvents
    o.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // evita resposta padrão
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json; charset=utf-8";

            var env = new ErrorEnvelope
            {
                Status = 401,
                Code = "Unauthorized",
                Title = "Não autenticado",
                Message = "Token ausente ou inválido.",
                TraceId = context.HttpContext.TraceIdentifier,
                Path = context.Request.Path
            };

            return context.Response.WriteAsJsonAsync(env);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json; charset=utf-8";

            var env = new ErrorEnvelope
            {
                Status = 403,
                Code = "Forbidden",
                Title = "Acesso negado",
                Message = "Você não tem permissão para acessar este recurso.",
                TraceId = context.HttpContext.TraceIdentifier,
                Path = context.HttpContext.Request.Path
            };

            return context.Response.WriteAsJsonAsync(env);
        }
    };
    #endregion
});

builder.Services.AddAuthorization();    

builder.Services.AddScoped<ITokenService, TokenService>();

#endregion

 
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o =>
{
    // (opcional) garantir camelCase
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

#region Padronização de erros 400
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key[1..], // fullName, email etc.
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var env = new ErrorEnvelope
        {
            Status = StatusCodes.Status400BadRequest,
            Code = "ValidationError",
            Title = "Erro de validação",
            Message = "Um ou mais campos estão inválidos.",
            Errors = errors,
            TraceId = context.HttpContext.TraceIdentifier,
            Path = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(env);
    };
});

#endregion


builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


 

// CORS p/ Svelte
var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("spa", p => p.WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

app.UseCors("spa");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

#region Padronização de erros 500
app.UseExceptionHandler(appErr =>
{
    appErr.Run(async http =>
    {
        var ctx = http;
        var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/json; charset=utf-8";

        var env = new ErrorEnvelope
        {
            Status = 500,
            Code = "InternalServerError",
            Title = "Erro interno",
            Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            TraceId = ctx.TraceIdentifier,
            Path = ctx.Request.Path
        };

        await ctx.Response.WriteAsJsonAsync(env);
    });
});

#endregion
#region Padronização de erros 404
app.UseStatusCodePages(async context =>
{
    var res = context.HttpContext.Response;
    var req = context.HttpContext.Request;

    if (res.StatusCode == StatusCodes.Status404NotFound)
    {
        res.ContentType = "application/json; charset=utf-8";
        var env = new ErrorEnvelope
        {
            Status = 404,
            Code = "NotFound",
            Title = "Recurso não encontrado",
            Message = "O recurso solicitado não foi encontrado.",
            TraceId = context.HttpContext.TraceIdentifier,
            Path = req.Path
        };
        await res.WriteAsJsonAsync(env);
    }
});
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
