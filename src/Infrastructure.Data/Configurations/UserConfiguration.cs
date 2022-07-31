using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.Reviews)
                .WithOne(r => r.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReadList)
                .WithOne(r => r.Owner)
                .HasForeignKey<ReadList>(x => x.UserId);
        }
    }
}
