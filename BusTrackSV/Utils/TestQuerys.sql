use bustrack;
go


select * from usuarios
select * from user_rutas
select * from buses

SELECT 
    c.id_chofer,    
    b.id_bus,
    u.id_usuario,
    u.usuario AS nombre_de_usuario
FROM choferes c
INNER JOIN buses b ON c.id_bus = b.id_bus
INNER JOIN usuarios u ON b.id_usuario = u.id_usuario
WHERE u.id_usuario = 4;



exec sp_registrar_usuarios_rutas
@id_usuario = 4,
@id_ruta = 2