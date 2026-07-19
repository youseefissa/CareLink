using CareLink.Dashboard.Components;
using CareLink.Dashboard.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Json;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient("CareLinkApiRaw", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "CareLinkDashboard.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapPost("/account/login", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    var httpClient = httpClientFactory.CreateClient("CareLinkApiRaw");

    var response = await httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

    if (!response.IsSuccessStatusCode)
    {
        context.Response.Redirect("/login?error=1");
        return;
    }

    var result = await response.Content.ReadFromJsonAsync<LoginApiResponse>();

    if (result is null)
    {
        context.Response.Redirect("/login?error=1");
        return;
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(ClaimTypes.Email, result.User.Email),
        new(ClaimTypes.Name, result.User.FullName),
        new(ClaimTypes.Role, ((RoleType)result.User.Role).ToString()),
        new("access_token", result.Token),
        new("refresh_token", result.RefreshToken)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    context.Response.Redirect("/");
})
.DisableAntiforgery();

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/login");
})
.DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

internal class LoginApiResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public LoginUserInfo User { get; set; } = null!;
}

internal class LoginUserInfo
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
}

internal enum RoleType
{
    Patient = 1,
    Caregiver = 2,
    Admin = 3
}