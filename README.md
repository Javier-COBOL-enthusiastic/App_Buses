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
- **Backend:** C# (ASP.NET Core)  
- **Frontend:** HTML, CSS y JavaScript puro  
- **Base de datos:** SQL Server  

---

## Ejecución

1. Abrir SQL Server y crear la base de datos `BusTrackSVDB`.
2. Insertar datos de prueba para poder iniciar sesión y visualizar rutas y conductores.
3. Abrir el proyecto en Visual Studio.
4. Registrar el `DbContext`.
5. Habilitar **CORS** si el frontend y backend están en dominios distintos.
6. Seleccionar el proyecto **BusTrackSV** como proyecto de inicio.
7. Presionar **F5** o **Ctrl + F5** para ejecutar.
8. Verificar que el backend esté corriendo en `https://localhost:5000`.
9. Abrir `login.html` en un navegador.
10. Asegurarse de que los `fetch` apunten al endpoint correcto del backend.

