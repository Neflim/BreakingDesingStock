using Microsoft.AspNetCore.Components;

namespace Stock3DWeb.Components.Tabs;

/// <summary>Base de las pestañas: alerta de estado y ejecución de acciones con manejo uniforme de errores.</summary>
public abstract class TabBase : ComponentBase
{
    protected string? Mensaje { get; set; }
    protected bool EsError { get; private set; }

    protected void CerrarMensaje() => Mensaje = null;

    /// <summary>Ejecuta la acción y muestra <paramref name="exito"/> (salvo que la acción fije otro Mensaje) o el error.</summary>
    protected async Task Ejecutar(Func<Task> accion, string? exito = null)
    {
        Mensaje = null;
        EsError = false;
        try
        {
            await accion();
            Mensaje ??= exito;
        }
        catch (Exception ex)
        {
            Mensaje = ex.Message;
            EsError = true;
        }
    }
}
