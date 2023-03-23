using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Helper.Schedule;
using Backend_Blaupause.Models.DatabaseMigration;
using Backend_Blaupause.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers(setupAction =>
    setupAction.ReturnHttpNotAcceptable = true
   ).AddNewtonsoftJson(setupAction => {
       setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
       setupAction.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
       setupAction.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
   }
); builder.Configuration.GetValue<string>("DefaultConnection");

var sqlConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(sqlConnectionString));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<UserAuthentication>();

builder.Services.AddScoped<IUser, UserImpl>();

builder.Services.AddScoped<DatabaseMigrationService>();

builder.Services.AddScoped<ScheduleJobs>();

builder.Services.AddTransient((config) =>
{
    var conf = new JWTConfiguration();
    builder.Configuration.GetSection("JWTConfiguration").Bind(conf);
    return conf;
});
builder.Services.AddTransient((config) =>
{
    var conf = new UserIdentity();
    builder.Configuration.GetSection("UserIdentity").Bind(conf);
    return conf;
});

builder.Services.AddResponseCaching();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Backend Blaupause",
        Description = "Blaupause um schnell .NET backend aufzubauen",
        Version = "v1"
    });
    // Use method name as operationId
    c.CustomOperationIds(apiDesc => { return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null; });
});

ConfigureJwt(builder.Services);

ConfigureScheduler(builder.Services);

builder.Services.AddSignalR();

builder.Logging.AddFile("Logs/Backend-{Date}.txt");

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.WithOrigins("*").AllowAnyMethod());

app.UseHttpsRedirection();

app.UseRouting();

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware(typeof(ExceptionHandler));

app.MapControllers();




var databasemigrationService = builder.Services.BuildServiceProvider().GetService<DatabaseMigrationService>();

app.Run();


void ConfigureScheduler(IServiceCollection services)
{
    Assembly asm = Assembly.GetExecutingAssembly();

    List<MethodInfo> jobs = asm.GetTypes()
        .SelectMany(type => type.GetMethods())
        .Where(method => method.IsPublic && method.IsDefined(typeof(ScheduleAttribute))).ToList();

    ServiceProvider provider = services.BuildServiceProvider();

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

void ConfigureJwt(IServiceCollection services)
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