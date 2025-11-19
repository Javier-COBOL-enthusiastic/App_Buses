using BusTrackSV.Models;
using Data.Repositories;

namespace BusTrackSV.Service;
public class ChoferService
{
    private readonly ChoferRepository _choferRepository;
    private readonly BusRepository _busRepository;

    public ChoferService(ChoferRepository choferRepository, BusRepository busRepository)
    {
        _choferRepository = choferRepository;
        _busRepository = busRepository;
    }

    public Chofer GetChofer(int userID, int busID)
    {
        var buses = _busRepository.GetBusesIDByUserID(userID);
        if(buses.Contains(busID) == false)
        {
            throw new UserInvalidado();
        }
        var chofer = _choferRepository.GetChoferByIdBus(busID);
        if(chofer == null)
        {
            throw new NullValue();
        }

        return chofer;
    }
    public void AddChofer(int userID, ChoferRegistroDTO nuevoChofer)
    {
        var buses = _busRepository.GetBusesIDByUserID(userID);
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }
        
        if(buses.Contains(nuevoChofer.id_bus) == false)
        {
            throw new UserInvalidado();
        }

       Bus? bus = _busRepository.GetBusById(nuevoChofer.id_bus);

        if(bus == null)
        {
            throw new NullValue();
        }

       if(bus.id_usuario != userID)
       {
            throw new UserInvalidado();
       }

        if(string.IsNullOrEmpty(nuevoChofer.telefono_chofer) || string.IsNullOrEmpty(nuevoChofer.nombre_completo))
        {
            throw new CamposRequeridosException();
        }

        _choferRepository.RegistrarChofer(nuevoChofer);
    }

    public void UpdateChofer(int userID, Chofer chofer)
    {
        var choferes = _choferRepository.GetChoferIdByUserID(userID);
        if(choferes.Contains(chofer.id_chofer) == false)
        {
            throw new UserInvalidado();
        }

        if(string.IsNullOrEmpty(chofer.telefono_chofer) || string.IsNullOrEmpty(chofer.nombre_completo))
        {
            throw new CamposRequeridosException();
        }

        _choferRepository.ActualizarChofer(chofer);
    }

    public void DeleteChofer(int userID, int choferID)
    {
        var choferes = _choferRepository.GetChoferIdByUserID(userID);
        if(choferes.Contains(choferID) == false)
        {
            throw new UserInvalidado();
        }

        _choferRepository.EliminarChofer(choferID);
    }
}