using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Authentication (Cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Must be in this order
app.UseAuthentication();
app.UseAuthorization();

// ===== Employee CRUD routes (MUST be before default route) =====
app.MapControllerRoute(
    name: "employee-index",
    pattern: "dashboard/employees",
    defaults: new { controller = "Employee", action = "Index" });

app.MapControllerRoute(
    name: "employee-create",
    pattern: "dashboard/employees/create",
    defaults: new { controller = "Employee", action = "Create" });

app.MapControllerRoute(
    name: "employee-edit",
    pattern: "dashboard/employees/edit/{id}",
    defaults: new { controller = "Employee", action = "Edit" });

app.MapControllerRoute(
    name: "employee-delete",
    pattern: "dashboard/employees/delete/{id}",
    defaults: new { controller = "Employee", action = "Delete" });

// ===== Other custom routes =====
app.MapControllerRoute(
    name: "dashboard",
    pattern: "dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });


app.MapControllerRoute(
    name: "dashboard-bookings",
    pattern: "dashboard/bookings",
    defaults: new { controller = "Dashboard", action = "Bookings" });

app.MapControllerRoute(
    name: "approve-booking",
    pattern: "dashboard/bookings/approve",
    defaults: new { controller = "Dashboard", action = "ApproveBooking" });

app.MapControllerRoute(
    name: "reject-booking",
    pattern: "dashboard/bookings/reject",
    defaults: new { controller = "Dashboard", action = "RejectBooking" });

app.MapControllerRoute(
    name: "complete-booking",
    pattern: "dashboard/bookings/complete",
    defaults: new { controller = "Dashboard", action = "CompleteBooking" });

// ===== Public & Auth routes =====
app.MapControllerRoute(
    name: "booking",
    pattern: "booking/book",
    defaults: new { controller = "Booking", action = "Book" });

app.MapControllerRoute(
    name: "about",
    pattern: "about",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "services",
    pattern: "services",
    defaults: new { controller = "Home", action = "Services" });

app.MapControllerRoute(
    name: "network",
    pattern: "network",
    defaults: new { controller = "Home", action = "Network" });

app.MapControllerRoute(
    name: "careers",
    pattern: "careers",
    defaults: new { controller = "Home", action = "Careers" });

app.MapControllerRoute(
    name: "testimonials",
    pattern: "testimonials",
    defaults: new { controller = "Home", action = "Testimonials" });

app.MapControllerRoute(
    name: "contact",
    pattern: "contact",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Login", action = "Index" });

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Login", action = "Logout" });

// ===== Default route (LAST) =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Seeder.Initialize(dbContext);
}

app.Run();