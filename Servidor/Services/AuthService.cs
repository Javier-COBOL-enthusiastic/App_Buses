using Modelos = BusTrackSV.Models;
namespace BusTrackSV.Service;

public class AuthService
{
    public Modelos.User Registrar(Modelos.User user)
    {
        // TODO: implementar con DB
        throw new NotImplementedException();
    }

    public Modelos.User? Login(string email, string password)
    {
        //TODO: implementar con DB
        return null;
    }
}
