using loginform.Models;
using loginform.Services;
using MySqlConnector;

namespace loginform.Data;

public sealed class MySqlUserRepository(IConfiguration configuration, PasswordHashService passwordHashService) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection.");

    public async Task EnsureCreatedAsync()
    {
        await EnsureDatabaseCreatedAsync();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS users (
                    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    username VARCHAR(100) NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
                    first_name VARCHAR(100) NOT NULL,
                    last_name VARCHAR(100) NULL,
                    email VARCHAR(255) NULL,
                    role VARCHAR(20) NOT NULL DEFAULT 'Standard',
                    is_active TINYINT(1) NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NULL
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        await using (var countCommand = connection.CreateCommand())
        {
            countCommand.CommandText = "SELECT COUNT(*) FROM users;";
            var count = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            if (count > 0)
            {
                return;
            }
        }

        await using (var seedCommand = connection.CreateCommand())
        {
            seedCommand.CommandText = """
                INSERT INTO users (username, password_hash, first_name, last_name, email, role, is_active)
                VALUES (@username, @password_hash, @first_name, @last_name, @email, @role, 1);
                """;
            seedCommand.Parameters.AddWithValue("@username", "admin");
            seedCommand.Parameters.AddWithValue("@password_hash", passwordHashService.HashPassword("Admin@123"));
            seedCommand.Parameters.AddWithValue("@first_name", "Admin");
            seedCommand.Parameters.AddWithValue("@last_name", "User");
            seedCommand.Parameters.AddWithValue("@email", "admin@example.com");
            seedCommand.Parameters.AddWithValue("@role", UserRoles.Admin);
            await seedCommand.ExecuteNonQueryAsync();
        }
    }

    private async Task EnsureDatabaseCreatedAsync()
    {
        var builder = new MySqlConnectionStringBuilder(_connectionString);
        if (string.IsNullOrWhiteSpace(builder.Database))
        {
            return;
        }

        var databaseName = builder.Database;
        builder.Database = string.Empty;

        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{EscapeIdentifier(databaseName)}`;";
        await command.ExecuteNonQueryAsync();
    }

    private static string EscapeIdentifier(string value)
    {
        return value.Replace("`", "``", StringComparison.Ordinal);
    }

    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, username, password_hash, first_name, last_name, email, role, is_active, created_at, updated_at
            FROM users
            WHERE username = @username
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("@username", username);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<AppUser?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, username, password_hash, first_name, last_name, email, role, is_active, created_at, updated_at
            FROM users
            WHERE id = @id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync()
    {
        var users = new List<AppUser>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, username, password_hash, first_name, last_name, email, role, is_active, created_at, updated_at
            FROM users
            ORDER BY id;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
        }

        return users;
    }

    public async Task CreateAsync(AppUser user)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO users (username, password_hash, first_name, last_name, email, role, is_active)
            VALUES (@username, @password_hash, @first_name, @last_name, @email, @role, @is_active);
            """;
        AddUserParameters(command, user);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(AppUser user, string? newPasswordHash)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE users
            SET username = @username,
                first_name = @first_name,
                last_name = @last_name,
                email = @email,
                role = @role,
                is_active = @is_active,
                password_hash = COALESCE(@new_password_hash, password_hash),
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @id;
            """;
        AddUserParameters(command, user);
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@new_password_hash", (object?)newPasswordHash ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static void AddUserParameters(MySqlCommand command, AppUser user)
    {
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@first_name", user.FirstName);
        command.Parameters.AddWithValue("@last_name", (object?)user.LastName ?? DBNull.Value);
        command.Parameters.AddWithValue("@email", (object?)user.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@role", user.Role);
        command.Parameters.AddWithValue("@is_active", user.IsActive);
    }

    private static AppUser MapUser(MySqlDataReader reader)
    {
        return new AppUser
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString(reader.GetOrdinal("last_name")),
            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
            Role = reader.GetString(reader.GetOrdinal("role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
