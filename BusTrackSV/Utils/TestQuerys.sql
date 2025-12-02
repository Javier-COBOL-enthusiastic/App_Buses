use bustrack;
go


select * from roles
select * from rutas
select * from user_rutas
select * from puntos_ruta
select * from usuarios
select * from buses
select * from choferes
select * from coordenadas

-- Obtener info del bus por usuario del chofer
SELECT 
  b.id_bus,
  b.numero_placa,
  b.estado_bus,
  r.nombre_ruta,
  r.descripcion_ruta  
FROM buses b
INNER JOIN rutas r ON b.id_ruta = r.id_ruta
INNER JOIN choferes ch ON b.id_bus = ch.id_bus
INNER JOIN usuarios u ON u.nombre_completo_usuario = ch.nombre_completo_chofer
WHERE u.id_usuario = 38;

exec sp_eliminar_usuario
@id_usuario = 36;

exec sp_eliminar_choferes
@id_chofer = 21;

exec sp_eliminar_buses
@id_bus = 39;

DELETE FROM choferes WHERE id_chofer = 10;
DELETE FROM buses WHERE id_bus = 27;

UPDATE usuarios SET id_rol = 3 WHERE id_rol = 2;

SELECT c.id_chofer, b.id_bus, u.id_usuario FROM choferes c 
INNER JOIN buses b on c.id_bus = b.id_bus 
INNER JOIN usuarios u on b.id_usuario = u.id_usuario 
WHERE u.id_usuario = 22;

            SELECT            
            u.id_usuario                                                
            FROM usuarios u
            JOIN choferes ch ON u.nombre_completo_usuario = ch.nombre_completo_chofer
            WHERE ch.id_chofer = 13


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

exec sp_validar_usuario_login 
@usuario = "prueba01",
@password = "123456!"


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