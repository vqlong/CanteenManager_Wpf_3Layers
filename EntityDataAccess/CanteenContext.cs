using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Cryptography;
using System.Text;

namespace EntityDataAccess
{
    public class CanteenContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillInfo> BillInfos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Table> Tables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString);
            //optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            //optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(build =>
            {
                build.HasKey(a => a.Username);

                build.ToTable("Account");

                build.Property(a => a.Username)
                        .HasMaxLength(50)
                        .IsUnicode(false);

                build.Property(a => a.DisplayName)
                        .HasDefaultValueSql("N'CafeNo1'")
                        .HasMaxLength(50);

                build.Property(a => a.Password)
                        .HasDefaultValueSql("N'1411501582391102022111941545898146128230134207126393901341752432021821214658220108146'") //123456
                        .HasMaxLength(100)
                        .HasConversion(password => password.ToSHA256(), password => "******");

                build.Property(a => a.Type)
                        .HasDefaultValue(AccountType.Staff)
                        .HasConversion<int>();
            });

            modelBuilder.Entity<Bill>(builder =>
            {
                builder.ToTable("Bill")
                        .ToTable(tb=>tb.HasTrigger("UTG_UpdateBill"));

                builder.Property(b => b.DateCheckIn)
                        .HasDefaultValueSql("GETDATE()");

                builder.Property(b => b.Status)
                        .HasColumnName("BillStatus")
                        .HasDefaultValue(0);

                builder.Property(b => b.Discount)
                        .HasDefaultValue(0);

                builder.Property(b => b.TotalPrice)
                        .HasDefaultValue(0);

                builder.HasOne(b => b.Table)
                        .WithMany(t => t.Bills)
                        .HasForeignKey(b => b.TableId)
                        .OnDelete(DeleteBehavior.ClientCascade);

                builder.HasMany(b => b.BillInfos)
                        .WithOne(bi => bi.Bill)
                        .HasForeignKey(bi => bi.BillId)
                        .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<BillInfo>(builder =>
            {
                builder.ToTable("BillInfo")
                        .ToTable(tb => tb.HasTrigger("UTG_InsertBillInfo"))
                        .ToTable(tb => tb.HasTrigger("UTG_DeleteBillInfo"))
                        .ToTable(tb => tb.HasTrigger("UTG_UpdateBillInfo"));

                builder.Property(bi => bi.FoodCount)
                        .HasDefaultValue(0);
            });

            modelBuilder.Entity<Category>(builder =>
            {
                builder.ToTable("FoodCategory");

                builder.Property(c => c.Name)
                        .HasMaxLength(50)
                        .HasDefaultValueSql("N'Chưa đặt tên'");

                builder.Property(c => c.CategoryStatus)
                        .HasDefaultValue(UsingState.Serving)
                        .HasConversion<int>();
            });

            modelBuilder.Entity<Food>(builder =>
            {
                builder.ToTable("Food");

                builder.Property(f => f.Name)
                        .HasMaxLength(50)
                        .HasDefaultValueSql("N'Chưa đặt tên'");

                builder.Property(f => f.Price)
                        .HasDefaultValue(0);

                builder.Property(f => f.FoodStatus)
                        .HasDefaultValue(UsingState.Serving)
                        .HasConversion<int>();

                builder.HasOne(f => f.Category)
                        .WithMany(c => c.Foods)
                        .HasForeignKey(f => f.CategoryId)
                        .OnDelete(DeleteBehavior.ClientCascade);

                builder.HasMany(f => f.BillInfos)
                        .WithOne(bi => bi.Food)
                        .HasForeignKey(bi => bi.FoodId)
                        .OnDelete(DeleteBehavior.ClientCascade);

            });

            modelBuilder.Entity<Table>(builder =>
            {
                builder.ToTable("TableFood");

                builder.Property(t => t.Name)
                        .HasMaxLength(50)
                        .HasDefaultValueSql("N'Chưa đặt tên'");

                builder.Property(t => t.Status)
                        .HasColumnName("TableStatus")
                        .HasMaxLength(50)
                        .HasDefaultValueSql("N'Trống'");

                builder.Property(t => t.UsingState)
                        .HasDefaultValue(UsingState.Serving)
                        .HasConversion<int>();
            });

            modelBuilder.HasDbFunction(typeof(CanteenContext).GetMethod(nameof(ToUnsigned), new[] { typeof(string) }))
                        .HasName("UF_ConvertToUnsigned");

            base.OnModelCreating(modelBuilder);
        }

        public string ToUnsigned(string input) => throw new NotSupportedException();
    }

    public static class Extension
    {
        public static string ToSHA256(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var sha256bytes = SHA256.Create().ComputeHash(bytes);

            var output = "";
            foreach (var item in sha256bytes)
            {
                output += Convert.ToString(item);
            }

            return output;
        }
    }
}