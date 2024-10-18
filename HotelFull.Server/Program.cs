using HotelAPI.Models;
using HotelAPI.Service;
using HotelFull.Server.Filters;
using HotelFull.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Authentication;


namespace HotelFull.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("MySql");
            builder.Services.AddDbContext<HotelContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddHttpClient();


            builder.Services.AddScoped<HotelService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });
            });

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            }).AddGoogle(options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
            });


            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<MemberJwtAuthFilter>();
            builder.Services.AddScoped<EmployeeJwtAuthFilter>();
            builder.Services.AddScoped<AdminJwtAuthFilter>();
            builder.Services.AddScoped<EcpayService>();

            builder.Services.AddOptions<JwtSettings>()
            .Bind(builder.Configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .PostConfigure(options =>
            {
                var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
                if (string.IsNullOrEmpty(options.Key))
                {
                    logger.LogError("JWT Key is null or empty in configuration");
                }
                else
                {
                    logger.LogInformation("JWT Key is configured successfully");
                }
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelAPI", Version = "v1" });
                c.OperationFilter<SwaggerFileUploadFilter>();
            });

            // 20241014 add

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                // 取得appsettings.json
                var config = builder.Configuration;
                // 取得appsettings.json Kestrel的部分
                serverOptions.Configure(config.GetSection("Kestrel"));
                serverOptions.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                });
            });
    

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();

        }
    }

}
