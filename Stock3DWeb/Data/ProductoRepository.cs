using Dapper;
using Stock3DWeb.Models;

namespace Stock3DWeb.Data;

public class ProductoRepository(Db db)
{
    /// <summary>Productos activos con su receta (2 consultas en un solo viaje).</summary>
    public async Task<List<Producto>> ListarActivosAsync()
    {
        const string sql = """
            SELECT Id, Nombre, Categoria, TipoImpresion, StockActual, StockMinimo,
                   CostoImpresion, CostoAgregado, PrecioVenta, ISNULL(LinkStl, '') AS LinkStl, Activo
            FROM dbo.Productos
            WHERE Activo = 1
            ORDER BY Nombre;

            SELECT pf.Id, pf.ProductoId, pf.FilamentoId, f.Nombre AS FilamentoNombre,
                   f.PrecioPorKg AS PrecioPorKgFilamento, pf.GramosUtilizados, pf.GramosDesperdicio
            FROM dbo.ProductoFilamentos pf
            JOIN dbo.Filamentos f ON f.Id = pf.FilamentoId
            JOIN dbo.Productos p ON p.Id = pf.ProductoId AND p.Activo = 1
            ORDER BY pf.Id;
            """;

        using var cn = db.Crear();
        using var multi = await cn.QueryMultipleAsync(sql);
        var productos = (await multi.ReadAsync<Producto>()).AsList();
        var recetas = (await multi.ReadAsync<ProductoFilamento>()).ToLookup(r => r.ProductoId);

        foreach (var p in productos)
            p.Filamentos = recetas[p.Id].ToList();
        return productos;
    }

    /// <summary>Alta o edición del producto y su receta, en una sola transacción.</summary>
    public async Task GuardarAsync(Producto p)
    {
        Validar(p);

        const string insertar = """
            INSERT INTO dbo.Productos (Nombre, Categoria, TipoImpresion, StockActual, StockMinimo,
                                       CostoImpresion, CostoAgregado, PrecioVenta, LinkStl, Activo)
            VALUES (@Nombre, @Categoria, @TipoImpresion, @StockActual, @StockMinimo,
                    @CostoImpresion, @CostoAgregado, @PrecioVenta, @LinkStl, 1);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        const string actualizar = """
            UPDATE dbo.Productos
            SET Nombre = @Nombre, Categoria = @Categoria, TipoImpresion = @TipoImpresion,
                StockActual = @StockActual, StockMinimo = @StockMinimo, CostoImpresion = @CostoImpresion,
                CostoAgregado = @CostoAgregado, PrecioVenta = @PrecioVenta, LinkStl = @LinkStl
            WHERE Id = @Id
            """;
        const string insertarFila = """
            INSERT INTO dbo.ProductoFilamentos (ProductoId, FilamentoId, GramosUtilizados, GramosDesperdicio)
            VALUES (@ProductoId, @FilamentoId, @GramosUtilizados, @GramosDesperdicio)
            """;

        var datos = new
        {
            p.Id, Nombre = p.Nombre.Trim(), Categoria = p.Categoria.Trim(), p.TipoImpresion,
            p.StockActual, p.StockMinimo, p.CostoImpresion, p.CostoAgregado, p.PrecioVenta,
            LinkStl = p.LinkStl.Trim()
        };

        using var cn = db.Crear();
        await cn.OpenAsync();
        await using var tx = await cn.BeginTransactionAsync();   // sin Commit => rollback automático

        var id = p.Id;
        if (id == 0)
            id = await cn.ExecuteScalarAsync<int>(insertar, datos, tx);
        else
        {
            await cn.ExecuteAsync(actualizar, datos, tx);
            await cn.ExecuteAsync("DELETE FROM dbo.ProductoFilamentos WHERE ProductoId = @id", new { id }, tx);
        }

        await cn.ExecuteAsync(insertarFila,
            p.Filamentos.Select(f => new { ProductoId = id, f.FilamentoId, f.GramosUtilizados, f.GramosDesperdicio }), tx);
        await tx.CommitAsync();
    }

    public async Task BajaAsync(int id)
    {
        using var cn = db.Crear();
        await cn.ExecuteAsync("UPDATE dbo.Productos SET Activo = 0 WHERE Id = @id", new { id });
    }

    public async Task IngresarTandaAsync(int productoId, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a ingresar debe ser mayor a cero.");

        using var cn = db.Crear();
        await cn.ExecuteAsync("UPDATE dbo.Productos SET StockActual = StockActual + @cantidad WHERE Id = @productoId",
            new { productoId, cantidad });
    }

    private static void Validar(Producto p)
    {
        if (string.IsNullOrWhiteSpace(p.Nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");
        if (p.PrecioVenta <= 0)
            throw new ArgumentException("El precio de venta debe ser mayor a cero.");
        if (p.CostoImpresion < 0 || p.CostoAgregado < 0)
            throw new ArgumentException("Los costos de impresión y agregado no pueden ser negativos.");
        if (p.Filamentos.Count == 0)
            throw new ArgumentException("Todo producto debe tener al menos un filamento asignado.");
        if (p.TipoImpresion == Producto.UnColor && p.Filamentos.Count > 1)
            throw new ArgumentException("Un producto de tipo 'Un color' solo puede tener 1 fila de filamento.");
        if (p.Filamentos.Any(f => f.FilamentoId <= 0))
            throw new ArgumentException("Debe seleccionar un filamento válido en todas las filas.");
        if (p.Filamentos.Any(f => f.GramosUtilizados <= 0))
            throw new ArgumentException("Los gramos utilizados deben ser mayores a cero.");
        if (p.Filamentos.Any(f => f.GramosDesperdicio < 0))
            throw new ArgumentException("Los gramos de desperdicio no pueden ser negativos.");
    }
}
