using System;
using Microsoft.Data.SqlClient;

public static class RdsConnectionHelper
{
    public static string? GetConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("RDS_HOSTNAME");
        var port = Environment.GetEnvironmentVariable("RDS_PORT") ?? "1433";
        var database = "Room2Room"; //Environment.GetEnvironmentVariable("RDS_DB_NAME");
        var username = Environment.GetEnvironmentVariable("RDS_USERNAME");
        var password = Environment.GetEnvironmentVariable("RDS_PASSWORD");

        if (
            string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(database)
            || string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(password)
        )
        {
            return null;
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{port}",
            InitialCatalog = database,
            UserID = username,
            Password = password,
            Encrypt = true,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }
}
