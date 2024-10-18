using HotelAPI.Models;
using HotelFull.Server.Models;
using Microsoft.EntityFrameworkCore;

public partial class HotelContext : DbContext
{
    public HotelContext(DbContextOptions<HotelContext> options) : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisement { get; set; }


    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<HotelInfo> HotelInfo { get; set; }

    public virtual DbSet<Image> Image { get; set; }

    public virtual DbSet<ImageType> ImageType { get; set; }

    public virtual DbSet<Member> Member { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<ProductOrder> ProductOrder { get; set; }

    public virtual DbSet<ProductType> ProductType { get; set; }

    public virtual DbSet<Room> Room { get; set; }

    public virtual DbSet<RoomBooking> RoomBooking { get; set; }

    public virtual DbSet<RoomType> RoomType { get; set; }
    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<ProductOrderItem> ProductOrderItem { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdvertisementID).HasName("PK__Advertis__C4C7F42D6CB86221");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.URL).HasMaxLength(255);

        });

       

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeID).HasName("PK__Employee__7AD04FF13A491557");

            entity.Property(e => e.ClockInStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IDNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Position).HasMaxLength(100);

            entity.HasOne(d => d.Image).WithMany(p => p.Employee)
                .HasForeignKey(d => d.ImageID)
                .HasConstraintName("FK__Employee__ImageI__52593CB8");
        });

        modelBuilder.Entity<HotelInfo>(entity =>
        {
            entity.HasKey(e => e.HotelInfoID).HasName("PK__HotelInf__54385C7A560238A4");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValue("teste@exemplo.us");

            entity.Property(e => e.FullAddress)
                .HasMaxLength(255)
                .HasDefaultValue("Rua Inexistente, 2000");

            entity.Property(e => e.HotelName)
                .HasMaxLength(100)
                .HasDefaultValue("健行小飯店");

            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(8, 6)")
                .HasDefaultValue(-48.817993m);

            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(9, 6)")
                .HasDefaultValue(-15.930247m);

            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("3121286800");

            entity.Property(e => e.CheckInTime)
                .HasMaxLength(5)
                .HasDefaultValue("05:00");

            entity.Property(e => e.CheckOutTime)
                .HasMaxLength(5)
                .HasDefaultValue("17:00");
        });


        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageID).HasName("PK__Image__7516F4ECA7933EFB");

            entity.HasOne(d => d.ImageType).WithMany(p => p.Image)
                .HasForeignKey(d => d.ImageTypeID)
                .HasConstraintName("FK__Image__ImageType__5070F446");
        });

        modelBuilder.Entity<ImageType>(entity =>
        {
            entity.HasKey(e => e.ImageTypeID).HasName("PK__ImageTyp__B9E9EB969A0417B4");

            entity.Property(e => e.ImageTypeName)
                .HasMaxLength(100)
                .HasColumnName("ImageType");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberID).HasName("PK__Member__0CF04B38D0B45132");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false); // 限制 RefreshToken 的長度並設為非 Unicode
            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasColumnType("datetime"); // 將 RefreshTokenExpiryTime 設為 datetime 類型
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID).HasName("PK__Product__B40CC6ED4D04B220");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Image).WithMany(p => p.Product)
                .HasForeignKey(d => d.ImageID)
                .HasConstraintName("FK__Product__ImageID__59063A47");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Product)
                .HasForeignKey(d => d.ProductTypeID)
                .HasConstraintName("FK__Product__Product__5812160E");
        });

        modelBuilder.Entity<ProductOrder>(entity =>
        {
            entity.HasKey(e => e.OrderID)
                .HasName("PK__ProductO__C3905BAF0951D809");

            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime");

            entity.Property(e => e.PaymentStat) // 更新屬性名稱
                .HasConversion<int>() // 將列舉類型轉換為整數
                .IsRequired();

            entity.Property(e => e.Status) // 更新屬性名稱
                .HasConversion<int>() // 將列舉類型轉換為整數
                .IsRequired();

            entity.HasOne(d => d.Member)
                .WithMany(p => p.ProductOrder)
                .HasForeignKey(d => d.MemberID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductOr__Membe__59FA5E80");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeID).HasName("PK__ProductT__A1312F4E04AC0399");

            entity.Property(e => e.ProductTypeName)
                .HasMaxLength(100)
                .HasColumnName("ProductTypeName");
        });

       

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomID).HasName("PK__Room__32863919D72E2158");

            entity.Property(e => e.LastCleaned).HasColumnType("datetime");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Image).WithMany(p => p.Room)
                .HasForeignKey(d => d.ImageID)
                .HasConstraintName("FK__Room__ImageID__5441852A");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Room)
                .HasForeignKey(d => d.RoomTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Room__RoomTypeID__534D60F1");
        });

        modelBuilder.Entity<RoomBooking>(entity =>
        {
            entity.HasKey(e => e.BookingID).HasName("PK__RoomBook__73951ACD10204ED1");

            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Member).WithMany(p => p.RoomBooking)
                .HasForeignKey(d => d.MemberID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomBooki__Membe__5629CD9C");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomBooking)
                .HasForeignKey(d => d.RoomID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomBooki__RoomI__571DF1D5");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeID).HasName("PK__RoomType__BCC89611463B39DC");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TypeName).HasMaxLength(50);

            entity.HasOne(d => d.Image).WithMany(p => p.RoomType)
                .HasForeignKey(d => d.ImageID)
                .HasConstraintName("FK__RoomType__ImageI__5535A963");
        });

        modelBuilder.Entity<ProductOrderItem>()
       .HasKey(item => new { item.OrderID, item.ProductID });  

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
