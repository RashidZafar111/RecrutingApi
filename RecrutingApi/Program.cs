using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using RecrutingApi.Authentication;
using RecrutingApi.DataAccessLayer;
using RecrutingApi.DBContext;
using RecrutingApi.Helper;
using RecrutingApi.Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMvc();
builder.Services.AddControllers().AddJsonOptions
    (x =>
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
builder.Services.AddHttpContextAccessor();
// Initilizing classes and and their dependencies so that they can be injected into various parts of
// the application
builder.Services.AddScoped<ApiKeyAuthenticaitonMiddleware>();
builder.Services.AddScoped<CandidateDAL>();
builder.Services.AddScoped<DocumentDAL>();
builder.Services.AddScoped<RecruiterDAL>();
builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<ResponseResult>();
builder.Services.AddScoped<Helpers>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
// Customizing problemDetails to include more information than the default response, such as
// additional error codes, correlation IDs, or custom fields
builder.Services.AddProblemDetails(
    options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance = $" {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} ";
            context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id);
        };
    });
// Creating extra layer of authentication and implement role-based access control (RBAC) in a .NET 8
// application, we can combine several techniques to ensure that all operations on controllers and
// classes are protected by appropriate authentication and role checks
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContext<RecrutingApiDBContext>(options =>

    options.UseInMemoryDatabase("RecrutingDB")

    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recruting API V1");
    });
}
app.UseSession();
app.UseMiddleware<ApiKeyAuthenticaitonMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.Lifetime.ApplicationStarted.Register(() =>
{
});

app.Run();