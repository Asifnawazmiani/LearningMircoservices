namespace UserService.Infrastructure.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(r => new { r.UserId, r.Role });

        builder.Property(r => r.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}
