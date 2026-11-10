using Dapper;
using Stock3DWeb.Models;

namespace Stock3DWeb.Data;

public class VentaRepository(Db db)
{
    /// <summary>Stock actual, stock a inicio del día y ventas de la fecha, por producto activo.</summary>
    public async Task<List<StockDiaItem>> StockDelDiaAsync(DateTime fecha)
    {
        const string sql = """
            SELECT p.Id AS ProductoId, p.Nombre AS ProductoNombre, p.StockActual, p.StockMinimo,
                   p.PrecioVenta AS PrecioVentaActual,
                   ISNULL(SUM(CASE WHEN v.Fecha < @fin THEN v.Cantidad END), 0) AS VendidoEnFecha,
                   ISNULL(SUM(CASE WHEN v.Fecha < @fin THEN v.Cantidad * v.PrecioUnitario END), 0) AS MontoVendidoEnFecha,
                   p.StockActual + ISNULL(SUM(v.Cantidad), 0) AS StockInicialDia
            FROM dbo.Productos p
            LEFT JOIN dbo.Ventas v ON v.ProductoId = p.Id AND v.Fecha >= @inicio
            WHERE p.Activo = 1
            GROUP BY p.Id, p.Nombre, p.StockActual, p.StockMinimo, p.PrecioVenta
            ORDER BY p.Nombre
            """;

        using var cn = db.Crear();
        var items = await cn.QueryAsync<StockDiaItem>(sql, new { inicio = fecha.Date, fin = fecha.Date.AddDays(1) });
        return items.AsList();
    }

    /// <summary>Descuenta stock y registra la venta al precio vigente, de forma atómica.</summary>
    public async Task RegistrarAsync(int productoId, int cantidad, DateTime fecha)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a vender debe ser mayor a cero.");

        using var cn = db.Crear();
        await cn.OpenAsync();
        await using var tx = await cn.BeginTransactionAsync();

        var producto = await cn.QuerySingleOrDefaultAsync<Producto>(
            "SELECT Nombre, StockActual, PrecioVenta FROM dbo.Productos WITH (UPDLOCK) WHERE Id = @productoId AND Activo = 1",
            new { productoId }, tx)
            ?? throw new InvalidOperationException("El producto seleccionado no existe o está inactivo.");

        if (cantidad > producto.StockActual)
            throw new InvalidOperationException(
                $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}, Solicitado: {cantidad}.");

        await cn.ExecuteAsync("UPDATE dbo.Productos SET StockActual = StockActual - @cantidad WHERE Id = @productoId",
            new { productoId, cantidad }, tx);
        await cn.ExecuteAsync("""
            INSERT INTO dbo.Ventas (Fecha, ProductoId, Cantidad, PrecioUnitario)
            VALUES (@Fecha, @ProductoId, @Cantidad, @PrecioUnitario)
            """,
            new Venta { Fecha = fecha, ProductoId = productoId, Cantidad = cantidad, PrecioUnitario = producto.PrecioVenta }, tx);
        await tx.CommitAsync();
    }
}
