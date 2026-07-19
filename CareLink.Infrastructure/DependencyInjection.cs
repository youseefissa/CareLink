using CareLink.Application.Interfaces;
using CareLink.Domain.Interfaces.Repositories;
using CareLink.Infrastructure.BackgroundServices;
using CareLink.Infrastructure.BackgroundServices;
using CareLink.Infrastructure.ExternalServices;
using CareLink.Infrastructure.Identity;
using CareLink.Infrastructure.Persistence;
using CareLink.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CareLink.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CareLinkDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();
            services.AddScoped<IFallEventRepository, FallEventRepository>();
            services.AddScoped<ISOSEventRepository, SOSEventRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddHostedService<InactivityMonitorService>();
            services.AddScoped<IPushNotificationSender, FirebaseNotificationSender>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            return services;
        }
    }
}