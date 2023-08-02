using CarReservationWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CarReservationWeb_F
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<Team1IotContext>(options => options.UseMySql(
				builder.Configuration.GetConnectionString("DefaultConnection"),
				ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
			));

			builder.Services.AddSession();

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

			app.UseRouting();

			app.UseSession();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}