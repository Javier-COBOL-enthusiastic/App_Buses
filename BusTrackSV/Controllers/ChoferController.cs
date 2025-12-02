using BusTrackSV.Service;
using Microsoft.AspNetCore.Authorization;
using Data.Repositories;
using BusTrackSV.Models;
using System.Formats.Asn1;

namespace BusTrackSV.API;

public static class ChoferController
{
    public static void MapChoferController(this WebApplication app)
    {
        var group = app.MapGroup("/choferes");
        group.RequireAuthorization();
        
        group.MapGet("/busruta/", async (HttpContext ctx, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var roleIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "roleId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            if(roleIdClaim == null || roleIdClaim.Value != "2")
                return Results.Unauthorized();

            try
            {                                
                var choferID = int.Parse(userIdClaim.Value);
                var res = await Task.Run(() => choferService.GetBusRutaInfo(choferID));
                return Results.Ok(res);
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (NullValue)
            {
                return Results.NotFound(new { message = "Chofer no encontrado." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/get/{id:int}", async (HttpContext ctx, int id, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            var userId = userIdClaim.Value;
            try
            {
                var res = await Task.Run(() => choferService.GetChofer(int.Parse(userId), id));
                return Results.Ok(res);
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (NullValue)
            {
                return Results.NotFound(new { message = "Chofer no encontrado." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/get", async (HttpContext ctx, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            var userId = userIdClaim.Value;
            try
            {
                var res = await Task.Run(() => choferService.Get(int.Parse(userId)));
                return Results.Ok(res);
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
        
        group.MapPost("/add", async (HttpContext ctx, ChoferRegistroDTO nch, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            var userId = userIdClaim.Value;
            try
            {
                await Task.Run(() => choferService.AddChofer(int.Parse(userId), nch));
                

                return Results.Created($"/choferes/get/", new { message = "Chofer registrado." });
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (NullValue)
            {
                return Results.NotFound(new { message = "Bus referenciado no existe." });
            }
            catch (Exception ex) when (ex is CamposRequeridosException)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapPut("/update", async (HttpContext ctx, Chofer nchofer, ChoferService choferService) =>
        {
            var userIDclaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIDclaim == null)
                return Results.Unauthorized();

            var userID = userIDclaim.Value;
            try
            {
                await Task.Run(() => choferService.UpdateChofer(int.Parse(userID), nchofer));
                return Results.NoContent();
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (Exception ex) when (ex is NullValue || ex is CamposRequeridosException)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }            
        });

        group.MapDelete("/delete/{id:int}", async (HttpContext ctx, int id, ChoferService choferService) =>
        {
            var userIDclaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIDclaim == null)
                return Results.Unauthorized();

            var userID = userIDclaim.Value;
            try
            {
                await Task.Run(() => choferService.DeleteChofer(int.Parse(userID), id));
                return Results.NoContent();
            }
            catch (UserInvalidado)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}