using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using ProductManagementRazorPages.Hubs;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("YOUR_SERVER"))
{
    throw new InvalidOperationException("Database connection string is missing or contains YOUR_SERVER placeholder. Please update appsettings.Development.json or set the Environment Variable on the server.");
}

builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<AccountDAO>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<SignalrServer>("/signalrServer");

app.Run();
