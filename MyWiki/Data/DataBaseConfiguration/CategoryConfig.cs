using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWiki.Entity.CategoryEntity;

namespace MyWiki.Data.DataBaseConfiguration;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.CategoryName).HasMaxLength(255);
        builder
            .HasOne<Category>(c => c.ParentCategory)
            .WithMany(c => c.ChildrenCategories)
            .HasForeignKey(c => c.ParentCategoryId) // 明确指定外键为ParentCategoryId
            .OnDelete(DeleteBehavior.Restrict); //当尝试删除父分类时，如果存在子分类，则会阻止删除操作
    }
}
