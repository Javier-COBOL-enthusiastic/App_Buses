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
        private static string key = "equipovicturboequipovicturboequipovicturboequipovicturbo";

        public static void MapAuthController(this WebApplication app)
        {
            var group = app.MapGroup("/auth");

            // ====== Registro ======
            group.MapPost("/register", async (UsuarioRegistroDTO u, AuthService authService) =>
            {
                try
                {
                    Console.WriteLine("Registrando usuario: " + u.usuario);
                    Console.WriteLine("ROL ASIGNADO: " + u.id_rol);

                    await Task.Run(() => authService.Registrar(u));
                    return Results.Ok(new { message = "Usuario registrado correctamente." });
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Exception: " + ex.Message);
                    if (ex.Number == 2627)
                        return Results.Conflict(new { error = "Ya existe un usuario con este correo." });

                    return Results.Conflict(new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // ====== Login ======
            group.MapPost("/login", async (LoginRequest req, AuthService authService) =>
            {
                try
                {
                    Console.WriteLine("Intentando login para usuario: " + req.usuario);

                    var res = await Task.Run(() => authService.Login(req));
                    if (res == null)
                        return Results.Problem("Autenticación problema");

                    if (res.id_usuario < 0)
                        return Results.BadRequest("Campos requeridos incompletos");

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, req.usuario),
                        new Claim("userId", res.id_usuario.ToString()),
                        new Claim("roleId", res.id_rol.ToString()),
                    };

                    var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: creds);

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Results.Ok(new { token = tokenString, user = res });
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 51000)
                        return Results.Unauthorized();

                    return Results.Problem(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });
        }
    }
}
