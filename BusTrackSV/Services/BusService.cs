using Modelos = BusTrackSV.Models;
using Data.Repositories;
using BusTrackSV.Models;
using System.Data.SqlTypes;

namespace BusTrackSV.Service;

public class BusService
{
	public List<int> getIds(int UserId, BusRepository db)
    {
        if(UserId < 0)
        {
            throw new UserInvalidado();
        }         
        var res = db.GetBusesIDByUserID(UserId);        
        return res;
    }

    public Bus getBus(int busID, BusRepository db)
    {
        var res = db.GetBusById(busID);
        if(res == null)
        {
            throw new NullValue();
        }
        return res;
    }

    public void AddBus(int UserID, BusRegistroDTO nbus, BusRepository db)
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

        if(db.RutaPertenceUsuario(nbus.id_ruta, UserID) == false)
        {
            throw new RutaNoRegistrada();
        }        
        
        db.RegistrarBus(nbus);
    }

    public void PutBus(int userID, Bus bus, BusRepository db)
    {
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }


        var buses = db.GetBusesIDByUserID(userID);
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

        db.ActualizarBus(bus);            
    }

    public void DeleteBus(int userID, int busID, BusRepository db)
    {
        if(userID <= 0)
        {
            throw new UserInvalidado();
        }

        var buses = db.GetBusesIDByUserID(userID);
        if(buses.Contains(busID) == false)
        {
            throw new UserInvalidado();
        }

        db.EliminarBus(busID);
    }
}
