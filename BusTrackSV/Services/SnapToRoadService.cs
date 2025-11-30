using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusTrackSV.Service;

public class SnapToRoadService
{
    private GeoJsonData? _roadsData;
    private readonly ILogger<SnapToRoadService> _logger;
    private const double MAX_SEARCH_DISTANCE = 100.0; // metros
    private const string GEOJSON_PATH = "calles.geojson";

    public SnapToRoadService(ILogger<SnapToRoadService> logger)
    {
        _logger = logger;
        LoadGeoJson();
    }

    private void LoadGeoJson()
    {
        try
        {
            _logger.LogInformation("Cargando archivo GeoJSON...");
            var jsonString = File.ReadAllText(GEOJSON_PATH);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            _roadsData = JsonSerializer.Deserialize<GeoJsonData>(jsonString, options);
            
            // Filtrar features inválidas
            if (_roadsData?.Features != null)
            {
                var validFeatures = _roadsData.Features
                    .Where(f => f?.Geometry?.Type == "LineString" && f.Geometry.GetCoordinatePairs() != null)
                    .ToList();
                
                int invalidCount = _roadsData.Features.Count - validFeatures.Count;
                _roadsData.Features = validFeatures;
                
                _logger.LogInformation($"GeoJSON cargado exitosamente: {validFeatures.Count} features válidas (LineStrings), {invalidCount} features omitidas");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el archivo GeoJSON");
            _roadsData = null;
        }
    }

    public (double latitud, double longitud) SnapToRoad(double lat, double lng)
    {
        if (_roadsData?.Features == null || _roadsData.Features.Count == 0)
        {
            _logger.LogWarning("GeoJSON no cargado, retornando coordenadas originales");
            return (lat, lng);
        }

        double minDistance = double.MaxValue;
        (double lat, double lng) closestPoint = (lat, lng);

        foreach (var feature in _roadsData.Features)
        {
            if (feature.Geometry?.Type != "LineString")
                continue;

            var coords = feature.Geometry.GetCoordinatePairs();
            if (coords == null || coords.Count == 0)
                continue;

            // Verificación rápida: revisar si al menos un punto del segmento está cerca
            bool hasNearbyPoint = false;
            foreach (var coord in coords)
            {
                if (coord.Length >= 2)
                {
                    double quickDist = HaversineDistance(lat, lng, coord[1], coord[0]);
                    if (quickDist < MAX_SEARCH_DISTANCE * 2)
                    {
                        hasNearbyPoint = true;
                        break;
                    }
                }
            }

            if (!hasNearbyPoint) continue;

            // Buscar el punto más cercano en cada segmento
            for (int i = 0; i < coords.Count - 1; i++)
            {
                if (coords[i].Length < 2 || coords[i + 1].Length < 2)
                    continue;

                double lng1 = coords[i][0];
                double lat1 = coords[i][1];
                double lng2 = coords[i + 1][0];
                double lat2 = coords[i + 1][1];

                var nearest = ClosestPointOnSegment(lng, lat, lng1, lat1, lng2, lat2);
                double distance = HaversineDistance(lat, lng, nearest.lat, nearest.lng);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = nearest;
                }
            }
        }

        // Si el punto más cercano está muy lejos, usar el punto original
        if (minDistance > MAX_SEARCH_DISTANCE)
        {
            return (lat, lng);
        }

        return closestPoint;
    }

    private double HaversineDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371000; // Radio de la Tierra en metros
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLng = (lng2 - lng1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private (double lat, double lng) ClosestPointOnSegment(double px, double py, double x1, double y1, double x2, double y2)
    {
        double dx = x2 - x1;
        double dy = y2 - y1;

        if (dx == 0 && dy == 0)
        {
            return (y1, x1);
        }

        double t = Math.Max(0, Math.Min(1, ((px - x1) * dx + (py - y1) * dy) / (dx * dx + dy * dy)));

        return (y1 + t * dy, x1 + t * dx);
    }
}

// Clases para deserializar el GeoJSON
public class GeoJsonData
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("features")]
    public List<GeoJsonFeature>? Features { get; set; }
}

public class GeoJsonFeature
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("geometry")]
    public GeoJsonGeometry? Geometry { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, JsonElement>? Properties { get; set; }
}

public class GeoJsonGeometry
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("coordinates")]
    public JsonElement Coordinates { get; set; }
    
    // Helper method to get coordinate pairs for LineString
    public List<double[]>? GetCoordinatePairs()
    {
        try
        {
            if (Type != "LineString")
                return null;
                
            var coords = new List<double[]>();
            
            if (Coordinates.ValueKind == JsonValueKind.Array)
            {
                foreach (var coord in Coordinates.EnumerateArray())
                {
                    if (coord.ValueKind == JsonValueKind.Array)
                    {
                        var pair = coord.EnumerateArray().Select(e => e.GetDouble()).ToArray();
                        if (pair.Length >= 2)
                        {
                            coords.Add(pair);
                        }
                    }
                }
            }
            
            return coords.Count > 0 ? coords : null;
        }
        catch
        {
            return null;
        }
    }
}

