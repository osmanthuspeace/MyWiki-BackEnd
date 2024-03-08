using Microsoft.EntityFrameworkCore;
using MyWiki.Data.DataBaseConfiguration;
using MyWiki.Entity.CategoryEntity;
using MyWiki.Entity.EntryEntity;
using MyWiki.Entity.PictureEntity;
using MyWiki.Entity.RoleEntity;
using MyWiki.Entity.TagEntity;
using MyWiki.Entity.UserEntity;

namespace MyWiki.Data;

public class WikiContext(DbContextOptions<WikiContext> options) : DbContext(options)
{
    public DbSet<Entry> Entries { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Picture> Pictures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new EntryConfig());
        modelBuilder.ApplyConfiguration(new CategoryConfig());
        base.OnModelCreating(modelBuilder);
    }
}
