using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Infrastructure.DataSeeders;

public class UserDataSeeder
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserDataSeeder> _logger;

    public UserDataSeeder(UserDbContext context, ILogger<UserDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if data already exists
            if (await _context.Users.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("User data already exists. Skipping seed.");
                return;
            }

            _logger.LogInformation("Starting user data seeding...");

            var users = new List<UserEntity>
            {
                // Admin Users
                UserEntity.Create(
                    "John",
                    "Admin",
                    "admin@example.com",
                    "+1234567890"
                ),
                UserEntity.Create(
                    "Sarah",
                    "Manager",
                    "sarah.manager@example.com",
                    "+1234567891"
                ),

                // Regular Users
                UserEntity.Create(
                    "Alice",
                    "Johnson",
                    "alice.johnson@example.com",
                    "+1234567892"
                ),
                UserEntity.Create(
                    "Bob",
                    "Smith",
                    "bob.smith@example.com",
                    "+1234567893"
                ),
                UserEntity.Create(
                    "Charlie",
                    "Brown",
                    "charlie.brown@example.com",
                    "+1234567894"
                ),
                UserEntity.Create(
                    "Diana",
                    "Prince",
                    "diana.prince@example.com",
                    "+1234567895"
                ),
                UserEntity.Create(
                    "Edward",
                    "Norton",
                    "edward.norton@example.com",
                    "+1234567896"
                ),
                UserEntity.Create(
                    "Fiona",
                    "Green",
                    "fiona.green@example.com",
                    "+1234567897"
                ),
                UserEntity.Create(
                    "George",
                    "White",
                    "george.white@example.com",
                    "+1234567898"
                ),
                UserEntity.Create(
                    "Hannah",
                    "Black",
                    "hannah.black@example.com",
                    "+1234567899"
                )
            };

            // Verify some users (make them active)
            // Note: When both email and phone are verified, status automatically becomes Active
            users[0].VerifyEmail(); // Admin
            users[0].VerifyPhone(); // Status becomes Active automatically

            users[1].VerifyEmail(); // Manager
            users[1].VerifyPhone(); // Status becomes Active automatically

            users[2].VerifyEmail(); // Alice
            users[2].VerifyPhone(); // Status becomes Active automatically

            users[3].VerifyEmail(); // Bob
            users[3].VerifyPhone(); // Status becomes Active automatically

            users[4].VerifyEmail(); // Charlie
            users[4].VerifyPhone(); // Status becomes Active automatically

            // Add roles to users
            var adminUser = users[0];
            adminUser.AssignRole(UserRoleType.Admin);

            var managerUser = users[1];
            managerUser.AssignRole(UserRoleType.Manager);

            // Assign customer role to active users
            users[2].AssignRole(UserRoleType.Customer);
            users[3].AssignRole(UserRoleType.Customer);
            users[4].AssignRole(UserRoleType.Customer);

            // Clear domain events before saving
            foreach (var user in users)
            {
                user.ClearDomainEvents();
            }

            await _context.Users.AddRangeAsync(users, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully seeded {Count} users", users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding user data");
            throw;
        }
    }

    public async Task<List<Guid>> GetSeededUserIdsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OrderBy(u => u.CreatedAt)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }
}
