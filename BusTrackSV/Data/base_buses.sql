--USE master;
--ALTER DATABASE bustrack 
--SET SINGLE_USER 
--WITH ROLLBACK IMMEDIATE;
--DROP DATABASE bustrack; -- en caso de que queramos borrar la base

create database bustrack;

use bustrack;

create table roles_usuarios(
id_rol int not null primary key identity(1,1),
nombre_rol varchar(30) not null
);

create table coordenadas(
id_coordenada int not null primary key identity(1,1),
direccion_1 varchar(200) not null,
direccion_2 varchar(200) not null,
punto_1 varchar(200) not null,
punto_2 varchar(200) not null
);

create table rutas(
id_ruta int not null primary key identity(1,1),
nombre_ruta varchar(10) not null,
descripcion_ruta varchar(1000) not null,
id_coordenada int not null,
CONSTRAINT fk_rutas_coordenadas FOREIGN KEY (id_coordenada) REFERENCES coordenadas(id_coordenada)
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
id_rol int not null,
CONSTRAINT fk_usuario_rol FOREIGN KEY (id_rol) REFERENCES roles_usuarios(id_rol)
ON DELETE CASCADE ON UPDATE CASCADE
);

create table telefonos_usuarios(
id_telefono_usuario int not null primary key identity(1,1),
telefono_usuario varchar(20) not null, -- ya que se van a guardar varios telefonos, pueden ser telefonos internacional
id_usuario int not null,
CONSTRAINT fk_telefonos_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
ON DELETE CASCADE ON UPDATE CASCADE
);


create table buses(
id_bus int not null primary key identity(1,1),
numero_placa varchar(10) not null,
capacidad int not null,
id_ruta int not null,
CONSTRAINT fk_buses_rutas FOREIGN KEY (id_ruta) REFERENCES rutas(id_ruta)
ON DELETE CASCADE ON UPDATE CASCADE,
id_usuario int not null,
CONSTRAINT fk_buses_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
ON DELETE CASCADE ON UPDATE CASCADE
);


create table choferes(
id_chofer int not null primary key identity(1,1),
nombre_chofer varchar(100) not null,
apellido_chofer varchar(100) not null,
DUI_chofer char(10) not null,
fecha_nacimiento date not null,
id_bus int not null,
CONSTRAINT fk_choferes_bus FOREIGN KEY (id_bus) REFERENCES buses(id_bus)
ON DELETE CASCADE ON UPDATE CASCADE
);

create table telefonos_choferes(
id_telefono_choferes int not null primary key identity(1,1),
telefono_choferes varchar(20) not null, -- ya que se van a guardar varios telefonos, pueden ser telefonos internacional
id_chofer int not null,
CONSTRAINT fk_telefonos_choferes FOREIGN KEY (id_chofer) REFERENCES choferes(id_chofer)
ON DELETE CASCADE ON UPDATE CASCADE
);

EXEC sp_registrar_roles
    @nombre_rol = 'Administrador';

-- Ejecutador de procedimientos almacenados
--usuarios
EXEC sp_registrar_usuario
    @nombre = 'Daniel',
    @apellido = 'Salguero',
    @dui = '12345678-9',
    @correo = 'daniel@example.com',
    @fecha = '2000-01-01',
    @usuario = 'dsalguero',
    @password = 'Academia2025',
    @idrol = 1;


-- es mejor usar procedimientos almacenado, es más seguro y es más dificil de vulnerar que dejar el muy puro "insert-deleté-update"

-- procedimiento almacenado para guardar usuarios
CREATE PROCEDURE sp_registrar_usuario
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @dui CHAR(10),
    @correo NVARCHAR(100),
    @fecha DATE,
    @usuario NVARCHAR(100),
    @password NVARCHAR(200),
    @idrol int
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
            contraseña_usuario,
            id_rol
        )
        VALUES (
            @nombre,
            @apellido,
            @dui,
            @correo,
            @fecha,
            @usuario,
            CONVERT(VARCHAR(100), @hash, 2),
            @idrol
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para guardar roles de usuario
CREATE PROCEDURE sp_registrar_roles
    @nombre_rol VARCHAR(30)
    
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO roles_usuarios(
            nombre_rol
        )
        VALUES (
            @nombre_rol
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para guardar las coordenadas
CREATE PROCEDURE sp_registrar_coordenadas
    @direccion1 VARCHAR(200),
    @direccion2 VARCHAR(200),
    @punto1 VARCHAR(200),
    @punto2 VARCHAR(200)
    
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO coordenadas(
            direccion_1, direccion_2, punto_1, punto_2
        )
        VALUES (
            @direccion1, @direccion2, @punto1, @punto2  
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para guardar las rutas
CREATE PROCEDURE sp_registrar_rutas
    @nombreruta VARCHAR(10),
    @descripcionruta VARCHAR(200),
    @idcoordenada int
    
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO rutas(
            nombre_ruta, descripcion_ruta, id_coordenada
        )
        VALUES (
            @nombreruta, @descripcionruta, @idcoordenada  
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

-- procedimiento almacenado para la tabla buses
CREATE PROCEDURE sp_registrar_bus
    @numeroplaca VARCHAR(10),
    @capacidad INT,
    @idruta INT,
    @idusuario INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO buses (
            numero_placa,
            capacidad,
            id_ruta,
            id_usuario
        )
        VALUES (
            @numeroplaca,
            @capacidad,
            @idruta,
            @idusuario
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla choferes
CREATE PROCEDURE sp_registrar_chofer
    @nombrechofer VARCHAR(100),
    @apellidochofer VARCHAR(100),
    @DUIchofer CHAR(10),
    @fechanacimiento DATE,
    @idbus INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO choferes (
            nombre_chofer,
            apellido_chofer,
            DUI_chofer,
            fecha_nacimiento,
            id_bus
        )
        VALUES (
            @nombrechofer,
            @apellidochofer,
            @DUIchofer,
            @fechanacimiento,
            @idbus
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- procedimiento almacenado para la tabla de telefonos de choferes

CREATE PROCEDURE sp_registrar_telefono_chofer
    @telefonochoferes VARCHAR(20),
    @idchofer INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO telefonos_choferes (
            telefono_choferes,
            id_chofer
        )
        VALUES (
            @telefonochoferes,
            @idchofer
        );
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

