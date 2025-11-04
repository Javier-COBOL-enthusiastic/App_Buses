using System;
using Modelos = BusTrackSV.Models;
using Service = BusTrackSV.Service;
using Microsoft.AspNetCore.Builder;

record LoginRequest(string email, string password);

namespace BusTrackSV.API
{
    public static class AuthController
    {
        public static Service.AuthService AuthService = new Service.AuthService();
        public static void MapAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth");
            group.MapPost("/register", (Modelos.User u) =>
            {
                try
                {
                    AuthService.Registrar(u);
                    return Results.Json(new { mensaje = u });
                }
                catch (Exception ex)
                {
                    return Results.Conflict(ex);
                }
            });

            group.MapPost("/login", (LoginRequest req) =>
            {
                var ret = AuthService.Login(req.email, req.password);
                if (ret == null)
                {
                    return Results.BadRequest("Email o Password incorrectos");
                }
                return Results.Json(new { mensaje = ret });
                
            });
        }



    }
}