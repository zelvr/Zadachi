using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
using Prometheus.SystemMetrics;
using Zadachi;
using Zadachi.Lib;
using Zadachi.Lib.Database;
using Zadachi.Lib.FileService;
using Zadachi.Lib.Logging;
using Zadachi.Lib.Telegram;
using Zadachi.Models;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ZadachiDbContext>(options => options.UseSqlite(connString));

builder.Services.AddScoped(typeof(IDatabaseService<>), typeof(DatabaseService<>));
builder.Services.AddScoped(typeof(IFileService), typeof(FileService));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 1;
}).AddEntityFrameworkStores<ZadachiDbContext>();

builder.Services.AddSingleton<UserStatistics>();
builder.Services.AddHostedService<BackgroundStatistics>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "local";
});

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddTransient<TelegramNotifier>();

var environment = builder.Environment;
builder.Logging.AddFile(Path.Combine(environment.WebRootPath, "files", "logging", "logs.txt"));
builder.Services.AddSystemMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseHttpMetrics();

app.MapMetrics();

app.Run();
