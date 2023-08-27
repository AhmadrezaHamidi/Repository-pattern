using DataAccess.Api;
using DataAccess.Api.Domain;
using DatatAccess.Ef;
using DatatAccess.Ef.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API V1",

        Description = "UserName = Administrator@localhost" + "" + " Password = Administrator1!"
    });
 });


var connectionString = builder.Configuration.GetConnectionString("Local");

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));


//builder.Services.AddScoped<IUnitOfWork>(_ => new UnitOfWork(new MyDbContext(), new SqlConnection(connectionString)));
builder.Services.AddScoped<IDbConnection>(x => new SqlConnection(connectionString));
builder.Services.AddScoped<DbContext, MyDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";
});

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
//app.UseContoller();
app.UseAuthorization();

//app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
