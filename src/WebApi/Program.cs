using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// chọn connection string: ưu tiên DATABASE_URL (Railway), fallback appsettings
string conn = builder.Configuration.GetConnectionString("Default")!;
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(dbUrl)) conn = BuildPgConnectionString(dbUrl);

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));

// Web
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// auto-migrate khi khởi động (hữu ích trên Railway)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// bind đúng cổng theo môi trường PaaS
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

static string BuildPgConnectionString(string databaseUrl)
{
    // chấp nhận dạng: postgres://user:pass@host:port/db?sslmode=require
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var csb = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
        Database = uri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };
    // copy query params nếu có
    var qp = System.Web.HttpUtility.ParseQueryString(uri.Query);
    if (qp["sslmode"] is string ssl) csb.SslMode = Enum.Parse<SslMode>(ssl, true);
    return csb.ConnectionString;
}
