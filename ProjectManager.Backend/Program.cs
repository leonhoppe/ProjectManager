using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectManager.Backend;
using ProjectManager.Backend.Options;
using ProjectManager.Backend.Security;
using ProjectManager.Backend.Apis;

var builder = WebApplication.CreateBuilder(args);

// Add options to the container
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOptionsFromConfiguration<GeneralOptions>(builder.Configuration);
builder.Services.AddOptionsFromConfiguration<ProxyOptions>(builder.Configuration);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITokenContext, TokenContext>();

builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<ITokenApi, TokenApi>();
builder.Services.AddScoped<IProjectApi, ProjectApi>();
builder.Services.AddScoped<IDockerApi, DockerApi>();
builder.Services.AddScoped<IProxyApi, ProxyApi>();

builder.Services.AddCors();
builder.Services.AddCustomAuthentication(true);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    options => options
        .WithOrigins(app.Configuration.GetSection("Frontend").Get<string>() ?? "")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
);

app.UseAuthorization();

app.MapControllers();
app.MapGet("", async context => {
    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.BodyWriter.WriteAsync("Ok"u8.ToArray());
    await context.Response.BodyWriter.CompleteAsync();
});

app.Run();