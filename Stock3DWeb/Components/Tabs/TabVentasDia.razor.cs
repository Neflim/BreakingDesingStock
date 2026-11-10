using Microsoft.AspNetCore.Components;
using Stock3DWeb.Data;
using Stock3DWeb.Models;

namespace Stock3DWeb.Components.Tabs;

public partial class TabVentasDia
{
    [Inject] private VentaRepository VentasRepo { get; set; } = default!;

    private DateTime _fecha = DateTime.Today;
    private List<StockDiaItem> _items = [];
    private bool _mostrarVenta;
    private int _productoId;
    private int _cantidad = 1;

    protected override Task OnInitializedAsync() => Recargar();

    private Task Recargar() => Ejecutar(CargarAsync);

    private async Task CargarAsync()
    {
        _items = await VentasRepo.StockDelDiaAsync(_fecha);
        if (_productoId == 0)
            _productoId = _items.FirstOrDefault()?.ProductoId ?? 0;
    }

    private Task ConfirmarAsync() => Ejecutar(async () =>
    {
        // Hoy: hora actual. Otro día: mediodía, para que caiga dentro de esa fecha.
        var momento = _fecha.Date == DateTime.Today ? DateTime.Now : _fecha.Date.AddHours(12);
        await VentasRepo.RegistrarAsync(_productoId, _cantidad, momento);
        _mostrarVenta = false;
        _cantidad = 1;
        await CargarAsync();
    }, "Venta registrada y stock descontado correctamente.");
}
