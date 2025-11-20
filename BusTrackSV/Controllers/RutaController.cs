using BusTrackSV.Service;
using Microsoft.AspNetCore.Authorization;
using Data.Repositories;
using BusTrackSV.Models;


namespace BusTrackSV.API;

public static class RutaController
{
    public static void MapRutaController(this WebApplication app)
    {
        var group = app.MapGroup("/ruta");
        group.RequireAuthorization();

        group.MapPost("/registrar", (RegistrarRutaDTO registrarRutaDTO, RutaService rutaService) =>
        {
            try
            {
                int idRuta = rutaService.RegistrarRuta(registrarRutaDTO.nuevaRuta, registrarRutaDTO.coordenadas);
                return Results.Ok(new { message = "Ruta registrada exitosamente.", id_ruta = idRuta });
            }
            catch (Exception ex) when (ex is NullValue || ex is CamposRequeridosException)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/{idRuta:int}", (int idRuta, HttpContext ctx, RutaService rutaService) =>
        {
            try
            {
                var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
                if (userIdClaim == null)
                    return Results.Unauthorized();

                var userId = userIdClaim.Value;
                Ruta ruta = rutaService.ObtenerRutaPorId(int.Parse(userId), idRuta);
                return Results.Ok(ruta);
            }
            catch (Exception ex) when (ex is NullValue)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}