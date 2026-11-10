using Microsoft.AspNetCore.Components;
using Stock3DWeb.Data;
using Stock3DWeb.Models;

namespace Stock3DWeb.Components.Tabs;

public partial class TabProductos
{
    [Inject] private ProductoRepository ProductosRepo { get; set; } = default!;
    [Inject] private FilamentoRepository FilamentosRepo { get; set; } = default!;

    private List<Producto> _productos = [];
    private List<Filamento> _filamentos = [];
    private Producto _edit = new();
    private Producto? _tanda;
    private int _cantidadTanda = 5;

    protected override Task OnInitializedAsync() => Ejecutar(async () =>
    {
        await CargarAsync();
        Nuevo();
    });

    private async Task CargarAsync()
    {
        _productos = await ProductosRepo.ListarActivosAsync();
        _filamentos = await FilamentosRepo.ListarActivosAsync();
    }

    private void Nuevo() => _edit = new()
    {
        StockMinimo = 2,
        PrecioVenta = 2500,
        Filamentos = _filamentos.Take(1).Select(ProductoFilamento.Desde).ToList()
    };

    private void Editar(Producto p) => _edit = p.Copia();

    private Task GuardarAsync() => Ejecutar(async () =>
    {
        await ProductosRepo.GuardarAsync(_edit);
        await CargarAsync();
        Nuevo();
    }, "Producto guardado correctamente.");

    private Task DarBajaAsync(int id) => Ejecutar(async () =>
    {
        await ProductosRepo.BajaAsync(id);
        await CargarAsync();
    }, "Producto dado de baja.");

    private Task SumarTandaAsync() => Ejecutar(async () =>
    {
        await ProductosRepo.IngresarTandaAsync(_tanda!.Id, _cantidadTanda);
        _tanda = null;
        await CargarAsync();
    }, "Stock de la tanda sumado correctamente.");
}
