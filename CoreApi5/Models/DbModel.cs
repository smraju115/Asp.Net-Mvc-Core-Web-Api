using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreApi5.Models
{
    public enum DeviceType { Mobile=1, Tab}
    public class Device
    {
        public int DeviceId { get; set; }
        [Required, StringLength(50)]
        public string DeviceName { get; set; } = default!;
        [Required, EnumDataType(typeof(DeviceType))]
        public DeviceType DeviceType { get; set; }
        [Required, Column(TypeName ="date")]
        public DateTime? ReleaseDate { get; set; }
        [Required, Column(TypeName ="money")]
        public decimal? Price { get; set; }
        [Required, StringLength (50)]
        public string Picture {  get; set; }=default!;  
        public bool InStock { get; set; }
        public virtual ICollection<Spec> Specs { get; set; } = [];

    }
    public class Spec
    {
        public int SpecId { get; set; }
        [Required, StringLength(50)]
        public string SpecName { get; set; } = default!;
        [Required, StringLength(50)]
        public string SpecValue { get; set; } = default!;
        [Required, ForeignKey("Device")]
        public int DeviceId { get; set; }
        public virtual Device? Devices { get; set; }
    }
    public class DeviceDbContext(DbContextOptions<DeviceDbContext> options) : DbContext(options)
    {
        //public DeviceDbContext(DbContextOptions<DeviceDbContext> options) : base(options) {}
        public DbSet<Device> Devices { get; set; }
        public DbSet<Spec> Specs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().HasData(
                new Device { DeviceId=1, DeviceName="Sym", DeviceType=DeviceType.Mobile, ReleaseDate=new DateTime(2021,1,1), Price=12000, Picture="1.jpg", InStock=true}
                );
            modelBuilder.Entity<Spec>().HasData(
                new Spec { SpecId=1, DeviceId=1, SpecName="Ram", SpecValue="4GB"},
                  new Spec { SpecId = 2, DeviceId = 1, SpecName = "Storage", SpecValue = "64GB" }
                );
        }
    }
    
    
}
