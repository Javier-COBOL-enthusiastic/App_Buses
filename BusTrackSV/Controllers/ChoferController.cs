using BusTrackSV.Service;
using Microsoft.AspNetCore.Authorization;
using Data.Repositories;
using BusTrackSV.Models;

namespace BusTrackSV.API;

public static class ChoferController
{
        public static void MapChoferController(this WebApplication app)
    {
        var group = app.MapGroup("/choferes");
        group.RequireAuthorization();
        
        group.MapGet("/get/{id:int}", (HttpContext ctx, int id, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();

            var userId = userIdClaim.Value;
            try
            {
                var res = choferService.GetChofer(int.Parse(userId), id);
                return Results.Json(res);
            }
            catch (UserInvalidado ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapPost("/add", (HttpContext ctx, ChoferRegistroDTO nch, ChoferService choferService) =>
        {
            var userIdClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Results.Unauthorized();   


            var userId = userIdClaim.Value;
            try
            {
                choferService.AddChofer(int.Parse(userId),nch);                
                return Results.Accepted();
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

        group.MapPut("/update", (HttpContext ctx, Chofer nchofer, ChoferService choferService) =>
        {
            var userIDclaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if(userIDclaim == null)
            {
                return Results.Unauthorized();                
            }

            var userID = userIDclaim.Value;            
            try
            {
                choferService.UpdateChofer(int.Parse(userID), nchofer);
                return Results.Accepted();
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
        group.MapDelete("/delete/{id:int}", (HttpContext ctx, int id, ChoferService choferService) =>{
            var userIDclaim = ctx.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if(userIDclaim == null)
            {
                return Results.Unauthorized();                
            }
            var userID = userIDclaim.Value;

            try
            {
                choferService.DeleteChofer(int.Parse(userID), id);
                return Results.Accepted();
            }
            catch(UserInvalidado ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}