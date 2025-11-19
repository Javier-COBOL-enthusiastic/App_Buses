using BusTrackSV.Models;
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
        private static string key = "equipovicturboequipovicturboequipovicturboequipovicturbo"; //te odio Ivan;        
        public static void MapAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth");
            group.MapPost("/register", (UsuarioRegistroDTO u, AuthService authService) =>
            {                
                //System.Console.WriteLine(u.nombre_completo);
                try
                {
                    var res = authService.Registrar(u);                                         
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

            group.MapPost("/login", (LoginRequest req, AuthService authService) =>
            {            
                // System.Console.WriteLine(req.usuario);
                // System.Console.WriteLine(req.password);
                var res = authService.Login(req);    
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