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


create table usuarios(
id_usuario int not null primary key identity(1,1),
nombre_usuario varchar(100) not null,
apellido_usuario varchar(100) not null,
DUI_usuario char(10) not null,
correo_electronico_usuario varchar(100) not null,
fecha_nacimiento_usuario date not null,
usuario varchar(100) not null, -- usuario con el que se inicia sesiòn, el nombre_usuario es su nombre de persona
contraseña_usuario varchar(100) not null, -- aquì se va a guardar el hash de la contraseña
);

create table telefonos_usuarios(
id_telefono_usuario int not null primary key identity(1,1),
telefono_usuario varchar(20) not null, -- ya que se van a guardar varios telefonos, pueden ser telefonos internacional
id_usuario int not null,
CONSTRAINT fk_telefonos_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE buses(
id_bus INT NOT NULL PRIMARY KEY IDENTITY(1,1),
numero_placa VARCHAR(10) NOT NULL,
capacidad INT NOT NULL,
id_ruta INT NOT NULL,
id_usuario INT NOT NULL,
CONSTRAINT fk_buses_rutas FOREIGN KEY (id_ruta) REFERENCES rutas(id_ruta)
ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT fk_buses_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE choferes(
id_chofer INT NOT NULL PRIMARY KEY IDENTITY(1,1),
nombre_chofer VARCHAR(100) NOT NULL,
apellido_chofer VARCHAR(100) NOT NULL,
DUI_chofer CHAR(10) NOT NULL,
fecha_nacimiento DATE NOT NULL,
id_bus INT NOT NULL,
CONSTRAINT fk_choferes_bus FOREIGN KEY (id_bus) REFERENCES buses(id_bus)
ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE telefonos_choferes(
id_telefono_choferes INT NOT NULL PRIMARY KEY IDENTITY(1,1),
telefono_choferes VARCHAR(20) NOT NULL,
id_chofer INT NOT NULL,
CONSTRAINT fk_telefonos_choferes FOREIGN KEY (id_chofer) REFERENCES choferes(id_chofer)
ON DELETE CASCADE ON UPDATE CASCADE
);

-- es mejor usar procedimientos almacenado, es más seguro y es más dificil de vulnerar que dejar el muy puro "insert-deleté-update"

-- procedimiento almacenado para guardar usuarios
CREATE PROCEDURE sp_registrar_usuario
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @dui CHAR(10),
    @correo NVARCHAR(100),
    @fecha DATE,
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
            nombre_usuario,
            apellido_usuario,
            DUI_usuario,
            correo_electronico_usuario,
            fecha_nacimiento_usuario,
            usuario,
            contraseña_usuario
        )
        VALUES (
            @nombre,
            @apellido,
            @dui,
            @correo,
            @fecha,
            @usuario,
            CONVERT(VARCHAR(100), @hash, 2)
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para guardar telefonos de usuarios

CREATE PROCEDURE sp_registrar_telefonos_usuarios
    @telefonousuario VARCHAR(10),
    @idusuarioo VARCHAR(200)
    
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO telefonos_usuarios(
            telefono_usuario, id_usuario
        )
        VALUES (
            @telefonousuario, @idusuarioo 
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla coordenadas
CREATE PROCEDURE sp_registrar_coordenadas
    @latitud DECIMAL(10,7),
    @longitud DECIMAL(10,7)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO coordenadas(latitud, longitud)
        VALUES (@latitud, @longitud);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

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

-- procedimiento almacenado para la tabla buses
CREATE PROCEDURE sp_registrar_buses
    @numero_placa VARCHAR(10),
    @capacidad INT,
    @id_ruta INT,
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO buses(numero_placa, capacidad, id_ruta, id_usuario)
        VALUES (@numero_placa, @capacidad, @id_ruta, @id_usuario);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla choferes
CREATE PROCEDURE sp_registrar_choferes
    @nombre VARCHAR(100),
    @apellido VARCHAR(100),
    @dui CHAR(10),
    @fecha_nacimiento DATE,
    @id_bus INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO choferes(nombre_chofer, apellido_chofer, DUI_chofer, fecha_nacimiento, id_bus)
        VALUES (@nombre, @apellido, @dui, @fecha_nacimiento, @id_bus);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla telefonos_choferes
CREATE PROCEDURE sp_registrar_telefonos_choferes
    @telefono VARCHAR(20),
    @id_chofer INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO telefonos_choferes(telefono_choferes, id_chofer)
        VALUES (@telefono, @id_chofer);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO






-- Ejecutador de procedimientos almacenados

--usuarios
EXEC sp_registrar_usuario
    @nombre = 'Daniel',
    @apellido = 'Salguero',
    @dui = '12345678-9',
    @correo = 'daniel@example.com',
    @fecha = '2000-01-01',
    @usuario = 'dsalguero',
    @password = 'Academia2025'

-- telefonos de usuario
EXEC sp_registrar_telefonos_usuarios
    @telefonousuario = '7123-4567',
    @idusuarioo = 1; -- aquí va el id del usuario, yo como ya había hecho pruebas ya no es el id 1, solo vean cual es el id que corresponde al usuario que quieran asignarle el telefono
   
-- rutas
EXEC sp_registrar_rutas 
    @nombre_ruta = 'Ruta 101',
    @descripcion_ruta = 'Ruta que va del Centro hasta la casa de Javier';

EXEC sp_registrar_rutas 
    @nombre_ruta = 'Ruta 101-B',
    @descripcion_ruta = 'Ruta que va del Centro hasta la casa de Enrique';

-- Insertar una coordenada de prueba
EXEC sp_registrar_coordenadas 
    @latitud = 13.6990000,
    @longitud = -89.1910000;

-- Insertar otra coordenada
EXEC sp_registrar_coordenadas 
    @latitud = 13.7005000,
    @longitud = -89.1902000;

EXEC sp_registrar_coordenadas @latitud=13.6991000, @longitud=-89.1911000;
EXEC sp_registrar_coordenadas @latitud=13.6992000, @longitud=-89.1912000;
EXEC sp_registrar_coordenadas @latitud=13.6993000, @longitud=-89.1913000;
EXEC sp_registrar_coordenadas @latitud=13.6994000, @longitud=-89.1914000;
EXEC sp_registrar_coordenadas @latitud=13.6995000, @longitud=-89.1915000;
EXEC sp_registrar_coordenadas @latitud=13.6996000, @longitud=-89.1916000;
EXEC sp_registrar_coordenadas @latitud=13.6997000, @longitud=-89.1917000;
EXEC sp_registrar_coordenadas @latitud=13.6998000, @longitud=-89.1918000;
EXEC sp_registrar_coordenadas @latitud=13.6999000, @longitud=-89.1919000;
EXEC sp_registrar_coordenadas @latitud=13.7000000, @longitud=-89.1920000;
EXEC sp_registrar_coordenadas @latitud=13.7001000, @longitud=-89.1921000;
EXEC sp_registrar_coordenadas @latitud=13.7002000, @longitud=-89.1922000;
EXEC sp_registrar_coordenadas @latitud=13.7003000, @longitud=-89.1923000;
EXEC sp_registrar_coordenadas @latitud=13.7004000, @longitud=-89.1924000;
EXEC sp_registrar_coordenadas @latitud=13.7005000, @longitud=-89.1925000;

-- Ruta 101
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=1, @orden=1;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=2, @orden=2;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=3, @orden=3;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=4, @orden=4;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=5, @orden=5;

EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=6, @orden=6;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=7, @orden=7;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=8, @orden=8;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=9, @orden=9;
EXEC sp_registrar_puntos_ruta @id_ruta=1, @id_coordenada=10, @orden=10;

-- consulta para ver las coordenadas de una ruta, ordenada
SELECT id_punto_ruta, longitud, latitud, nombre_ruta from puntos_ruta pr
INNER JOIN rutas r on pr.id_ruta = r.id_ruta
INNER JOIN coordenadas c on pr.id_coordenada = c.id_coordenada
ORDER BY pr.orden ASC

-- buses
EXEC sp_registrar_buses @numero_placa='P000 101', @capacidad=40, @id_ruta=1, @id_usuario=1;
EXEC sp_registrar_buses @numero_placa='P 9 1A2', @capacidad=30, @id_ruta=2, @id_usuario=1;


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
EXEC sp_registrar_choferes @nombre='Juan', @apellido='Pérez', @dui='12345678-9', @fecha_nacimiento='1985-05-12', @id_bus=1;
EXEC sp_registrar_choferes @nombre='María', @apellido='Gómez', @dui='98765432-1', @fecha_nacimiento='1990-08-22', @id_bus=2;

-- telefonos_choferes
EXEC sp_registrar_telefonos_choferes @telefono='7777-1111', @id_chofer=1;
EXEC sp_registrar_telefonos_choferes @telefono='7777-2222', @id_chofer=1;
EXEC sp_registrar_telefonos_choferes @telefono='8888-3333', @id_chofer=2;
EXEC sp_registrar_telefonos_choferes @telefono='8888-4444', @id_chofer=2;

