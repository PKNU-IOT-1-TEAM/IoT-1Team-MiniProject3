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
            // Data에서 만든 ApplicationDbContext를 사용하겠다는 설정 추가
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                // appsettings.json ConnectionString 내의 연결 문자열 할당
                //< ApplicationDbContext >랑 ("DefaultConnection") 연결
                builder.Configuration.GetConnectionString("DefaultConnection"),
                // 연결 문자열로 DB의 서버 버전을 자동으로 가져올 것
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));
            // 2. ASP.NET Identity - ASP.NET 계정을 위한 서비스 설정
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 비밀번호 정책 변경 설정
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Custom Password Policy // 실무에서는 아래와 같이 변경하면 안됨
                options.Password.RequireDigit = false; // 영문자 필요 여부
                options.Password.RequireLowercase = false; // 소문자 필요 여부
                options.Password.RequireUppercase = false; // 대문자 필요 여부
                options.Password.RequireNonAlphanumeric = false; // 특수문자 필요 여부
                options.Password.RequiredLength = 4; // 최소 글자 수
                options.Password.RequiredUniqueChars = 0; // 암호 고유문자 개수
            });
            // 권한을 위한 설정 추가
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