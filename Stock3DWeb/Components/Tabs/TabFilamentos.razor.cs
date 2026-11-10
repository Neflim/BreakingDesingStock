using Microsoft.AspNetCore.Components;
using Stock3DWeb.Data;
using Stock3DWeb.Models;

namespace Stock3DWeb.Components.Tabs;

public partial class TabFilamentos
{
    [Inject] private FilamentoRepository FilamentosRepo { get; set; } = default!;

    private List<Filamento> _filamentos = [];
    private Filamento _edit = Vacio();

    protected override Task OnInitializedAsync() => Ejecutar(CargarAsync);

    private static Filamento Vacio() => new() { Material = "PLA", PrecioPorKg = 18000 };

    private async Task CargarAsync() => _filamentos = await FilamentosRepo.ListarActivosAsync();

    private void Nuevo() => _edit = Vacio();

    private void Editar(Filamento f) => _edit = f.Copia();

    private Task GuardarAsync() => Ejecutar(async () =>
    {
        var id = _edit.Id;
        var cambioPrecio = _filamentos.Any(f => f.Id == id && f.PrecioPorKg != _edit.PrecioPorKg);

        await FilamentosRepo.GuardarAsync(_edit);

        Mensaje = cambioPrecio
            ? $"Precio actualizado. Se recalcularon automáticamente los costos de {await FilamentosRepo.ContarProductosAfectadosAsync(id)} producto(s)."
            : "Filamento guardado correctamente.";

        await CargarAsync();
        Nuevo();
    });

    private Task DarBajaAsync(int id) => Ejecutar(async () =>
    {
        await FilamentosRepo.BajaAsync(id);
        await CargarAsync();
    }, "Filamento dado de baja.");
}
