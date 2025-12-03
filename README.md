# BusTrackSV

## Descripción
**BusTrackSV** es una aplicación web destinada a dueños de buses, que permite:
- Hacer seguimiento de sus buses.
- Visualizar rutas.
- Ver conductores asignados.

---

## Requisitos
- **Visual Studio 2022** o superior.
- **SQL Server** (o compatible).
- **Navegador moderno** (Chrome, Edge, Firefox).

---

## Tecnologías usadas
- **Backend:** C# (ASP.NET Core 9.0)  
- **Frontend:** HTML, CSS y JavaScript puro  
- **Base de datos:** SQL Server  

---

## Ejecución
1. Instala .NET 9: https://dotnet.microsoft.com/
2. Abre SQL Server Management y crea la base de datos con base_buses.sql
3. En la carpeta BusTrackSV/, ejecuta:
    dotnet restore
    dotnet run
4. Para ejecutar el Frontend escribe en una consola:
    python -m http.server 5500 
    y posteriormente abre http://localhost:5500

