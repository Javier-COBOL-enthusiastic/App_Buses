using BusTrackSV.Models;
using Data.Repositories;

namespace BusTrackSV.Service;

public class BusService
{
    private readonly BusRepository _busRepository;
    public BusService(BusRepository busRepository)
    {
        _busRepository = busRepository;
    }
	public List<int> getIds(int UserId)
    {
        if(UserId < 0)
        {
            throw new UserInvalidado();
        }         
        var res = _busRepository.GetBusesIDByUserID(UserId);        
        return res;
    }

    public Bus getBus(int busID)
    {
        var res = _busRepository.GetBusById(busID);
        if(res == null)
        {
            throw new NullValue();
        }
        return res;
    }

    public void AddBus(int UserID, BusRegistroDTO nbus)
    {        
        if(UserID <= 0)
        {
            throw new UserInvalidado();
        }

        if(nbus == null)
        {
            throw new NullValue();
        }

        if(string.IsNullOrWhiteSpace(nbus.numero_placa))
        {
            throw new CamposRequeridosException();
        }

        if(_busRepository.RutaPertenceUsuario(nbus.id_ruta, UserID) == false)
        {
            throw new RutaNoRegistrada();
        }        
        
        _busRepository.RegistrarBus(nbus);
    }

    public void PutBus(int userID, Bus bus)
    {
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }


        var buses = _busRepository.GetBusesIDByUserID(userID);
        if(buses.Contains(bus.id_bus) == false)
        {
            throw new UserInvalidado();
        }
        
        if(bus == null)
        {
            throw new NullValue();
        }

        if(string.IsNullOrWhiteSpace(bus.numero_placa))
        {
            throw new CamposRequeridosException();
        }

        _busRepository.ActualizarBus(bus);            
    }

    public void DeleteBus(int userID, int busID)
    {
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }


        var buses = _busRepository.GetBusesIDByUserID(userID);
        if(buses.Contains(busID) == false)
        {
            throw new UserInvalidado();
        }

        _busRepository.EliminarBus(busID);
    }
}
