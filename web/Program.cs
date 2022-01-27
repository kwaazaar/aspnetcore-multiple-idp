using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using web.auth.AuthPolicies;
using web.auth.BasicAuth;
using web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddAuthentication() // No default scheme, so authentication is not done automatically: Authorize-attribute must now be present and (default) policy must now specify schemes
        .AddBasicAuthentication()
        .AddJwtBearer("idp", options =>
        {
            options.MetadataAddress = "https://localhost:5001/.well-known/openid-configuration";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "client_id",
                ValidateAudience = false
            };
            options.Validate();
        })
        .AddJwtBearer("azuread", options =>
        {
            options.MetadataAddress = builder.Configuration["Authentication:Metadata"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = "api://aspnetcorepoc",
                NameClaimType = "appid",
                ValidateIssuer = !bool.TryParse(builder.Configuration["Authentication:ValidateIssuer"], out var validateIssuer) || validateIssuer, // Azure AD tokens have invalid issuer, according to the OIDC metadata
            };
            options.Validate();
        })
        ;

var allSchemes = new string[] { BasicAuthenticationHandler.DefaultScheme, "idp", "azuread" };

builder.Services.AddAuthorization(options => options // Enable authorization and set default policy to require authenticated user
    .AddApiAuthorizationPolicy(allSchemes) // Try authenticate with all schemes, instead of just the default
    .AddUserAuthorizationPolicy(allSchemes) // Try authenticate with all schemes, instead of just the default
    );

builder.Services.AddAuthorizationHandlers(); // Implementation of requirements used in policies

builder.Services.AddServiceModelServices();

builder.Services.AddSignalR();

builder.Services.AddControllers().
    AddApplicationPart(typeof(TestController).Assembly);

builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "IreckonU Secured Api", Version = "v1" });

        // Seurity config taken from: https://codeburst.io/api-security-in-swagger-f2afff82fb8e
        // (Swagger documentation is very extensive and without useful examples)

        // OAuth2 JWT
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter JWT Bearer token **_only_**",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http, //
            Scheme = "bearer", // must be lower case
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseServiceModel(builder =>
{
    builder.ConfigureServiceHostBase<Calculator>(host =>
    {
        // All kinds of metadata
        //host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
    });
    builder
        .AddService<Calculator>()
        .AddServiceEndpoint<Calculator, ICalculator>(new BasicHttpBinding(), "http://localhost/basicHttpUserPassword");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");
app.MapRazorPages();
app.MapControllers();

app.Run();
