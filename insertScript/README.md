# MySQL Dual-DB SQL Executor

A C# WinForms tool that executes SQL files against **two MySQL instances simultaneously**,
handles stored procedures, triggers, functions, events, and gracefully reports duplicate/exists errors.

---

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ |
| Docker Desktop | Any recent |
| MySQL .NET Connector | (auto via NuGet) |

---

## 1 — Start the Docker databases

```bash
cd Docker
docker-compose up -d
```

This starts:
- **mysql_db1** → `localhost:3308`  root password: `pass1`
- **mysql_db2** → `localhost:3309`  root password: `pass2`

Wait ~10 seconds for MySQL to initialize.

---

## 2 — Build and run the app

```bash
dotnet restore
dotnet run
```

Or open in Visual Studio / Rider and press **F5**.

---

## 3 — Using the tool

### ⚙ Connections tab
- Fill in host/port/user/password/database for each DB
- Click **Test DB1 Connection** / **Test DB2 Connection** to verify

### 🔍 SQL Preview tab
- After browsing a file, all parsed statements are shown numbered with their type
- DELIMITER $$ blocks, procedures, triggers, functions are all correctly parsed

### ▶ Execute tab
1. Click **Browse…** → select a `.sql` or `.txt` file
2. Check which DBs to target (DB1, DB2, or both)
3. Click **▶ Execute**
4. Watch the live log with color-coded output:
   - 🟢 `✅ OK` — executed successfully
   - 🟠 `⚠ DUPLICATE` — object already exists (MySQL 1050/1304/1359 etc.)
   - 🔵 `ℹ IGNORED` — safe to ignore (e.g. DROP IF NOT EXISTS on missing table)
   - 🔴 `❌ ERROR` — real error

### 📊 Results tab
- Grid shows every statement result per database
- Click **Export Log…** to save the full log

---

## Error code classification

| Code | Meaning | Classification |
|------|---------|---------------|
| 1007 | Database already exists | DUPLICATE |
| 1050 | Table already exists | DUPLICATE |
| 1061 | Duplicate key name | DUPLICATE |
| 1062 | Duplicate entry (data) | DUPLICATE |
| 1304 | Procedure already exists | DUPLICATE |
| 1305 | Function already exists | DUPLICATE |
| 1359 | Trigger already exists | DUPLICATE |
| 1537 | Event already exists | DUPLICATE |
| 1008 | Can't drop DB (missing) | IGNORED |
| 1091 | Can't drop key (missing) | IGNORED |
| 1146 | Table doesn't exist | IGNORED |
| All others | Real errors | ❌ ERROR |

---

## Project structure

```
DbSyncTool/
├── Docker/
│   └── docker-compose.yml      ← Two MySQL instances
├── Forms/
│   └── MainForm.cs             ← Full WinForms UI
├── Models/
│   └── DbModels.cs             ← Config + Result models
├── Services/
│   ├── SqlParser.cs            ← Intelligent SQL statement splitter
│   └── DatabaseExecutor.cs    ← MySql.Data executor + error handling
├── Program.cs
├── DbSyncTool.csproj
└── sample_test.sql             ← Demo file with all MySQL object types
```
