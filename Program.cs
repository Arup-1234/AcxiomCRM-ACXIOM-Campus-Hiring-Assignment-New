using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AcxiomCRM.Infrastructure.AuditService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(options => {
    options.AddPolicy("CrmRead", p => p.RequireRole("Admin", "Manager", "Sales Executive"));
    options.AddPolicy("CrmManage", p => p.RequireRole("Admin", "Manager", "Sales Executive"));
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", p => p.RequireRole("Admin", "Manager"));
});
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); } else { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapRazorPages();
// Create the database and seed roles/admin for a first local run. For production, use EF migrations.
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
    var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Manager", "Sales Executive" })
        if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));
    var users = services.GetRequiredService<UserManager<IdentityUser>>();
    var email = app.Configuration["SeedAdmin:Email"] ?? "admin@acxiomcrm.local";
    var password = app.Configuration["SeedAdmin:Password"] ?? "ChangeMe123!";
    var admin = await users.FindByEmailAsync(email);
    if (admin is null) {
        admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true, LockoutEnabled = true };
        var result = await users.CreateAsync(admin, password);
        if (result.Succeeded) await users.AddToRoleAsync(admin, "Admin");
        else app.Logger.LogError("Could not seed admin: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
    } else if (!await users.IsInRoleAsync(admin, "Admin")) await users.AddToRoleAsync(admin, "Admin");
}
app.Run();
