using BusTrackSV.Service;
using Microsoft.AspNetCore.Authorization;
using Data.Repositories;
using BusTrackSV.Models;

namespace BusTrackSV.API;
public static class BusController
{
    
    public static void MapBusController(this WebApplication app)
    {
        var group = app.MapGroup("/buses");
        group.RequireAuthorization(); 
        
        group.MapGet("/ids", async (HttpContext context, BusService busService)  =>
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();            

            var userId = userIdClaim.Value;
            try
            {
                var res = await Task.Run(() => busService.getIds(int.Parse(userId)));                
                return Results.Json(res);
            }
            catch(UserInvalidado ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {                
                return Results.Problem(ex.Message);
            }                        
        }); 

        group.MapGet("/get/{id:int}", async (int id, BusService busService) =>
        {
            try
            {
                var res = await Task.Run(() => busService.getBus(id));
                return Results.Json(res);
            }
            catch(NullValue)
            {
                return Results.NoContent();
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapPost("/add", async (HttpContext ctx, BusRegistroDTO nbus, BusService busService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            var userId = userIdClaim.Value;
            try
            {
                await Task.Run(() => busService.AddBus(int.Parse(userId), nbus));
                return Results.Accepted();
            }
            catch(UserInvalidado ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch(Exception ex) when(
                ex is NullValue ||
                ex is RutaNoRegistrada ||
                ex is CamposRequeridosException
            )           
            {
                return Results.BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapPut("/update", async (HttpContext ctx, Bus bus, BusService busService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();
            var userId = userIdClaim.Value;

            try
            {
                await Task.Run(() => busService.PutBus(int.Parse(userId), bus));
                return Results.Accepted();
            }
            catch(UserInvalidado ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch(Exception ex) when(
                ex is NullValue ||                
                ex is CamposRequeridosException
            )           
            {
                return Results.BadRequest(ex.Message);
            }   
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });  
        group.MapDelete("/delete/{id:int}", async (HttpContext ctx, int id, BusService busService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();
            var userId = userIdClaim.Value;

            try
            {
                await Task.Run(() => busService.DeleteBus(int.Parse(userId), id));
                return Results.Accepted();
            }
            catch(UserInvalidado ex)
            {
                return Results.NotFound(ex.Message);
            }            
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });    
    }
}
