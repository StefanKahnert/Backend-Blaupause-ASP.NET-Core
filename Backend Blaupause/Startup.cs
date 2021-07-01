using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Helper.Schedule;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.DatabaseMigration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Backend_Blaupause
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers(setupAction =>
                setupAction.ReturnHttpNotAcceptable = true
               ).AddNewtonsoftJson(setupAction => {
                   setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                   setupAction.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                   setupAction.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
               }
           );

            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(sqlConnectionString));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<UserAuthentication>();

            services.AddScoped<IUser, UserImpl>();

            services.AddScoped<DatabaseMigrationService>();

            services.AddTransient((config) =>
            {
                var conf = new JWTConfiguration();
                Configuration.GetSection("JWTConfiguration").Bind(conf);
                return conf;
            });
            services.AddTransient((config) =>
            {
                var conf = new UserIdentity();
                Configuration.GetSection("UserIdentity").Bind(conf);
                return conf;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            ConfigureJwt(services);

            services.AddSignalR();

            Assembly asm = Assembly.GetExecutingAssembly();

            List<MethodInfo> jobs = asm.GetTypes()
                .Where(job => job.IsInstanceOfType(typeof(ScheduleJobs)))
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && method.IsDefined(typeof(ScheduleAttribute))).ToList();

            #pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            ServiceProvider provider = services.BuildServiceProvider();
            #pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

            jobs.ForEach(job =>
            {
                if (job.IsDefined(typeof(ScheduleSecondAttribute)))
                {
                    ScheduleSecondAttribute attribute = (ScheduleSecondAttribute)job.GetCustomAttribute(typeof(ScheduleSecondAttribute));
                    Scheduler.Interval(attribute.day, attribute.hour, attribute.min, attribute.interval, Scheduler.SECONDS, (Action)Delegate.CreateDelegate(typeof(Action), provider.GetService<ScheduleJobs>(), job));
                }
                else if (job.IsDefined(typeof(ScheduleMinuteAttribute)))
                {
                    ScheduleMinuteAttribute attribute = (ScheduleMinuteAttribute)job.GetCustomAttribute(typeof(ScheduleMinuteAttribute));
                    Scheduler.Interval(attribute.day, attribute.hour, attribute.min, attribute.interval, Scheduler.MINUTES, (Action)Delegate.CreateDelegate(typeof(Action), provider.GetService<ScheduleJobs>(), job));
                }
                else if (job.IsDefined(typeof(ScheduleHourAttribute)))
                {
                    ScheduleHourAttribute attribute = (ScheduleHourAttribute)job.GetCustomAttribute(typeof(ScheduleHourAttribute));
                    Scheduler.Interval(attribute.day, attribute.hour, attribute.min, attribute.interval, Scheduler.HOURS, (Action)Delegate.CreateDelegate(typeof(Action), provider.GetService<ScheduleJobs>(), job));
                }
                else if (job.IsDefined(typeof(ScheduleDayAttribute)))
                {
                    ScheduleDayAttribute attribute = (ScheduleDayAttribute)job.GetCustomAttribute(typeof(ScheduleDayAttribute));
                    Scheduler.Interval(attribute.day, attribute.hour, attribute.min, attribute.interval, Scheduler.HOURS, (Action)Delegate.CreateDelegate(typeof(Action), provider.GetService<ScheduleJobs>(), job));
                }

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, DatabaseMigrationService databaseMigrationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options => options.WithOrigins("*").AllowAnyMethod());

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware(typeof(ExceptionHandler));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddFile("Logs/Backend-{Date}.txt");


            databaseMigrationService.updateDatabaseToCurrentVersion();
        }

        public void ConfigureJwt(IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<JWTConfiguration>();
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.SecretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ValidIssuer = config.ValidIssuer,
                ValidAudience = config.ValidAudience
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer(c =>
            {
                c.RequireHttpsMetadata = false;
                c.SaveToken = true;
                c.TokenValidationParameters = tokenValidationParameters;
            });

        }
    }
}
