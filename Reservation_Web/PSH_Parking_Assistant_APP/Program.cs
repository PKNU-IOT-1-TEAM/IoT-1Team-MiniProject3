using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSH_Parking_Assist_APP.Data;

namespace PSH_Parking_Assistant_APP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Data���� ���� ApplicationDbContext�� ����ϰڴٴ� ���� �߰�
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                // appsettings.json ConnectionString ���� ���� ���ڿ� �Ҵ�
                //< ApplicationDbContext >�� ("DefaultConnection") ����
                builder.Configuration.GetConnectionString("DefaultConnection"),
                // ���� ���ڿ��� DB�� ���� ������ �ڵ����� ������ ��
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));
            // 2. ASP.NET Identity - ASP.NET ������ ���� ���� ����
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ��й�ȣ ��å ���� ����
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Custom Password Policy // �ǹ������� �Ʒ��� ���� �����ϸ� �ȵ�
                options.Password.RequireDigit = false; // ������ �ʿ� ����
                options.Password.RequireLowercase = false; // �ҹ��� �ʿ� ����
                options.Password.RequireUppercase = false; // �빮�� �ʿ� ����
                options.Password.RequireNonAlphanumeric = false; // Ư������ �ʿ� ����
                options.Password.RequiredLength = 4; // �ּ� ���� ��
                options.Password.RequiredUniqueChars = 0; // ��ȣ �������� ����
            });
            // ������ ���� ���� �߰�
            builder.Services.AddAuthorization();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}