using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Author).IsRequired();

            builder.HasOne(x => x.BookImage)
               .WithOne(b => b.Book)
               .HasForeignKey<Photo>(x => x.BookId);
            
            builder.HasMany(x => x.Reviews)
                .WithOne(b => b.Book)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(b => b.Category)
            //    .WithMany(x => x.Books);
            //.OnDelete(DeleteBehavior.Cascade); //composition i.e delete category, delete books
        }
    }
}// seed books in db with filepath then later upload
