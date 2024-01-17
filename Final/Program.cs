using Final;
using Final.Data;
using Final.Properties;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ���������, ����� �� �������������� �������� ��� ��������� ������
        ValidateIssuer = true,
        // ������, �������������� ��������
        ValidIssuer = AuthOptions.ISSUER,
        // ����� �� �������������� ����������� ������
        ValidateAudience = true,
        // ��������� ����������� ������
        ValidAudience = AuthOptions.AUDIENCE,
        // ����� �� �������������� ����� �������������
        ValidateLifetime = true,
        // ��������� ����� ������������
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        // ��������� ����� ������������
        ValidateIssuerSigningKey = true,
    };
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection), ServiceLifetime.Singleton);    // ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// ����������� �������������� � ������� jwt-�������
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRUD API Library", Version = "v1" });

    var basePath = AppContext.BaseDirectory;

    var xmlPath = Path.Combine(basePath, "Final.xml");
    c.IncludeXmlComments(xmlPath);

    // �������� ������������ Bearer ��������������
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // �������� ������� ������������, ��������, ��� ������ ������ ������� ����� Bearer
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotnetClaimAuthorization v1");

    });
}
app.UseSwagger();
app.UseSwaggerUI(c => {

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MY API");

});

app.UseAuthorization();
app.MapControllers();

app.Run();
