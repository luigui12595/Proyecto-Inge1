USE BD_IngeGrupo4;

CREATE TABLE Usuario(
	cedula			CHAR(9)			NOT NULL,
	nombre			VARCHAR(15),							
	apellidos		VARCHAR(40),
	correo			VARCHAR(40),
	lider			BIT,
	id				NVARCHAR(128)
	
	CONSTRAINT PK_Usuario		PRIMARY KEY CLUSTERED ( cedula ASC ),
	CONSTRAINT CHK_ced_Usuario	CHECK (cedula LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT FK_NetUsers_Usuario	FOREIGN KEY ( id ) REFERENCES AspNetUsers ( id )
									ON UPDATE CASCADE
);


CREATE TABLE Proyecto(
	nombre			VARCHAR(30)		NOT NULL,
	descripcion		VARCHAR(50),
	fechaInicio		DATE			NOT NULL,
	fechaFinal		DATE,
	estado			VARCHAR(12)		NOT NULL,
	lider			CHAR(9)			NOT NULL,
	Cliente 		CHAR(9) 		NULL,
	
	CONSTRAINT CHK_cliente_Proyecto CHECK (cliente LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	
	CONSTRAINT PK_Proyecto	PRIMARY KEY CLUSTERED ( nombre ASC ),
	CONSTRAINT FK_Usuario_Proyecto_Cliente FOREIGN KEY ( cliente ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_Proyecto	FOREIGN KEY ( lider ) REFERENCES Usuario ( cedula )
);
-- Atributo Duración es un atributo calculado, y por ende no se almacena en esta tabla.


CREATE TABLE ReqFuncional(
	id			INT IDENTITY	NOT NULl,				
	nombre			VARCHAR(20),
	sprint			TINYINT,
	modulo			TINYINT,
	estado			VARCHAR(10),
	fechaInicial		DATE,
	fechaFinal		DATE,
	observaciones		VARCHAR(256),
	descripcion		VARCHAR(256),
	esfuerzo		SMALLINT,	
	prioridad		SMALLINT,
	imagen			VARBINARY,
	fuente			CHAR(9),
	responsable1		CHAR(9),
	responsable2		CHAR(9),
	nomProyecto		VARCHAR(30)		NOT NULL,
	
	CONSTRAINT PK_ReqFuncional 	PRIMARY KEY CLUSTERED ( id, nomProyecto ASC ),
	
	CONSTRAINT CHK_fuente_reqFuncional	CHECK (fuente LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_resp1_reqFuncional	CHECK (responsable1 LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_resp2_reqFuncional	CHECK (responsable2 LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT FK_Usuario_ReqFunc_Fuente	FOREIGN KEY ( fuente ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_ReqFunc_Resp1		FOREIGN KEY ( responsable1 ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_ReqFunc_Resp2		FOREIGN KEY ( responsable2 ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Proyecto_ReqFunc		FOREIGN KEY ( nomProyecto )	REFERENCES Proyecto ( nombre )
										ON UPDATE CASCADE
);

CREATE TABLE HistVersiones(
	versionRF		SMALLINT		NOT NULL,
	fecha			DATE			NOT NULL,
	razon			VARCHAR(256)	NOT NULL,
	realizadoPor	CHAR(9)			NULL,
	idReqFunc		INT				NOT NULL,
	nomProyecto		VARCHAR(30)		NOT NULL,

	CONSTRAINT CHK_realizadoPor_HistVerciones CHECK (realizadoPor LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_HistVersiones	 PRIMARY KEY CLUSTERED ( idReqFunc, nomProyecto , versionRF ASC ),
	CONSTRAINT FK_ReqFunc_HistVersiones	FOREIGN KEY ( idReqFunc, nomProyecto ) REFERENCES ReqFuncional ( id, nomProyecto ),
	CONSTRAINT FK_Usuario_HistVersiones	FOREIGN KEY ( RealizadoPor ) REFERENCES Usuario ( cedula )
										ON UPDATE CASCADE
);


CREATE TABLE Solicitud(
	fecha			DATETIME		NOT NULL,
	estado			VARCHAR(11)		NOT NULL,
	razon			VARCHAR(256)	NOT NULL,
	realizadoPor	CHAR(9)			NULL,
	aprobadoPor		CHAR(9)			NULL,
	versionRF		SMALLINT		NOT NULL,
	idReqFunc		INT				NOT NULL,
	nomProyecto		VARCHAR(30)		NOT NULL,


	CONSTRAINT CHK_realizadoPor_Solicitud CHECK (realizadoPor LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_aprobadoPor_Solicitud CHECK (aprobadoPor LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_Solicitud PRIMARY KEY CLUSTERED ( versionRF, idReqFunc, nomProyecto, fecha ASC),
	CONSTRAINT FK_HistVersiones_Solicitud	FOREIGN KEY (idReqFunc, nomProyecto, versionRF ) REFERENCES HistVersiones ( idReqFunc, nomProyecto, versionRF ),
	CONSTRAINT FK_Usuario_Solicitud_realizadoPor FOREIGN KEY ( realizadoPor ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_Solicitud_aprobadoPor FOREIGN KEY ( aprobadoPor ) REFERENCES Usuario ( cedula )
);


CREATE TABLE Permiso(
	id				SMALLINT		NOT NULL,
	descripcion		VARCHAR(50)		NOT NULL,

	CONSTRAINT PK_Permiso	PRIMARY KEY CLUSTERED ( id ASC )
);

CREATE TABLE ProyectoUsuario(
	proyecto	VARCHAR(30)		NOT NULL,
	usuario		CHAR(9)			NOT NULL,
	
	CONSTRAINT CHK_usuario_ProyectoUsuario	CHECK (usuario LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_ProyectoUsuario	PRIMARY KEY CLUSTERED ( proyecto, usuario ASC ),
	CONSTRAINT FK_Proyecto_ProyUsuario	FOREIGN KEY ( proyecto ) REFERENCES Proyecto ( nombre ),
	CONSTRAINT FK_Usuario_ProyUsuario FOREIGN KEY ( usuario ) REFERENCES Usuario ( cedula )
										ON UPDATE CASCADE
);

CREATE TABLE NetRolesPermiso(
	idNetRoles	NVARCHAR(128)		NOT NULL,			
	idPermiso	SMALLINT			NOT NULL,
	
	CONSTRAINT PK_NetRolesPermiso	PRIMARY KEY CLUSTERED ( idNetRoles, idPermiso ASC),
	CONSTRAINT FK_NetRoles_NetRolesUsers	FOREIGN KEY ( idNetRoles ) REFERENCES AspNetRoles ( id )
											ON UPDATE CASCADE,
	CONSTRAINT FK_Permiso_NetRolesUsers FOREIGN KEY ( idPermiso ) REFERENCES Permiso ( id )
										ON UPDATE CASCADE
);


CREATE TABLE Telefono(
	usuario		CHAR(9)		NOT NULL,
	numero		CHAR(8),

	CONSTRAINT CHK_usuario_telefono	CHECK (usuario LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_numero_telefono	CHECK (numero LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_Telefono	PRIMARY KEY CLUSTERED ( usuario, numero ASC ),
	CONSTRAINT FK_Usuario_Telefono	FOREIGN KEY ( usuario ) REFERENCES Usuario ( cedula )
									ON UPDATE CASCADE,
);

CREATE TABLE CriterioAceptacion(
	idReqFunc	INT	NOT NULL,
	nomProyecto	VARCHAR(30),
	criterio	VARCHAR(128),
	
	CONSTRAINT PK_CritAceptacion	PRIMARY KEY CLUSTERED ( idReqFunc, nomProyecto, criterio ASC ),
	CONSTRAINT FK_ReqFunc_CritAceptacion 	FOREIGN KEY ( idReqFunc, nomProyecto ) REFERENCES ReqFuncional ( id,nomProyecto )
											ON UPDATE CASCADE
);

CREATE TRIGGER borrar_usuario
ON Usuario INSTEAD OF DELETE
AS
BEGIN
	UPDATE ReqFuncional
	SET fuente = NULL
	WHERE fuente IN (SELECT cedula
		              FROM deleted);

	UPDATE ReqFuncional
	SET responsable1 = NULL
	WHERE responsable1 IN (SELECT cedula
		              FROM deleted);
	
	UPDATE ReqFuncional
	SET responsable2 = NULL
	WHERE responsable2 IN (SELECT cedula
		              FROM deleted);

	DELETE FROM Telefono
	WHERE usuario IN (SELECT cedula
		          FROM deleted);

	DELETE FROM AspNetUserRoles
	WHERE UserId IN (SELECT id
	                 FROM deleted);

	DELETE FROM Usuario
	WHERE cedula IN (SELECT cedula
	                 FROM deleted);

	DELETE FROM AspNetUsers
	WHERE id IN (SELECT id
	             FROM deleted);

END;


CREATE TRIGGER borrar_reqFuncional
ON ReqFuncional INSTEAD OF DELETE
AS
BEGIN
	DECLARE @id INT
	DECLARE @nomProyecto VARCHAR(30)
	DECLARE cursorRF CURSOR FOR SELECT id, nomProyecto FROM deleted
	OPEN cursorRF
	FETCH NEXT FROM cursorRF INTO @id, @nomProyecto
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DELETE FROM CriterioAceptacion
		WHERE idReqFunc = @id
		AND nomProyecto = @nomProyecto

		DELETE FROM HistVersiones
		WHERE idReqFunc = @id
		AND nomProyecto = @nomProyecto

		FETCH NEXT FROM cursorRF INTO @id, @nomProyecto
	END
	
	DELETE FROM ReqFuncional
	WHERE id = @id
	AND nomProyecto = @nomProyecto

	CLOSE cursorRF
	DEALLOCATE cursorRF
END;

CREATE TRIGGER borrar_Version
ON HistVersiones INSTEAD OF DELETE
AS
BEGIN
	DECLARE @idReqFunc INT
	DECLARE @nomProyecto VARCHAR(30)
	DECLARE cursorHV CURSOR FOR SELECT idReqFunc, nomProyecto FROM deleted
	OPEN cursorHV
	FETCH NEXT FROM cursorHV INTO @idReqFunc, @nomProyecto

	DELETE FROM Solicitud
	WHERE idReqFunc = @idReqFunc
	AND nomProyecto = @nomProyecto

	DELETE FROM HistVersiones
	WHERE idReqFunc = @idReqFunc
	AND nomProyecto = @nomProyecto

	CLOSE cursorHV
	DEALLOCATE cursorHV
END;

CREATE TRIGGER borrar_reqFuncional
ON ReqFuncional INSTEAD OF DELETE
AS
BEGIN
	DECLARE @id INT
	DECLARE @nombre VARCHAR(30)
	DECLARE cursorRF CURSOR FOR SELECT id, nomProyecto FROM deleted
	OPEN cursorRF
	FETCH NEXT FROM cursorRF INTO @id, @nombre
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DELETE FROM CriterioAceptacion
		WHERE idReqFunc = @id
		AND nomProyecto = @nombre
		
		FETCH NEXT FROM cursorRF INTO @id, @nombre
	END
	DELETE FROM ReqFuncional
	WHERE id = @id
	AND nomProyecto = @nombre

	CLOSE cursorRF
	DEALLOCATE cursorRF
END;

CREATE TRIGGER borrar_proyecto
ON Proyecto INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM ProyectoUsuario
	WHERE proyecto IN (SELECT nombre
						FROM deleted);
	DECLARE @cantidad INT
	SET @cantidad = (SELECT COUNT(*) FROM ReqFuncional WHERE nomProyecto IN (SELECT nombre FROM deleted) );
	WHILE @cantidad != 0
	BEGIN
		DELETE FROM ReqFuncional
		WHERE nomProyecto IN (SELECT nombre
								FROM deleted)
		SET @cantidad = @cantidad - 1
	END

	DELETE FROM Proyecto
	WHERE nombre IN (SELECT nombre
						FROM deleted);
END;

CREATE TRIGGER crearVersionInicial
ON ReqFuncional AFTER INSERT
AS
BEGIN
	DECLARE @id INT
	DECLARE @nombre VARCHAR(20)
	DECLARE @sprint TINYINT
	DECLARE @modulo TINYINT
	DECLARE @estado VARCHAR(10)
	DECLARE @fechaInicial DATE
	DECLARE @fechaFinal DATE
	DECLARE @observaciones VARCHAR(256)
	DECLARE @descripcion VARCHAR(256)
	DECLARE @esfuerzo SMALLINT
	DECLARE @prioridad SMALLINT
	DECLARE @imagen VARBINARY(8000)
	DECLARE @fuente CHAR(9)
	DECLARE @responsable1 CHAR(9)
	DECLARE @responsable2 CHAR(9)
	DECLARE @nomProyecto VARCHAR(30)
	DECLARE @fechaDia VARCHAR

	DECLARE cursorRF CURSOR FOR SELECT id, nombre,sprint,modulo,estado,fechaInicial,fechaFinal,observaciones,descripcion,esfuerzo,prioridad,imagen,fuente,responsable1,responsable2,nomProyecto FROM inserted
	OPEN cursorRF
	FETCH NEXT FROM cursorRF INTO @id, @nombre, @sprint, @modulo, @estado, @fechaInicial, @fechaFinal, @observaciones, @descripcion, @esfuerzo, @prioridad, @imagen, @fuente, @responsable1, @responsable2, @nomProyecto 
	INSERT INTO HistVersiones VALUES(1, @fechaInicial, 'Version inicial requerimiento funcional', @fuente, @id, @nomProyecto, @nombre, @sprint, @modulo, @fechaInicial, @fechaFinal, @observaciones, @descripcion, @esfuerzo, @prioridad, @imagen, @responsable1, @responsable2)
	CLOSE cursorRF
	DEALLOCATE cursorRF
END;

