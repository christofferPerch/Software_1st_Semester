using HeartDisease.Data;
using HeartDisease.DataAccess;
using HeartDisease.Services;
using HeartDisease.Services.Webshop;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IDataAccess, SqlDataAccess>(sp =>
    new SqlDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddScoped<IMongoDataAccess, MongoDataAccess>(sp => {
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoDataAccess(settings.ConnectionString, settings.DatabaseName);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<PredictionService>();

builder.Services.AddScoped<KnowledgeBaseService>();

builder.Services.AddScoped<ChatBotService>();

builder.Services.AddScoped<ChatBotHistoryService>();

builder.Services.AddScoped<DatabaseManagementService>();

// Webshop
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<OrderHistoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<SideEffectService>();
builder.Services.AddScoped<ManufacturerService>();
builder.Services.AddScoped<OrderManagementService>();


var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Webshop}/{action=Index}/{id?}");
app.MapRazorPages();

await EnsureRolesAsync(app);
await EnsureUsersAsync(app);

app.Run();

async Task EnsureRolesAsync(WebApplication app) {
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

async Task EnsureUsersAsync(WebApplication app) {
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "admin@admin.com";
    string password = "Admin.123";
    var user = await userManager.FindByEmailAsync(email);
    if (user == null) {
        user = new IdentityUser { UserName = email, Email = email };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }
}
