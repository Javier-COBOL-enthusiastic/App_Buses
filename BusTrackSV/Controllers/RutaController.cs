using BusTrackSV.Service;
using Microsoft.AspNetCore.Authorization;
using BusTrackSV.Models;

namespace BusTrackSV.API;

public static class RutaController
{
    public static void MapRutaController(this WebApplication app)
    {
        var group = app.MapGroup("/ruta");
        group.RequireAuthorization();

        group.MapPost("/registrar/{idRuta:int}", async (int idRuta, HttpContext ctx, RutaService rutaService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Results.Unauthorized();

            try
            {
                await Task.Run(() => rutaService.RegistrarRutaExistente(int.Parse(userIdClaim.Value), idRuta));
                return Results.Ok(new { message = "Ruta vinculada exitosamente." });
            }
            catch (UserInvalidado) { return Results.Unauthorized(); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (NullValue ex)
            {
                // si es ID inválido -> 400, si es "no existe" -> 404
                return ex.Message?.ToLower().Contains("no existe") == true
                    ? Results.NotFound(new { message = ex.Message })
                    : Results.BadRequest(new { message = ex.Message });
            }
            catch (CamposRequeridosException ex) { return Results.BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapPost("/registrar", async (RegistrarRutaDTO dto, HttpContext ctx, RutaService rutaService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Results.Unauthorized();

            try
            {
                var idRuta = await Task.Run(() => rutaService.RegistrarRuta(int.Parse(userIdClaim.Value), dto.nuevaRuta, dto.coordenadas));
                return Results.Created($"/ruta/{idRuta}", new { id_ruta = idRuta });
            }
            catch (UserInvalidado) { return Results.Unauthorized(); }
            catch (CamposRequeridosException ex) { return Results.BadRequest(new { message = ex.Message }); }
            catch (NullValue ex) { return Results.BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapGet("/{idRuta:int}", async (int idRuta, HttpContext ctx, RutaService rutaService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Results.Unauthorized();

            try
            {
                var ruta = await Task.Run(() => rutaService.ObtenerRutaPorId(int.Parse(userIdClaim.Value), idRuta));
                return Results.Ok(ruta);
            }
            catch (UserInvalidado) { return Results.Unauthorized(); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (NullValue ex) { return Results.NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapGet("/coordenadas/{idRuta:int}", async (int idRuta, HttpContext ctx, RutaService rutaService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Results.Unauthorized();

            try
            {
                var coords = await Task.Run(() => rutaService.ObtenerCoordenadasPorRuta(int.Parse(userIdClaim.Value), idRuta));
                return Results.Ok(coords);
            }
            catch (UserInvalidado) { return Results.Unauthorized(); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (NullValue ex) { return Results.NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });

        group.MapDelete("/eliminar/{idRuta:int}", async (int idRuta, HttpContext ctx, RutaService rutaService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Results.Unauthorized();

            try
            {
                await Task.Run(() => rutaService.EliminarRuta(int.Parse(userIdClaim.Value), idRuta));
                return Results.NoContent();
            }
            catch (UserInvalidado) { return Results.Unauthorized(); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (NullValue ex) { return Results.NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return Results.Problem(ex.Message); }
        });
    }
}