using BusTrackSV.Models;
using Data.Repositories;

namespace BusTrackSV.Service;

public class RutaService
{
    private readonly RutasRepository _rutasRepository;
    private readonly PuntosRutaRepository _puntosRutaRepository;
    private readonly CoordenadasRepository _coordenadasRepository;
    
    private readonly BusRepository _busRepository;
    public RutaService(RutasRepository rutasRepository, PuntosRutaRepository puntosRutaRepository, CoordenadasRepository coordenadasRepository, BusRepository busRepository)
    {
        _rutasRepository = rutasRepository;
        _puntosRutaRepository = puntosRutaRepository;
        _coordenadasRepository = coordenadasRepository;
        _busRepository = busRepository;
    }

    public int RegistrarRuta(Ruta nuevaRuta, List<Coordenada> coordenadas)
    {
        if(nuevaRuta == null)
            throw new NullValue();
        if(coordenadas == null || coordenadas.Count == 0)
            throw new CamposRequeridosException();

        if(string.IsNullOrEmpty(nuevaRuta.nombre_ruta) || string.IsNullOrEmpty(nuevaRuta.descripcion_ruta))
            throw new CamposRequeridosException();
        
        List<int> CoordsID = _coordenadasRepository.RegistrarCoordenadas(coordenadas);

        int idRuta = _rutasRepository.RegistrarRuta(nuevaRuta);        
    
        _puntosRutaRepository.RegistrarPuntosRuta(idRuta, CoordsID);

        return idRuta;
    }

    public Ruta ObtenerRutaPorId(int idUsuario, int idRuta)
    {
        
        if(idUsuario <= 0 || idRuta <= 0)
            throw new NullValue("ID de usuario o ruta inválido.");
        
        if(!_busRepository.RutaPertenceUsuario(idRuta, idUsuario))
            throw new UnauthorizedAccessException("El usuario no tiene asignada esta ruta.");

        var res = _rutasRepository.GetRutaById(idRuta);        

        if(res == null)
            throw new NullValue("La ruta solicitada no existe.");

        return res;
    }

    public void ActualizarRuta(int idUsuario, Ruta ruta)
    {


        //TODO: Agregar un nuevo modelo para actualizar la ruta con coordenadas incluidas
        if (idUsuario <= 0)
            throw new UserInvalidado();

        if (ruta == null)
            throw new NullValue();

        if (ruta.id_ruta <= 0)
            throw new NullValue("ID de ruta inválido.");

        if (string.IsNullOrWhiteSpace(ruta.nombre_ruta) || string.IsNullOrWhiteSpace(ruta.descripcion_ruta))
            throw new CamposRequeridosException();
        
        var existente = _rutasRepository.GetRutaById(ruta.id_ruta);
        if (existente == null)
            throw new NullValue("La ruta a actualizar no existe.");

        if (!_busRepository.RutaPertenceUsuario(ruta.id_ruta, idUsuario))
            throw new UnauthorizedAccessException("El usuario no tiene permiso para actualizar esta ruta.");

        _rutasRepository.ActualizarRuta(ruta);
    }
    public void EliminarRuta(int idUsuario, int idRuta)
    {
        //TODO: Verificar que la ruta no esté asignada a ningun bus antes + verificar que
        // si el usuario es el unico con la ruta, eliminar full de la base de datos, si no, solo desvincularla del usuario
        if (idUsuario <= 0)
            throw new UserInvalidado();

        if (idRuta <= 0)
            throw new NullValue("ID de ruta inválido.");

        var existente = _rutasRepository.GetRutaById(idRuta);
        if (existente == null)
            throw new NullValue("La ruta a eliminar no existe.");

        if (!_busRepository.RutaPertenceUsuario(idRuta, idUsuario))
            throw new UnauthorizedAccessException("El usuario no tiene permiso para eliminar esta ruta.");

        _rutasRepository.EliminarRuta(idRuta);        
    }
}