using Modelos = BusTrackSV.Models;
using Service = BusTrackSV.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

record LoginRequest(string email, string password);


namespace BusTrackSV.API
{    
    public static class AuthController
    {
        private static string key = "equipovicturboequipovicturboequipovicturboequipovicturbo"; //te odio Ivan;
        public static Service.AuthService AuthService = new Service.AuthService();
        public static void MapAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth");
            group.MapPost("/register", (Modelos.User u) =>
            {
                try
                {
                    System.Console.WriteLine(u.Nombre);
                    AuthService.Registrar(u);
                    return Results.Json(new { mensaje = u });
                }
                catch (Exception ex)
                {
                    return Results.Conflict(ex.Message);
                }
            });

            group.MapPost("/login", (LoginRequest req) =>
            {
                if (req.email != "prueba@gmail.com" && req.password != "qwerty123!!")
                {
                    return Results.Unauthorized();
                }
                                
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, req.email)
                };

                // TODO: Agregar con DataBase
                // var ret = AuthService.Login(req.email, req.password);
                // if (ret == null)
                // {
                //     return Results.BadRequest("Email o Password incorrectos");
                // }
                // return Results.Json(new { mensaje = ret });
                var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString, user =  req.email });
            });
        }



    }
}