using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion
{
    public static class IQueryableExtensionsPaginado
    {
        public static async Task<PaginadoResult<T>> ObtenerPaginado<T>(
            this IQueryable<T> query, int pagina, int tamañoPagina)
        {
            var result = new PaginadoResult<T>();
            result.PaginaActual = pagina;
            result.TamañoPagina = tamañoPagina;
            result.TotalElementos = await query.CountAsync();    
            result.TotalPaginas = (int)Math.Ceiling(result.TotalElementos / (double)tamañoPagina);
            
            //omite los elementos de la pagina anterior 
            var ItemsOmitidos = (pagina - 1) * tamañoPagina;
            result.Items = await query.Skip(ItemsOmitidos).Take(tamañoPagina).ToListAsync();

            return result;
        }
    }
}
