using Modelos = BusTrackSV.Models;
namespace BusTrackSV.Service;
using Data.Repositories;

public class AuthService
{
    public Modelos.UsuarioRegistroDTO Registrar(Modelos.UsuarioRegistroDTO user, UsuarioRepository _usuarioRepository)
    {
        if(user.usuario == "" || user.correo == "" || user.nombre_completo == "" || user.password == "")
        {
            throw new CamposRequeridosException();
        }
        
        _usuarioRepository.RegistrarUsuario(user);        
        return user;        
        
    }

    public Modelos.UsuarioValidado? Login(Modelos.LoginRequest req, UsuarioRepository _usuarioRepository)    
    {
        if(req.usuario == "" || req.password == "")
        {
            Modelos.UsuarioValidado ans =  new Modelos.UsuarioValidado();
            ans.id_usuario = -1;
            return ans;
        }

        var res = _usuarioRepository.Login(req.usuario, req.password);                
        return res;
    }
}
