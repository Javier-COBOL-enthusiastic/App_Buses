using BusTrackSV.Models;
using Data.Repositories;

namespace BusTrackSV.Service;

public class BusService
{    
    private readonly BusRepository _busRepository;
    private readonly BusTrackingService _track;
    public BusService(BusRepository busRepository, BusTrackingService track)
    {
        _busRepository = busRepository;
        _track = track;
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
    public BusLocation GetLocation(int userID, int busID)
    {
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }

        var buses =_busRepository.GetBusesIDByUserID(userID);
        if(buses.Contains(busID) == false)
        {
            throw new UserInvalidado();
        }

        var res = _track.GetLocation(busID);        
        if(res == null)
        {
            throw new NullValue();
        }
        return res;
    }
    public void UpdateLocation(BusLocation nbus)
    {        
        if(nbus.IdBus <= 0)
        {
            throw new UserInvalidado();
        } //jijija        
                
        _track.UpdateLocation(nbus);
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

        nbus.id_usuario = UserID;                
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

public class BusTrackingService
{    
    private readonly Dictionary<int, BusLocation> _busLocations = new();
 
    public void UpdateLocation(BusLocation nbus)
    {
        _busLocations[nbus.IdBus] = nbus;        
    }

    public BusLocation? GetLocation(int busId)
    {
        _busLocations.TryGetValue(busId, out var loc);
        return loc;
    }
}
