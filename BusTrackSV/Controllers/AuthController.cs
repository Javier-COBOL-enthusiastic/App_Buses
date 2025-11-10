using System;
using Modelos = BusTrackSV.Models;
using Service = BusTrackSV.Service;
using Microsoft.AspNetCore.Builder;
using DB = Data.Repositories;
using Data.Repositories;

record LoginRequest(string email, string password);

namespace BusTrackSV.API
{
    public static class AuthController
    {
        public static Service.AuthService AuthService = new Service.AuthService();           
        public static void MapAuth(this WebApplication app)
        {            
            var group = app.MapGroup("/auth");
            group.MapPost("/register", (Modelos.UsuarioRegistroDTO u, DB.UsuarioRepository db) =>
            {
                try
                {                    
                    u.fecha = DateTime.Now;
                    AuthService.Registrar(u, db); //parece que nunca dara error pq le dio ganas a enrique que no duelva nada
                    return Results.Json(new { mensaje = u });
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return Results.Conflict(ex.Message);
                }
            });

            group.MapPost("/login", (LoginRequest req, DB.UsuarioRepository db) =>
            {
                var ret = AuthService.Login(req.email, req.password, db);
                if (ret == null)
                {
                    return Results.BadRequest("Email o Password incorrectos");
                }
                return Results.Json(new { mensaje = ret });
                
            });
        }



    }
}