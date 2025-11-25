<<<<<<< HEAD
﻿using System;
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



=======
﻿using BusTrackSV.Models;
using BusTrackSV.Service;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace BusTrackSV.API
{    
    public static class AuthController
    {
        private static string key = "equipovicturboequipovicturboequipovicturboequipovicturbo";
        public static void MapAuthController(this WebApplication app)
        {
            var group = app.MapGroup("/auth");

            group.MapPost("/register", async (UsuarioRegistroDTO u, AuthService authService) =>
            {
                try
                {
                    await Task.Run(() => authService.Registrar(u));
                    return Results.Ok(new {message = "Usuario registrado correctamente." });
                }                
                catch(SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 2627:
                            return Results.Conflict(new { error = "Ya existe un usuario con este correo." });
                        default:
                            return Results.Conflict(new { error = ex.Message });
                    }
                }
                catch (Exception ex)
                {                    
                    return Results.Problem(ex.Message);
                }                              
            });

            group.MapPost("/login", async (LoginRequest req, AuthService authService) =>
            {
                try
                {                                                      
                    var res = await Task.Run(() => authService.Login(req));
                    if (res == null)
                        return Results.Problem("Autenticación problema");
                    if (res.id_usuario < 0)
                        return Results.BadRequest("Campos requeridos incompletos");

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, req.usuario),
                        new Claim("userId", res.id_usuario.ToString())
                    };
                    var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Results.Ok(new { token = tokenString, user = req.usuario });
                }
                catch (SqlException ex)
                {
                    if(ex.Number == 51000)
                    {
                        return Results.Unauthorized();
                    }

                    return Results.Problem(ex.Message);
                }
                catch (Exception ex)        
                {                                        
                    return Results.Problem(ex.Message);
                }
            });
        }
>>>>>>> Javier
    }
}