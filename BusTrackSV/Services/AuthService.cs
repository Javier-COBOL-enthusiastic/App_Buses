using Modelos = BusTrackSV.Models;
using DB = Data.Repositories;

namespace BusTrackSV.Service;

public class AuthService
{
    public Modelos.UsuarioRegistroDTO Registrar(Modelos.UsuarioRegistroDTO user, DB.UsuarioRepository _usuarioRepository)
    {
        _usuarioRepository.RegistrarUsuario(user);
        return user;
    }

    public Modelos.Usuario? Login(string email, string password, DB.UsuarioRepository _usuarioRepository)
    {
        return _usuarioRepository.Login(email, password);
    }
}
