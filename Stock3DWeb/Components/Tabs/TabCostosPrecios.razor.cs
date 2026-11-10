using Microsoft.AspNetCore.Components;
using Stock3DWeb.Data;
using Stock3DWeb.Models;

namespace Stock3DWeb.Components.Tabs;

public partial class TabCostosPrecios
{
    [Inject] private ProductoRepository ProductosRepo { get; set; } = default!;

    private List<Producto> _productos = [];
    private Producto? _desglose;
    private string _texto = "", _categoria = "", _tipo = "";   // "" = sin filtro

    protected override Task OnInitializedAsync() =>
        Ejecutar(async () => _productos = await ProductosRepo.ListarActivosAsync());

    private IEnumerable<string> Categorias() => _productos
        .Select(p => p.Categoria)
        .Where(c => !string.IsNullOrWhiteSpace(c))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Order();

    private List<Producto> Filtrar()
    {
        var texto = _texto.Trim();
        var cmp = StringComparison.OrdinalIgnoreCase;
        return _productos.Where(p =>
            (p.Nombre.Contains(texto, cmp) || p.Categoria.Contains(texto, cmp)) &&
            (_categoria == "" || p.Categoria.Equals(_categoria, cmp)) &&
            (_tipo == "" || p.TipoImpresion == _tipo)).ToList();
    }

    private void Limpiar() => (_texto, _categoria, _tipo) = ("", "", "");
}
