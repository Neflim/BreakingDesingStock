namespace Stock3DWeb.Models;

/// <summary>Bobina / tipo de filamento.</summary>
public class Filamento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Material { get; set; } = "";
    public string Color { get; set; } = "";
    public decimal PrecioPorKg { get; set; }
    public bool Activo { get; set; } = true;

    public string DescripcionCombo => $"{Nombre} ({Material} - ${PrecioPorKg:N2}/kg)";

    public Filamento Copia() => (Filamento)MemberwiseClone();
}

/// <summary>Fila de la receta de un producto: gramos de un filamento (pieza + desperdicio).</summary>
public class ProductoFilamento
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int FilamentoId { get; set; }
    public string FilamentoNombre { get; set; } = "";
    public decimal PrecioPorKgFilamento { get; set; }
    public decimal GramosUtilizados { get; set; }
    public decimal GramosDesperdicio { get; set; }

    public decimal GramosTotales => GramosUtilizados + GramosDesperdicio;
    public decimal CostoSubtotal => CalculadoraCostos.CostoFila(this);

    public ProductoFilamento Copia() => (ProductoFilamento)MemberwiseClone();

    public static ProductoFilamento Desde(Filamento f) => new()
    {
        FilamentoId = f.Id,
        FilamentoNombre = f.Nombre,
        PrecioPorKgFilamento = f.PrecioPorKg,
        GramosUtilizados = 20,
        GramosDesperdicio = 2
    };
}

/// <summary>Producto impreso en 3D, de un color o multicolor.</summary>
public class Producto
{
    public const string UnColor = "Un color";
    public const string Multicolor = "Multicolor";

    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string TipoImpresion { get; set; } = UnColor;
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public decimal CostoImpresion { get; set; }   // electricidad y desgaste de la impresora
    public decimal CostoAgregado { get; set; }    // pintura, cadenas, LED, imanes, etc.
    public decimal PrecioVenta { get; set; }
    public string LinkStl { get; set; } = "";     // MakerWorld, Printables, Cults3D...
    public bool Activo { get; set; } = true;
    public List<ProductoFilamento> Filamentos { get; set; } = [];

    public string ResumenFilamentos => Filamentos.Count > 0 ? string.Join(", ", Filamentos.Select(f => f.FilamentoNombre)) : "Sin filamentos";
    public decimal GramosTotales => Filamentos.Sum(f => f.GramosTotales);
    public decimal CostoFilamentos => Filamentos.Sum(f => f.CostoSubtotal);
    public decimal CostoProduccion => CalculadoraCostos.CostoTotal(Filamentos, CostoImpresion, CostoAgregado);
    public decimal GananciaUnitaria => PrecioVenta - CostoProduccion;
    public decimal MargenPorcentaje => PrecioVenta > 0 ? GananciaUnitaria / PrecioVenta * 100 : 0;
    public decimal ValorStockCosto => StockActual * CostoProduccion;
    public decimal ValorStockVenta => StockActual * PrecioVenta;

    public bool TieneLink => !string.IsNullOrWhiteSpace(LinkStl);
    public string UrlStl =>
        Uri.TryCreate(LinkStl.Trim(), UriKind.Absolute, out var uri) && (uri.Scheme is "http" or "https")
            ? LinkStl.Trim()
            : $"https://{LinkStl.Trim()}";

    /// <summary>Copia editable (incluye la receta) para no modificar la lista mostrada.</summary>
    public Producto Copia()
    {
        var copia = (Producto)MemberwiseClone();
        copia.Filamentos = Filamentos.Select(f => f.Copia()).ToList();
        return copia;
    }
}

/// <summary>Única fuente de la fórmula de costos.</summary>
public static class CalculadoraCostos
{
    /// <summary>((GramosUso + GramosDesp) * PrecioKg) / 1000</summary>
    public static decimal CostoFila(ProductoFilamento f) =>
        (f.GramosUtilizados + f.GramosDesperdicio) * f.PrecioPorKgFilamento / 1000m;

    /// <summary>Costo Total = Σ CostoFila + CostoImpresion + CostoAgregado</summary>
    public static decimal CostoTotal(IEnumerable<ProductoFilamento> receta, decimal costoImpresion, decimal costoAgregado) =>
        receta.Sum(f => CostoFila(f)) + costoImpresion + costoAgregado;
}

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total => Cantidad * PrecioUnitario;
}

/// <summary>Fila de la pestaña "Ventas del Día".</summary>
public class StockDiaItem
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = "";
    public int StockInicialDia { get; set; }
    public int VendidoEnFecha { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public decimal MontoVendidoEnFecha { get; set; }

    public bool BajoStockMinimo => StockActual < StockMinimo;
}
