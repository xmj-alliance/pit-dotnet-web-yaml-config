# Web Yaml Config
Use yaml format App Settings and Secrets

## Dependencies
NetEscapades.Configuration.Yaml ^2.1.0

## Procedures

Install Dependencies

Convert all your `appsettings(.xxx).json` to `appsettings(.xxx).yaml`

Create a `secrets.yaml` following the example from `secrets.example.yaml`.

``` yaml
# secrets.yaml
  myDBConnection:
    username: rooooooot
    # ...
```

(In this cog, for demo purposes, the yaml file has been already created for you.)

Edit host builder to load config files to the server

``` csharp
// Program.cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.Sources.Clear();

            IHostEnvironment env = hostingContext.HostingEnvironment;

            config.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true)
                    .AddYamlFile($"appsettings.{env.EnvironmentName}.yaml",
                                optional: true, reloadOnChange: true)
                    .AddYamlFile("secrets.yaml", optional: true);

            config.AddEnvironmentVariables();

            if (args != null)
            {
                config.AddCommandLine(args);
            }
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            // ...
        });
```

App Settings will be working out of the box. But custom secrets are not.

We need to tell ASP.Net core server how to read that secret file.

Create a model for your secret

``` csharp
// MySecret.cs
public class MySecret
{
    public string Username { get; set; }
    // ...
}
```

Inject the secret to the config collection

``` csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // add secrets
    services.Configure<MySecret>(
        Configuration.GetSection("myDBConnection")
    );

    services.AddSingleton<MySecret>(sp =>
        sp.GetRequiredService<IOptions<MySecret>>().Value
    );
}
```

That's it.

To use the secret, just inject the `MySecret` class to wherever needed..

e.g.

``` csharp
// APIController.cs
public class APIController : ControllerBase
{
    private readonly MySecret appSecret;

    public APIController(MySecret appSecret)
    {
        this.appSecret = appSecret;
    }

    public string GetUsername()
    {
        return appSecret.Username;  // <--- You get the username in your secret
    }
}

```
