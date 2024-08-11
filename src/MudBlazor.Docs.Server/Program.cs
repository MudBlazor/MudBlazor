using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient<GitHubApiClient>();
builder.Services.TryAddDocsViewServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var client = new HttpClient { BaseAddress = new Uri($"{context!.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}") };

    return client;
});

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

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var notificationService = scope.ServiceProvider.GetService<INotificationService>();
    if (notificationService is InMemoryNotificationService inMemoryService)
    {
        inMemoryService.Preload();
    }
}

app.Run();
