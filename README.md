# training
neew job that testing time projets developing git

## loginform setup

The `loginform` project is an ASP.NET Core Razor Pages app with MySQL login.

1. Install the .NET SDK that supports the project target framework.
2. Update `loginform/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=loginform;User ID=root;Password=@passmysql;ConnectionProtocol=Tcp;SslMode=None;Connection Timeout=10;"
}
```

3. Start MySQL, then run the app from `loginform`.

On first startup the app creates the `loginform` database, creates the `users` table, and inserts 17 mock accounts.

- Admin users: `admin`, `admin2`
- Admin password: `Admin@123`
- Standard users: `standard1` through `standard15`
- Standard password: `User@123`

Admin users can view, add, and edit all users. Standard users are sent to a welcome page that shows their first name from the database.



this is main error You may only use the Microsoft Visual Studio .NET/C/C++ Debugger (vsdbg) with
Visual Studio Code, Visual Studio or Visual Studio for Mac software to help you
develop and test your applications.
------------------------------------------------------------------------------
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Private.CoreLib.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/app/bin/Debug/net10.0/loginform.dll'. Symbols loaded.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Runtime.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/HotReloadAgent2/net10.0/Microsoft.Extensions.DotNetDeltaApplier.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Linq.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Runtime.Loader.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Console.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Private.Uri.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.IO.Pipes.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Memory.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Collections.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Runtime.InteropServices.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Collections.Concurrent.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Threading.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Net.Sockets.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Net.Primitives.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/Microsoft.Win32.Primitives.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Diagnostics.Tracing.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Threading.ThreadPool.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Threading.Thread.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Diagnostics.DiagnosticSource.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Hosting.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.DependencyInjection.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Diagnostics.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Http.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Routing.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.ComponentModel.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Features.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.Core.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authentication.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authentication.Cookies.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authorization.Policy.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Hosting.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.HttpsPolicy.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.StaticAssets.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.RazorPages.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Diagnostics.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Primitives.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Hosting.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Hosting.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.EnvironmentVariables.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.FileProviders.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.FileProviders.Physical.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.FileExtensions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Options.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Diagnostics.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.Binder.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.Json.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.IO.FileSystem.Watcher.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.ComponentModel.Primitives.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Text.Json.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Text.Encoding.Extensions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.IO.Pipelines.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Configuration.UserSecrets.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Razor.Runtime.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.Configuration.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Options.ConfigurationExtensions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.Console.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.Debug.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Logging.EventSource.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.DependencyInjection.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.Kestrel.Core.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.Kestrel.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.Kestrel.Transport.Quic.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Net.Quic.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.IIS.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Server.IISIntegration.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.FileProviders.Composite.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Text.Encodings.Web.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Numerics.Vectors.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Http.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Connections.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Hosting.Server.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.HostFiltering.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.HttpOverrides.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Routing.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.ObjectPool.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.ObjectModel.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.Razor.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authentication.Core.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authentication.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Security.Claims.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Authorization.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.DataAnnotations.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.ViewFeatures.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Linq.Expressions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.DataProtection.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.DataProtection.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Cryptography.Internal.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Antiforgery.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.WebEncoders.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Components.Endpoints.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Components.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Components.Authorization.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Components.Web.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.JSInterop.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Caching.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Caching.Memory.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Mvc.TagHelpers.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Razor.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Localization.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Http.Extensions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Reflection.Emit.Lightweight.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Reflection.Emit.ILGeneration.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Reflection.Primitives.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Net.Http.Headers.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.StaticFiles.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Http.Features.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.ComponentModel.TypeConverter.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Runtime.Numerics.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/Microsoft.Win32.Registry.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Xml.XDocument.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Private.Xml.Linq.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Private.Xml.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
warn: Microsoft.AspNetCore.DataProtection.Repositories.FileSystemXmlRepository[60]
      Storing keys in a directory '/home/app/.aspnet/DataProtection-Keys' that may not be persisted outside of the container. Protected data will be unavailable when container is destroyed. For more information go to https://aka.ms/aspnet/dataprotectionwarning
Microsoft.AspNetCore.DataProtection.Repositories.FileSystemXmlRepository: Warning: Storing keys in a directory '/home/app/.aspnet/DataProtection-Keys' that may not be persisted outside of the container. Protected data will be unavailable when container is destroyed. For more information go to https://aka.ms/aspnet/dataprotectionwarning
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Html.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/app/bin/Debug/net10.0/MySqlConnector.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Data.Common.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Metadata.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.FileSystemGlobbing.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.ComponentModel.Annotations.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Collections.Immutable.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Security.Cryptography.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.Extensions.Validation.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.7/Microsoft.AspNetCore.Diagnostics.Abstractions.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Collections.NonGeneric.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Runtime.Intrinsics.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
The thread 402 has exited with code 0 (0x0).
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Diagnostics.StackTrace.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.Reflection.Metadata.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
fail: Microsoft.Extensions.Hosting.Internal.Host[11]
      Hosting failed to start
      System.InvalidOperationException: Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.
      To generate a developer certificate run 'dotnet dev-certs https'. To trust the certificate (Windows and macOS only) run 'dotnet dev-certs https --trust'.
      For more information on configuring HTTPS see https://go.microsoft.com/fwlink/?linkid=848054.
         at Microsoft.AspNetCore.Hosting.ListenOptionsHttpsExtensions.UseHttps(ListenOptions listenOptions, Action`1 configureOptions)
         at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.AddressBinder.AddressesStrategy.BindAsync(AddressBindContext context, CancellationToken cancellationToken)
         at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.BindAsync(CancellationToken cancellationToken)
         at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.StartAsync[TContext](IHttpApplication`1 application, CancellationToken cancellationToken)
         at Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync(CancellationToken cancellationToken)
         at Microsoft.Extensions.Hosting.Internal.Host.<StartAsync>b__14_1(IHostedService service, CancellationToken token)
         at Microsoft.Extensions.Hosting.Internal.Host.ForeachService[T](IEnumerable`1 services, CancellationToken token, Boolean concurrent, Boolean abortOnFirstException, List`1 exceptions, Func`3 operation)
Microsoft.Extensions.Hosting.Internal.Host: Error: Hosting failed to start

System.InvalidOperationException: Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.
To generate a developer certificate run 'dotnet dev-certs https'. To trust the certificate (Windows and macOS only) run 'dotnet dev-certs https --trust'.
For more information on configuring HTTPS see https://go.microsoft.com/fwlink/?linkid=848054.
   at Microsoft.AspNetCore.Hosting.ListenOptionsHttpsExtensions.UseHttps(ListenOptions listenOptions, Action`1 configureOptions)
   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.AddressBinder.AddressesStrategy.BindAsync(AddressBindContext context, CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.BindAsync(CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.StartAsync[TContext](IHttpApplication`1 application, CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.Internal.Host.<StartAsync>b__14_1(IHostedService service, CancellationToken token)
   at Microsoft.Extensions.Hosting.Internal.Host.ForeachService[T](IEnumerable`1 services, CancellationToken token, Boolean concurrent, Boolean abortOnFirstException, List`1 exceptions, Func`3 operation)
The thread 399 has exited with code 0 (0x0).
Exception thrown: 'System.InvalidOperationException' in System.Private.CoreLib.dll
An unhandled exception of type 'System.InvalidOperationException' occurred in System.Private.CoreLib.dll: 'Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.
To generate a developer certificate run 'dotnet dev-certs https'. To trust the certificate (Windows and macOS only) run 'dotnet dev-certs https --trust'.
For more information on configuring HTTPS see https://go.microsoft.com/fwlink/?linkid=848054.'
Stack trace:
 >   at Microsoft.AspNetCore.Hosting.ListenOptionsHttpsExtensions.UseHttps(ListenOptions listenOptions, Action`1 configureOptions)
 >   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.AddressBinder.AddressesStrategy.<BindAsync>d__3.MoveNext()
 >   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.<BindAsync>d__31.MoveNext()
 >   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.<StartAsync>d__28`1.MoveNext()
 >   at Microsoft.AspNetCore.Hosting.GenericWebHostService.<StartAsync>d__40.MoveNext()
 >   at Microsoft.Extensions.Hosting.Internal.Host.<<StartAsync>b__14_1>d.MoveNext()
 >   at Microsoft.Extensions.Hosting.Internal.Host.<ForeachService>d__17`1.MoveNext()
 >   at Microsoft.Extensions.Hosting.Internal.Host.<StartAsync>d__14.MoveNext()
 >   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.<RunAsync>d__4.MoveNext()
 >   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.<RunAsync>d__4.MoveNext()
 >   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.Run(IHost host)
 >   at Program.Main(String[] args) in C:\Users\Dell\Documents\temuulen\loginform\Program.cs:line 46
The thread '.NET TP Worker' (394) has exited with code 0 (0x0).
The thread '.NET TP Worker' (397) has exited with code 0 (0x0).
Loaded '/usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.7/System.IO.MemoryMappedFiles.dll'. Skipped loading symbols. Module is optimized and the debugger option 'Just My Code' is enabled.
Unhandled exception. System.InvalidOperationException: Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.
To generate a developer certificate run 'dotnet dev-certs https'. To trust the certificate (Windows and macOS only) run 'dotnet dev-certs https --trust'.
For more information on configuring HTTPS see https://go.microsoft.com/fwlink/?linkid=848054.
   at Microsoft.AspNetCore.Hosting.ListenOptionsHttpsExtensions.UseHttps(ListenOptions listenOptions, Action`1 configureOptions)
   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.AddressBinder.AddressesStrategy.BindAsync(AddressBindContext context, CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.BindAsync(CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerImpl.StartAsync[TContext](IHttpApplication`1 application, CancellationToken cancellationToken)
   at Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.Internal.Host.<StartAsync>b__14_1(IHostedService service, CancellationToken token)
   at Microsoft.Extensions.Hosting.Internal.Host.ForeachService[T](IEnumerable`1 services, CancellationToken token, Boolean concurrent, Boolean abortOnFirstException, List`1 exceptions, Func`3 operation)
   at Microsoft.Extensions.Hosting.Internal.Host.StartAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.Run(IHost host)
   at Program.Main(String[] args) in C:\Users\Dell\Documents\temuulen\loginform\Program.cs:line 46
The program 'dotnet' has exited with code 0 (0x0).
