<<<<<<< HEAD
﻿using Modelos = BusTrackSV.Models;
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
=======
﻿using BusTrackSV.Models;
using Data.Repositories;

namespace BusTrackSV.Service;
public class AuthService
{

    private readonly UsuarioRepository _usuarioRepository;
    public AuthService(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
    public UsuarioRegistroDTO Registrar(UsuarioRegistroDTO user)
    {
        if(user.usuario == "" || user.correo == "" || user.nombre_completo == "" || user.password == "")
        {
            throw new CamposRequeridosException();
        }
        
        _usuarioRepository.RegistrarUsuario(user);        
        return user;        
        
    }

    public UsuarioValidado? Login(LoginRequest req)    
    {
        if(req.usuario == "" || req.password == "")
        {
            UsuarioValidado ans =  new UsuarioValidado();
            ans.id_usuario = -1;
            return ans;
        }

        var res = _usuarioRepository.Login(req.usuario, req.password);                
        return res;
>>>>>>> Javier
    }
}
