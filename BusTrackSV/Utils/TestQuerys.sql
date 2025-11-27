use bustrack;
go


select * from usuarios
select * from user_rutas
select * from buses
select * from rutas
select * from puntos_ruta
select * from choferes
select * from coordenadas

SELECT c.id_chofer, b.id_bus, u.id_usuario FROM choferes c 
INNER JOIN buses b on c.id_bus = b.id_bus 
INNER JOIN usuarios u on b.id_usuario = u.id_usuario 
WHERE u.id_usuario = 22;

SELECT id_usuario FROM user_rutas where id_ruta = 2;

SELECT 
    c.id_chofer,    
    b.id_bus,
    u.id_usuario,
    u.usuario AS nombre_de_usuario
FROM choferes c
INNER JOIN buses b ON c.id_bus = b.id_bus
INNER JOIN usuarios u ON b.id_usuario = u.id_usuario
WHERE u.id_usuario = 4;


SELECT
  pr.id_punto_ruta,
  pr.id_ruta,
  r.nombre_ruta,
  pr.orden,
  pr.id_coordenada,
  c.latitud,
  c.longitud
FROM puntos_ruta pr
JOIN coordenadas c ON pr.id_coordenada = c.id_coordenada
JOIN rutas r ON pr.id_ruta = r.id_ruta
ORDER BY pr.id_ruta, pr.orden;


UPDATE choferes
SET id_bus = 8
WHERE id_chofer = 7;

select * from buses

exec sp_registrar_usuarios_rutas
@id_usuario = 4,
@id_ruta = 6