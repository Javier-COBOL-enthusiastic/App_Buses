using Service = BusTrackSV.Service;
namespace BusTrackSV.API;
public static class BusController
{
	public static Service.BusService BusService = new Service.BusService();

	public static void MapBus(this WebApplication app)
    {
		var group = app.MapGroup("/routes");
		group.MapGet("{id_usuario}", (int id_usuario) =>
        {
            
        });
    }
}
