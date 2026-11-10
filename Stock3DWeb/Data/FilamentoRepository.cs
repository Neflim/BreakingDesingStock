using Dapper;
using Stock3DWeb.Models;

namespace Stock3DWeb.Data;

public class FilamentoRepository(Db db)
{
    public async Task<List<Filamento>> ListarActivosAsync()
    {
        using var cn = db.Crear();
        var filamentos = await cn.QueryAsync<Filamento>(
            "SELECT Id, Nombre, Material, Color, PrecioPorKg, Activo FROM dbo.Filamentos WHERE Activo = 1 ORDER BY Nombre");
        return filamentos.AsList();
    }

    public async Task GuardarAsync(Filamento f)
    {
        if (f.PrecioPorKg <= 0)
            throw new ArgumentException("El precio por Kg debe ser un número positivo mayor a cero.");

        const string insertar = """
            INSERT INTO dbo.Filamentos (Nombre, Material, Color, PrecioPorKg, Activo)
            VALUES (@Nombre, @Material, @Color, @PrecioPorKg, 1)
            """;
        const string actualizar = """
            UPDATE dbo.Filamentos
            SET Nombre = @Nombre, Material = @Material, Color = @Color, PrecioPorKg = @PrecioPorKg
            WHERE Id = @Id
            """;

        using var cn = db.Crear();
        await cn.ExecuteAsync(f.Id == 0 ? insertar : actualizar,
            new { f.Id, Nombre = f.Nombre.Trim(), Material = f.Material.Trim(), Color = f.Color.Trim(), f.PrecioPorKg });
    }

    public async Task BajaAsync(int id)
    {
        using var cn = db.Crear();
        await cn.ExecuteAsync("UPDATE dbo.Filamentos SET Activo = 0 WHERE Id = @id", new { id });
    }

    /// <summary>Productos activos cuyo costo cambia al modificar el precio de este filamento.</summary>
    public async Task<int> ContarProductosAfectadosAsync(int id)
    {
        using var cn = db.Crear();
        return await cn.ExecuteScalarAsync<int>("""
            SELECT COUNT(DISTINCT pf.ProductoId)
            FROM dbo.ProductoFilamentos pf
            JOIN dbo.Productos p ON p.Id = pf.ProductoId
            WHERE pf.FilamentoId = @id AND p.Activo = 1
            """, new { id });
    }
}
