using GymSys.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasColumnType("VARCHAR")
                .HasMaxLength(11);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email LIKE '%_@_%._%'");
                tb.HasCheckConstraint("PhoneCheck",
                    "Phone NOT LIKE '%[^0-9]%' AND LEN(Phone) = 11 AND LEFT(Phone, 3) IN ('010', '011', '012', '015')"
                    );
            });

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(x => x.Street).HasColumnName("Street").HasColumnType("VARCHAR").HasMaxLength(30);
                address.Property(x => x.City).HasColumnName("City").HasColumnType("VARCHAR").HasMaxLength(30);
            });
        }
    }
}
