--USE master;
--ALTER DATABASE bustrack 
--SET SINGLE_USER 
--WITH ROLLBACK IMMEDIATE;
--DROP DATABASE bustrack; -- en caso de que queramos borrar la base

create database bustrack;

use bustrack;

CREATE TABLE coordenadas (
id_coordenada INT NOT NULL PRIMARY KEY IDENTITY(1,1),
latitud DECIMAL(10, 7) NOT NULL,
longitud DECIMAL(10, 7) NOT NULL
);

CREATE TABLE rutas(
id_ruta INT NOT NULL PRIMARY KEY IDENTITY(1,1),
nombre_ruta VARCHAR(10) NOT NULL,
descripcion_ruta VARCHAR(1000) NOT NULL
);

CREATE TABLE puntos_ruta (
id_punto_ruta INT NOT NULL PRIMARY KEY IDENTITY(1,1),
id_ruta INT NOT NULL,
id_coordenada INT NOT NULL,
orden INT NOT NULL, -- secuencia del recorrido
CONSTRAINT fk_pr_ruta FOREIGN KEY (id_ruta) REFERENCES rutas(id_ruta) 
ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT fk_pr_coordenada FOREIGN KEY (id_coordenada) REFERENCES coordenadas(id_coordenada)
ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE roles (
id_rol INT NOT NULL PRIMARY KEY IDENTITY(1,1),
nombre_rol VARCHAR(50) NOT NULL UNIQUE
);

create table usuarios(
id_usuario int not null primary key identity(1,1),
nombre_completo_usuario varchar(150) not null,
correo_electronico_usuario varchar(100) not null UNIQUE,
usuario varchar(100) not null UNIQUE, -- usuario con el que se inicia sesiòn, el nombre_usuario es su nombre de persona
contraseña_usuario varchar(100) not null, -- aquì se va a guardar el hash de la contraseña
CONSTRAINT U_correo UNIQUE(correo_electronico_usuario),
CONSTRAINT U_usuario UNIQUE(usuario)
);

ALTER TABLE usuarios
ADD CONSTRAINT fk_usuarios_roles FOREIGN KEY (id_rol)
REFERENCES roles(id_rol)
ON UPDATE CASCADE ON DELETE NO ACTION;


CREATE TABLE buses(
id_bus INT NOT NULL PRIMARY KEY IDENTITY(1,1),
numero_placa VARCHAR(10) NOT NULL UNIQUE,
estado_bus BIT NOT NULL, -- Solo acepta 0 o 1
id_ruta INT,
id_usuario INT NOT NULL,
CONSTRAINT fk_buses_rutas FOREIGN KEY (id_ruta) REFERENCES rutas(id_ruta)
ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT fk_buses_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT U_placa UNIQUE(numero_placa)
);

CREATE TABLE choferes(
id_chofer INT NOT NULL PRIMARY KEY IDENTITY(1,1),
nombre_completo_chofer VARCHAR(100) NOT NULL,
telefono_chofer char(9) NOT NULL UNIQUE,
id_bus INT NOT NULL UNIQUE,
CONSTRAINT fk_choferes_bus FOREIGN KEY (id_bus) REFERENCES buses(id_bus)
ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT U_telefono_chofer UNIQUE(telefono_chofer)
);

CREATE TABLE user_rutas (
  id_usuario INT NOT NULL,
  id_ruta INT NOT NULL,
  CONSTRAINT fk_ur_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
  CONSTRAINT fk_ur_ruta FOREIGN KEY (id_ruta) REFERENCES rutas(id_ruta),
  CONSTRAINT pk_user_rutas PRIMARY KEY (id_usuario, id_ruta)
);
-- luego, validar con:
-- SELECT 1 FROM user_rutas WHERE id_usuario=@userId AND id_ruta=@routeId

-- es mejor usar procedimientos almacenado, es más seguro y es más dificil de vulnerar que dejar el muy puro "insert-deleté-update"

-- procedimiento almacenado para guardar usuarios

CREATE PROCEDURE sp_registrar_usuario
    @nombre_completo NVARCHAR(150),
    @correo NVARCHAR(100),
    @usuario NVARCHAR(100),
    @password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- SAL elegida: equipovicturbo (democraticamente)
        DECLARE @salt NVARCHAR(15) = 'equipovicturbo';

        -- Hash SHA2_256(SAL + contraseña)
        DECLARE @hash VARBINARY(32) =
            HASHBYTES('SHA2_256', @salt + @password);

        INSERT INTO usuarios (
            nombre_completo_usuario,
            correo_electronico_usuario,
            usuario,
            contraseña_usuario
        )
        VALUES (
            @nombre_completo,
            @correo,
            @usuario,
            CONVERT(VARCHAR(100), @hash, 2)
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- ODIO A LOS ROLES :,(
ALTER PROCEDURE sp_registrar_usuario
    @nombre_completo NVARCHAR(150),
    @correo NVARCHAR(100),
    @usuario NVARCHAR(100),
    @password NVARCHAR(200),
    @id_rol INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @salt NVARCHAR(15) = 'equipovicturbo';

        DECLARE @hash VARBINARY(32) =
            HASHBYTES('SHA2_256', @salt + @password);

        INSERT INTO usuarios (
            nombre_completo_usuario,
            correo_electronico_usuario,
            usuario,
            contraseña_usuario,
            id_rol
        )
        VALUES (
            @nombre_completo,
            @correo,
            @usuario,
            CONVERT(VARCHAR(100), @hash, 2),
            @id_rol
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SELECT * FROM usuarios


-- procedimiento almacenado para la tabla rutas

CREATE PROCEDURE sp_registrar_rutas
    @nombre_ruta VARCHAR(10),
    @descripcion_ruta VARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO rutas(nombre_ruta, descripcion_ruta)
        VALUES (@nombre_ruta, @descripcion_ruta);

        -- Devolver el id generado para que ExecuteScalar() lo reciba
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS id_ruta;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla puntos_rutas

CREATE PROCEDURE sp_registrar_puntos_ruta
    @id_ruta INT,
    @id_coordenada INT,
    @orden INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO puntos_ruta(id_ruta, id_coordenada, orden)
        VALUES (@id_ruta, @id_coordenada, @orden);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla usuarios_rutas

CREATE PROCEDURE sp_registrar_usuarios_rutas
    @id_usuario INT,
    @id_ruta INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO user_rutas(id_usuario, id_ruta)
        VALUES (@id_usuario, @id_ruta);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla buses

CREATE PROCEDURE sp_registrar_buses
    @numero_placa VARCHAR(10),
    @estado_bus BIT,
    @id_ruta INT,
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO buses(numero_placa, estado_bus, id_ruta, id_usuario)
        VALUES (@numero_placa, @estado_bus, @id_ruta, @id_usuario);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla choferes

CREATE PROCEDURE sp_registrar_choferes
    @nombre_completo VARCHAR(150),
    @telefono_chofer char(9),
    @id_bus INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO choferes(nombre_completo_chofer, telefono_chofer, id_bus)
        VALUES (@nombre_completo, @telefono_chofer, @id_bus);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO



-- Ejecutador de procedimientos almacenados

--usuarios
EXEC sp_registrar_usuario
    @nombre_completo = 'Daniel Salguero',
    @correo = 'daniel@example.com',
    @usuario = 'dsalguero',
    @password = 'Academia2025'

 
-- rutas
EXEC sp_registrar_rutas 
    @nombre_ruta = 'Ruta 101',
    @descripcion_ruta = 'Ruta que va del Centro hasta la casa de Javier';

EXEC sp_registrar_rutas 
    @nombre_ruta = 'Ruta 101-B',
    @descripcion_ruta = 'Ruta que va del Centro hasta la casa de Enrique';


-- consulta para ver las coordenadas de una ruta, ordenada
SELECT id_punto_ruta, longitud, latitud, nombre_ruta from puntos_ruta pr
INNER JOIN rutas r on pr.id_ruta = r.id_ruta
INNER JOIN coordenadas c on pr.id_coordenada = c.id_coordenada
ORDER BY pr.orden ASC



-- consulta para que aparezca una sola vez la placa si aparece en varios registros
SELECT 
CASE WHEN ROW_NUMBER() OVER(PARTITION BY b.numero_placa ORDER BY pr.orden) = 1 -- el primer registro donde aparece el numero_placa va a mostrar numero_placa
THEN b.numero_placa 
ELSE '' 
END AS numero_placa,
c.latitud, c.longitud, r.nombre_ruta
FROM buses b
INNER JOIN rutas r ON b.id_ruta = r.id_ruta
INNER JOIN puntos_ruta pr ON pr.id_ruta = r.id_ruta
INNER JOIN coordenadas c ON pr.id_coordenada = c.id_coordenada
ORDER BY b.numero_placa, pr.orden;

-- choferes
EXEC sp_registrar_choferes @nombre_completo = 'Enrique Rafael deL Ano', @telefono_chofer = '9999-0000', @id_bus=2; --  @id_bus = 1
EXEC sp_registrar_choferes @nombre_completo = 'Javier Boliviano', @telefono_chofer = '9909-0000', @id_bus=4; --  @id_bus = 1
EXEC sp_registrar_choferes @nombre_completo = 'Pruebencia', @telefono_chofer = '9900-0000', @id_bus=5; --  @id_bus = 1

-- Procedimiento almacenado para validar el usuario y su contraseña

CREATE PROCEDURE sp_validar_usuario_login
    @usuario NVARCHAR(100),
    @password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- SAL usada en sp_registrar_usuario: equipovicturbo
    DECLARE @salt NVARCHAR(15) = 'equipovicturbo';
    
    -- Hash SHA2_256(SAL + contraseña)
    DECLARE @hash_entrada VARBINARY(32) = 
        HASHBYTES('SHA2_256', @salt + @password);
        
    -- Convertir el hash de entrada a VARCHAR para compararlo con el de la DB
    DECLARE @hash_comparar VARCHAR(100) = CONVERT(VARCHAR(100), @hash_entrada, 2);

    -- Agregar mensajes de error personalizados

    IF NOT EXISTS (SELECT 1 FROM usuarios WHERE usuario = @usuario AND contraseña_usuario = @hash_comparar)
    BEGIN
      -- excepción personalizada (código >= 50000 ó >= 51000)
      THROW 51000, 'Credenciales inválidas.', 1;
    END

    -- Seleccionar el usuario si el nombre de usuario y el hash coinciden
    SELECT 
    u.id_usuario, 
    u.nombre_completo_usuario, 
    u.correo_electronico_usuario,
    u.id_rol,
    u.usuario
    FROM usuarios u
    INNER JOIN roles r ON u.id_rol = r.id_rol
    WHERE 
        u.usuario = @usuario 
        AND u.contraseña_usuario = @hash_comparar;
END;
GO



-- procedimiento almacenado para eliminar
-- Eliminación de usuarios

CREATE PROCEDURE sp_eliminar_usuario
  @id_usuario INT 
AS
BEGIN
  DELETE FROM usuarios
  WHERE id_usuario = @id_usuario;
END;
GO

SELECT * FROM usuarios;
-- EXEC sp_eliminar_usuario @id_usuario = 1 


-- eliminar coordenadas
CREATE PROCEDURE sp_eliminar_coordenadas
  @id_coordenada INT
AS
BEGIN
  DELETE FROM coordenadas
  WHERE id_coordenada = @id_coordenada;
END;
GO

SELECT * FROM coordenadas;
-- EXEC sp_eliminar_coordenadas @id_coordenada = 1;


-- eliminar rutas
CREATE PROCEDURE sp_eliminar_rutas
  @id_ruta INT
AS
BEGIN
  DELETE FROM rutas
  WHERE id_ruta = @id_ruta;
END;
GO

SELECT * FROM rutas;
-- EXEC sp_eliminar_rutas @id_ruta = 1;


-- eliminar relación coordenadas y rutas

CREATE PROCEDURE sp_eliminar_coord_rutas
  @id_ruta INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    -- El OUTPUT devuelve los id_coordenada de las filas eliminadas
    DELETE FROM puntos_ruta
    OUTPUT deleted.id_coordenada
    WHERE id_ruta = @id_ruta;
  END TRY
  BEGIN CATCH
    THROW;
  END CATCH 
END;
GO

SELECT * FROM puntos_ruta;
-- EXEC sp_eliminar_coord_rutas @id_punto_ruta = 1;


-- eliminar buses
CREATE PROCEDURE sp_eliminar_buses
  @id_bus INT
AS
BEGIN
  DELETE FROM buses
  WHERE id_bus = @id_bus;
END;
GO

SELECT * FROM buses;
-- EXEC sp_eliminar_buses @id_bus = 1;

-- elminar relación usuarios y rutas
CREATE PROCEDURE sp_eliminar_usuarios_rutas
  @id_usuario INT,
  @id_ruta INT
AS
BEGIN
  DELETE FROM user_rutas
  WHERE id_usuario = @id_usuario AND id_ruta = @id_ruta;
END;
GO


-- eliminar choferes
CREATE PROCEDURE sp_eliminar_choferes
  @id_chofer INT
AS
BEGIN
  DELETE FROM choferes
  WHERE id_chofer = @id_chofer;
END;
GO

SELECT * FROM choferes;
-- EXEC sp_eliminar_choferes @id_chofer = 1;

------------------------------------------------------



-- procedimientos almacenados para actualizar datos
-- Actualizar usuarios

CREATE PROCEDURE sp_actualizar_usuario
    @id_usuario INT,
    @nombre_completo VARCHAR(150),
    @correo VARCHAR(100),
    @usuario VARCHAR(100),
    @password VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        
        DECLARE @salt NVARCHAR(15) = 'equipovicturbo';

        -- Hash SHA2_256(SAL + contraseña)
        DECLARE @hash VARBINARY(32) =
            HASHBYTES('SHA2_256', @salt + @password);

        UPDATE usuarios SET nombre_completo_usuario = @nombre_completo,
        correo_electronico_usuario = @correo,
        usuario = @usuario,
        contraseña_usuario = @hash
        WHERE id_usuario = @id_usuario
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- CULPO DE TODO A ENRIQUE Y A LOS ROLES
ALTER PROCEDURE sp_actualizar_usuario
    @id_usuario INT,
    @nombre_completo VARCHAR(150),
    @correo VARCHAR(100),
    @usuario VARCHAR(100),
    @password VARCHAR(200),
    @id_rol INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        
        DECLARE @salt NVARCHAR(15) = 'equipovicturbo';

        DECLARE @hash VARBINARY(32) =
            HASHBYTES('SHA2_256', @salt + @password);

        UPDATE usuarios
        SET nombre_completo_usuario = @nombre_completo,
            correo_electronico_usuario = @correo,
            usuario = @usuario,
            contraseña_usuario = CONVERT(VARCHAR(100), @hash, 2),
            id_rol = @id_rol
        WHERE id_usuario = @id_usuario;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO



SELECT * FROM usuarios;
-- EXEC sp_actualizar_usuario @id_usuario = 1, @nombre_completo = 'Gustavio Rafael Del Ano', @correo = 'gustavio@gmail.com', @usuario = 'perunoesclave', @password = 'hondurasesclave1234';

-- Actualizar buses
CREATE PROCEDURE sp_actualizar_buses
    @id_bus INT,
    @numero_placa VARCHAR(10),
    @estado_bus BIT,
    @id_ruta INT,
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE buses SET numero_placa = @numero_placa,
        estado_bus = @estado_bus,
        id_ruta = @id_ruta,
        id_usuario = @id_usuario
        WHERE id_bus = @id_bus
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SELECT * FROM buses;
-- EXEC sp_actualizar_buses @id_bus = 4, @numero_placa = 'P-12345', @estado_bus = 1, @id_ruta = 2, @id_usuario = 1; -- @id_bus = 1

-- actualizar coordenadas

CREATE PROCEDURE sp_actualizar_coordenadas
    @id_coordenada INT,
    @latitud DECIMAL(10, 7),
    @longitud DECIMAL(10, 7)
AS
BEGIN
    

    BEGIN TRY
        UPDATE coordenadas
        SET 
            latitud = @latitud,
            longitud = @longitud
        WHERE id_coordenada = @id_coordenada;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SELECT * FROM coordenadas;
-- EXEC sp_actualizar_coordenadas @id_coordenada = 1, @latitud = 13.6929, @longitud = -89.2182;

-- actualizar rutas
CREATE PROCEDURE sp_actualizar_ruta
    @id_ruta INT,
    @nombre_ruta VARCHAR(10),
    @descripcion_ruta VARCHAR(1000)
AS
BEGIN

    BEGIN TRY
        UPDATE rutas
        SET nombre_ruta = @nombre_ruta,
            descripcion_ruta = @descripcion_ruta
        WHERE id_ruta = @id_ruta;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
SELECT * FROM rutas;
-- EXEC sp_actualizar_ruta @id_ruta = 1, @nombre_ruta = 'Ruta 01', @descripcion_ruta = 'Ruta principal del centro al sur.';

-- actualizar buses

create procedure sp_actualizar_puntos_ruta
    @id_punto_ruta INT,
    @id_ruta INT,
    @id_coordenada INT
AS
    BEGIN
    
    BEGIN TRY
    UPDATE puntos_ruta
        SET id_ruta = @id_ruta,
            id_coordenada = @id_coordenada
        WHERE id_punto_ruta = @id_punto_ruta;
    END TRY
    BEGIN CATCH
    THROW;
    END CATCH

END;
GO

-- Actualizar usuarios_rutas
CREATE PROCEDURE sp_actualizar_usuarios_rutas
    @id_usuario INT,
    @id_ruta INT
AS
BEGIN
    BEGIN TRY
        UPDATE user_rutas
        SET id_ruta = @id_ruta
        WHERE id_usuario = @id_usuario;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SELECT * FROM puntos_ruta;
-- EXEC sp_actualizar_puntos_ruta @id_punto_ruta = 1, @id_ruta = 2, @id_coordenada = 5;

-- Actualizar choferes
CREATE PROCEDURE sp_actualizar_chofer
    @id_chofer INT,
    @nombre_completo_chofer VARCHAR(100),
    @telefono_chofer CHAR(9),
    @id_bus INT
AS
BEGIN

    BEGIN TRY
        UPDATE choferes
        SET nombre_completo_chofer = @nombre_completo_chofer,
            telefono_chofer = @telefono_chofer,
            id_bus = @id_bus
        WHERE id_chofer = @id_chofer;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
SELECT * FROM choferes;
-- EXEC sp_actualizar_chofer @id_chofer = 3, @nombre_completo_chofer = 'Juan Perez', @telefono_chofer = '77778888', @id_bus = 3; -- @id_chofer = 3


SELECT * 
FROM sys.tables;

SELECT * FROM coordenadas
SELECT * FROM rutas
SELECT * from puntos_ruta


