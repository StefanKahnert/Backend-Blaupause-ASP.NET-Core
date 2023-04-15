using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Helper.Schedule;
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
using Microsoft.AspNetCore.Identity;
using Backend_Blaupause.Models.Entities;
using Backend_Blaupause.Helper.GraphQL;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.IO;
using Backend_Blaupause.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

//Configure JSON Parser
builder.Services.AddControllers(setupAction =>
    setupAction.ReturnHttpNotAcceptable = true
   ).AddNewtonsoftJson(setupAction => {
       setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
       setupAction.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
       setupAction.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
   }
); builder.Configuration.GetValue<string>("DefaultConnection");

//Configure DB Connection
var sqlConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(sqlConnectionString));

//Configure Services
builder.Services.AddTransient<IUser, UserImpl>();
builder.Services.AddTransient<ScheduleJobs>();
builder.Services.AddTransient<NotificationHub>();

//Configure Auth
builder.Services.AddTransient((config) =>
{
    var conf = new JWTConfiguration();
    builder.Configuration.GetSection("JWTConfiguration").Bind(conf);
    return conf;
});
builder.Services.AddIdentity<User, Permission>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

//Configure GraphQL Endpoint
builder.Services
    .AddGraphQLServer()
    .AddQueryType<GraphQLQuery>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

//Configure API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(2, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

//Configure Caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

//Configure Endpoint Documentation
builder.Services.AddSwaggerGen(c =>
{
    var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
    apiVersionDescriptionProvider.ApiVersionDescriptions.ToList().ForEach(description =>
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = "ASP.NET Core",
            Description = "Best Practice",
            Version = description.GroupName
        });
    });
    // Use method name as operationId
    c.CustomOperationIds(apiDesc => { return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null; });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

ConfigureJwt(builder.Services);

ConfigureScheduler(builder.Services);

builder.Services.AddSignalR();

builder.Logging.AddFile("Logs/Backend-{Date}.txt");

var dbContext = builder.Services.BuildServiceProvider().GetService<DatabaseContext>();
dbContext.Database.Migrate();

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse().ToList().ForEach(description =>
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        });
    });
}

app.UseCors(options => options.WithOrigins("*").AllowAnyMethod());

app.UseHttpsRedirection();

app.UseRouting();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware(typeof(ExceptionHandler));

app.MapControllers();
app.MapGraphQL();
app.MapHub<NotificationHub>("/Notifications");

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