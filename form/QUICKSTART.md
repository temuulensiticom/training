# Quick Start Guide

## Start in 5 Steps:

### 1. Start Docker Databases
```bash
cd c:\Users\Dell\Documents\temuulen\form
docker-compose up -d
```
Wait 30-60 seconds for MySQL to start.

### 2. Install Dependencies
```bash
cd form
dotnet restore
```

### 3. Run Application
```bash
dotnet run
```

### 4. Open Browser
```
https://localhost:5001
```

### 5. Upload SQL File
- Click "Select File" and choose `sample.sql` from the project root
- Click "Upload & Execute"
- See results for both databases

---

## Testing Duplicate Error

1. Upload `sample.sql` twice
2. Second upload will be renamed to `sample_1.sql`
3. You'll see duplicate key errors for users (expected)
4. Error #1062 will be logged

---

## Test Database Connection

```bash
# Database 1 (Port 3308)
mysql -h localhost -P 3308 -u root -p
# Enter password: root123
# Then: SHOW DATABASES;

# Database 2 (Port 3309)
mysql -h localhost -P 3309 -u root -p
# Enter password: root123
```

---

## Stop Services

```bash
docker-compose down
```

---

## Custom SQL File Format

Your SQL file can contain:

```sql
-- Comments are removed automatically
CREATE TABLE my_table (id INT PRIMARY KEY);
INSERT INTO my_table VALUES (1);

/* Multi-line comments
   are also removed */
DELIMITER //
CREATE PROCEDURE my_proc()
BEGIN
    SELECT * FROM my_table;
END//
DELIMITER ;
```

Separate queries with **semicolons (;)**
