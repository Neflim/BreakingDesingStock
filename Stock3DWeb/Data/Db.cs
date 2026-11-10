using Microsoft.Data.SqlClient;

namespace Stock3DWeb.Data;

/// <summary>Fábrica de conexiones. La cadena vive en appsettings.json → ConnectionStrings:Default.</summary>
public class Db(IConfiguration config)
{
    private readonly string _cadena = config.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:Default en appsettings.json.");

    public SqlConnection Crear() => new(_cadena);
}
