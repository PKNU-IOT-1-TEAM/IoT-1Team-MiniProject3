using Microsoft.EntityFrameworkCore;
using PSH_Parking_Assist_APP.Models;

namespace PSH_Parking_Assist_APP.Data
{
    public class ApplicationDbContext : DbContext// 1. ASP.NET Identity : DbContext -> IdentityDbContext 결국 DbContext 쓰는것하고 동일
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // User_Informations 라는 테이블을 만들기 위한 컬럼 수는 1개
        public DbSet<User_Information> User_Informations { get; set; }
        // 포트폴리오 DB로 관리하기 위한 모델  
    }
}