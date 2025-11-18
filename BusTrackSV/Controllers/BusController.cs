using Service = BusTrackSV.Service;
namespace BusTrackSV.API;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Repositories;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using BusTrackSV.Models;

public static class BusController
{
	public static Service.BusService BusService = new Service.BusService();

	public static void MapBus(this WebApplication app)
    {
		var group = app.MapGroup("/buses");
		group.MapGet("/ids", [Authorize](HttpContext context, Data.Repositories.BusRepository db)  =>
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();            

    
            var userId = userIdClaim.Value;
            try
            {
                var res = BusService.getIds(int.Parse(userId), db);                
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
        group.MapGet("/get/{id:int}", [Authorize](int id, BusRepository db) =>
        {
            try
            {
                var res = BusService.getBus(id, db);
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

        group.MapPost("/add", [Authorize](HttpContext ctx, BusRegistroDTO nbus, BusRepository db) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();


            var userId = userIdClaim.Value;
            try
            {
                BusService.AddBus(int.Parse(userId), nbus, db);
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

        group.MapPut("/update", [Authorize](HttpContext ctx, Bus bus, BusRepository db) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();
            var userId = userIdClaim.Value;

            try
            {
                BusService.PutBus(int.Parse(userId), bus, db);
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
        group.MapDelete("/delete/{id:int}", [Authorize](HttpContext ctx, int id, BusRepository db) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();
            var userId = userIdClaim.Value;

            try
            {
                BusService.DeleteBus(int.Parse(userId), id, db);
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
