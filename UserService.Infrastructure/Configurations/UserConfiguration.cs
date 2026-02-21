namespace UserService.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Ignore(u => u.DomainEvents);

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt);
        builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(u => u.DeletedAt);

        builder.OwnsOne(u => u.FullName, fn =>
        {
            fn.Property(f => f.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            fn.Property(f => f.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
        });

        builder.OwnsOne(u => u.Email, e =>
        {
            e.Property(x => x.Value).HasColumnName("Email").HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Value).IsUnique();
        });

        builder.OwnsOne(u => u.MobileNo, m =>
        {
            m.Property(x => x.Value).HasColumnName("MobileNo").HasMaxLength(20).IsRequired();
        });

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.IsEmailVerified).IsRequired();
        builder.Property(u => u.IsPhoneVerified).IsRequired();
        builder.Property(u => u.LastLoginAt);

        builder.HasMany(u => u.Roles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Roles).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
