using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Middleware;
using Dashboard_MilkStore.Services.Auth;
using Dashboard_MilkStore.Services.Customer;
using Dashboard_MilkStore.Services.Order;
using Dashboard_MilkStore.Services.Product;
using Dashboard_MilkStore.Services.Statistics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services
builder.Services.AddScoped<CallAPI>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register StatisticsService with base URL
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
builder.Services.AddScoped<IStatisticsService>(provider =>
    new StatisticsService(
        apiBaseUrl,
        provider.GetRequiredService<IHttpContextAccessor>()
    ));

// Register Order Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();

// Register Customer Service
builder.Services.AddScoped<ICustomerService, CustomerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

// Use custom authentication middleware
app.UseAuthenticationMiddleware();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
