# training
neew job that testing time projets developing git

## loginform setup

The `loginform` project is an ASP.NET Core Razor Pages app with MySQL login.

1. Install the .NET SDK that supports the project target framework.
2. Update `loginform/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=loginform;User ID=root;Password=@passmysql;Connection Timeout=10;"
}
```

3. Start MySQL, then run the app from `loginform`.

On first startup the app creates the `loginform` database, creates the `users` table, and inserts 17 mock accounts.

- Admin users: `admin`, `admin2`
- Admin password: `Admin@123`
- Standard users: `standard1` through `standard15`
- Standard password: `User@123`

Admin users can view, add, and edit all users. Standard users are sent to a welcome page that shows their first name from the database.
