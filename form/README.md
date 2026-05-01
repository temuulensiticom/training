# Database SQL File Executor

A C# ASP.NET Core application that allows uploading SQL files and executing them against two MySQL databases running in Docker containers.

## Features

- **Multi-Database Support**: Execute SQL queries against 2 separate MySQL databases simultaneously
- **File Upload**: Upload `.sql` or `.txt` files with SQL queries
- **Automatic Duplicate Handling**: Renames uploaded files if they already exist
- **Error Handling**: Handles duplicate key errors, connection errors, and syntax errors
- **Real-time Status**: Check database connection status before execution
- **Detailed Reporting**: View execution results, success/failure counts, and error messages
- **Procedure Support**: Execute stored procedures from SQL files
- **Comment Removal**: Automatically removes SQL comments before execution

## Prerequisites

- Docker and Docker Compose
- .NET 10.0 SDK or later
- MySQL Client (optional, for manual testing)

## Setup Instructions

### 1. Start Docker Containers

Navigate to the project root directory and run:

```bash
docker-compose up -d
```

This will start two MySQL containers:
- **Database 1**: Port 3308 (db1)
- **Database 2**: Port 3309 (db2)

Both databases use:
- Root Password: `root123`

Wait 30-60 seconds for MySQL to initialize before proceeding.

### 2. Install Dependencies

```bash
cd form
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The application will start at `https://localhost:5001` (or your configured port).

## Usage

### 1. Check Database Status
When you open the application, it automatically checks connections to both databases. The status badges will show:
- **Green (Connected)**: Database is ready
- **Red (Disconnected)**: Database is not accessible

### 2. Upload SQL File

1. Click "Select SQL or Text File" and choose a `.sql` or `.txt` file
2. The file can contain:
   - Multiple SQL statements separated by semicolons
   - CREATE TABLE statements
   - INSERT statements
   - Stored procedures
   - Comments (single-line `--` or multi-line `/* */`)

3. Select which databases to execute on:
   - ✓ Database 1 (Port 3308)
   - ✓ Database 2 (Port 3309)

4. Click "Upload & Execute"

### 3. View Results

After execution, you'll see:
- **Successful Queries**: Count of successfully executed queries
- **Failed Queries**: Count of failed queries
- **Duplicate Errors**: Count of duplicate key errors (error #1062)
- **Detailed Results**: Individual status for each query

## Example SQL File

Create a file called `sample.sql`:

```sql
-- Create table
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert data
INSERT INTO users (name, email) VALUES ('John Doe', 'john@example.com');
INSERT INTO users (name, email) VALUES ('Jane Smith', 'jane@example.com');

-- Create procedure
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS GetUserCount()
BEGIN
    SELECT COUNT(*) as user_count FROM users;
END//
DELIMITER ;
```

## Configuration

Edit `appsettings.json` to customize:

```json
{
  "ConnectionStrings": {
    "Database1": "Server=localhost;Port=3308;Database=db1;Uid=root;Pwd=root123;",
    "Database2": "Server=localhost;Port=3309;Database=db2;Uid=root;Pwd=root123;"
  },
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [ ".sql", ".txt" ]
  }
}
```

### Parameters:
- **MaxFileSize**: Maximum file size in bytes (default: 5MB)
- **AllowedExtensions**: File types allowed for upload

## Error Handling

The application handles:

| Error Type | Code | Description |
|-----------|------|-------------|
| Duplicate Entry | 1062 | Unique constraint violation |
| Unknown Column | 1054 | Invalid column reference |
| Connection Error | 0 | Cannot connect to database |
| General SQL Error | * | Syntax errors, permission denied, etc. |

Duplicate errors are shown as warnings (⚠) and don't stop execution.

## File Management

Uploaded files are stored in the `uploads/` folder in the application directory.

If a file with the same name exists:
- The file will be renamed to `filename_1.sql`, `filename_2.sql`, etc.
- You'll receive a duplicate file warning

## Troubleshooting

### "Cannot connect to Database"
1. Check if Docker containers are running: `docker ps`
2. Verify ports are correct (3308 and 3309)
3. Wait a few seconds after starting containers for MySQL to initialize

### "File type not allowed"
- Only `.sql` and `.txt` files are allowed
- Check the FileUpload:AllowedExtensions in appsettings.json

### "File size exceeds limit"
- Maximum file size is 5MB by default
- Change MaxFileSize in appsettings.json if needed

### "Duplicate entry error"
- The query tries to insert duplicate primary/unique key values
- This is normal for testing - modify your SQL to use INSERT IGNORE or UPDATE instead

## Database Access

### Connect to Databases Manually

```bash
# Database 1 (Port 3308)
mysql -h 127.0.0.1 -P 3308 -u root -p
# Password: root123

# Database 2 (Port 3309)
mysql -h 127.0.0.1 -P 3309 -u root -p
# Password: root123
```

## Stopping Containers

```bash
docker-compose down
```

To remove volumes as well:
```bash
docker-compose down -v
```

## License

MIT License
