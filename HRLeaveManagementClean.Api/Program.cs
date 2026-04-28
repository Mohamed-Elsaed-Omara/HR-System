
using HealthChecks.UI.Client;
using HRLeaveManagement.Application;
using HRLeaveManagement.Identity;
using HRLeaveManagement.Identity.DbContext;
using HRLeaveManagement.Identity.Models;
using HRLeaveManagement.Infrastructure;
using HRLeaveManagement.Persistence;
using HRLeaveManagementClean.Api.Extensions;
using HRLeaveManagementClean.Api.Jobs;
using HRLeaveManagementClean.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace HRLeaveManagementClean.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.ConfigureIdentityServices(builder.Configuration);
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("all", builder => builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<RefreshTokenCleanupJob>();


            

            builder.Services.AddHangfire(builder); // Hangfire
            builder.AddSerilogServices(); //serilog
            builder.Services.AddRateLimitExtension(); //rate limit
            builder.Services.HealthChecks(builder); //health checks

            var app = builder.Build();

            app.UseHangfireMiddleware();
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection()
                ;
            app.UseCors("all");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.UseHealthChecksMiddleware();
            app.MapControllers();

            app.Run();
        }
    }
}
