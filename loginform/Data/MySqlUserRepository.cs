using loginform.Models;
using loginform.Services;
using MySqlConnector;

namespace loginform.Data;

public sealed class MySqlUserRepository(IConfiguration configuration, PasswordHashService passwordHashService) : IUserRepository
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _isInitialized;

    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection.");

    public async Task EnsureCreatedAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await InitializationLock.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                return;
            }

            await EnsureCreatedCoreAsync();
            _isInitialized = true;
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    private async Task EnsureCreatedCoreAsync()
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

        var seedUsers = GetSeedUsers();
        foreach (var seedUser in seedUsers)
        {
            await using var seedCommand = connection.CreateCommand();
            seedCommand.CommandText = """
                INSERT INTO users (username, password_hash, first_name, last_name, email, role, is_active)
                VALUES (@username, @password_hash, @first_name, @last_name, @email, @role, @is_active)
                ON DUPLICATE KEY UPDATE username = username;
                """;
            seedCommand.Parameters.AddWithValue("@username", seedUser.Username);
            seedCommand.Parameters.AddWithValue("@password_hash", passwordHashService.HashPassword(seedUser.Password));
            seedCommand.Parameters.AddWithValue("@first_name", seedUser.FirstName);
            seedCommand.Parameters.AddWithValue("@last_name", seedUser.LastName);
            seedCommand.Parameters.AddWithValue("@email", seedUser.Email);
            seedCommand.Parameters.AddWithValue("@role", seedUser.Role);
            seedCommand.Parameters.AddWithValue("@is_active", seedUser.IsActive);
            await seedCommand.ExecuteNonQueryAsync();
        }
    }

    private static IReadOnlyList<SeedUser> GetSeedUsers()
    {
        return
        [
            new("admin", "Admin@123", "Admin", "User", "admin@example.com", UserRoles.Admin),
            new("admin2", "Admin@123", "Second", "Admin", "admin2@example.com", UserRoles.Admin),
            new("standard1", "User@123", "Ariun", "Bold", "standard1@example.com", UserRoles.Standard),
            new("standard2", "User@123", "Bat", "Erdene", "standard2@example.com", UserRoles.Standard),
            new("standard3", "User@123", "Bayar", "Suren", "standard3@example.com", UserRoles.Standard),
            new("standard4", "User@123", "Bilegt", "Gan", "standard4@example.com", UserRoles.Standard),
            new("standard5", "User@123", "Chin", "Zorig", "standard5@example.com", UserRoles.Standard),
            new("standard6", "User@123", "Davaa", "Tugs", "standard6@example.com", UserRoles.Standard),
            new("standard7", "User@123", "Enkh", "Munkh", "standard7@example.com", UserRoles.Standard),
            new("standard8", "User@123", "Ganzorig", "Dorj", "standard8@example.com", UserRoles.Standard),
            new("standard9", "User@123", "Khulan", "Baatar", "standard9@example.com", UserRoles.Standard),
            new("standard10", "User@123", "Maral", "Temuulen", "standard10@example.com", UserRoles.Standard),
            new("standard11", "User@123", "Naran", "Altan", "standard11@example.com", UserRoles.Standard),
            new("standard12", "User@123", "Oyun", "Sukh", "standard12@example.com", UserRoles.Standard),
            new("standard13", "User@123", "Saruul", "Purev", "standard13@example.com", UserRoles.Standard),
            new("standard14", "User@123", "Temuujin", "Erkhes", "standard14@example.com", UserRoles.Standard),
            new("standard15", "User@123", "Tselmeg", "Nomun", "standard15@example.com", UserRoles.Standard)
        ];
    }

    private sealed record SeedUser(
        string Username,
        string Password,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive = true);

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
        await EnsureCreatedAsync();

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
        await EnsureCreatedAsync();

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
        await EnsureCreatedAsync();

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
        await EnsureCreatedAsync();

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
        await EnsureCreatedAsync();

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
