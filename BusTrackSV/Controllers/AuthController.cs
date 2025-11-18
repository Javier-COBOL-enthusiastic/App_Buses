using Modelos = BusTrackSV.Models;
using Service = BusTrackSV.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DB = Data.Repositories;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BusTrackSV.API
{    
    public static class AuthController
    {
        private static string key = "equipovicturboequipovicturboequipovicturboequipovicturbo"; //te odio Ivan;
        public static Service.AuthService AuthService = new Service.AuthService();
        public static void MapAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth");
            group.MapPost("/register", (Modelos.UsuarioRegistroDTO u, DB.UsuarioRepository db) =>
            {                
                //System.Console.WriteLine(u.nombre_completo);
                try
                {
                    var res = AuthService.Registrar(u, db);                                         
                    return Results.Accepted();
                }                
                catch(SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 2627:
                            return Results.Conflict(new {error = "Ya existe un usuario con este correo."});
                        default:
                            return Results.Conflict(new { error = ex.Message});
                    }
                }
                catch (Exception ex)
                {                    
                    return Results.Problem(ex.Message);
                }
                              
            });

            group.MapPost("/login", (Modelos.LoginRequest req, DB.UsuarioRepository db) =>
            {            
                // System.Console.WriteLine(req.usuario);
                // System.Console.WriteLine(req.password);
                var res = AuthService.Login(req, db);    
                if(res == null)
                {
                    return Results.Problem("Autenticación problema");
                }
                else if(res.id_usuario < 0)
                {
                    return Results.BadRequest("Campos requeridos incompletos");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, req.usuario),
                    new Claim("userId", res.id_usuario.ToString())
                };
                var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString, user = req.usuario });
            });
        }
    }
}