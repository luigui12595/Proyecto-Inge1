
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

CREATE TABLE GestionCambios(
	Fecha			DATETIME		NOT NULL,
	Razon			VARCHAR(50),
	idReqFunc		INT,
	nomProyecto		VARCHAR(30), 
	realizadoPor		CHAR(9),
	
	CONSTRAINT CHK_realizadoPor_gestCambios	CHECK (realizadoPor LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_GestionCambios	 PRIMARY KEY CLUSTERED ( idReqFunc, nomProyecto ,Fecha ASC ),
	CONSTRAINT FK_ReqFunc_GestCambios	FOREIGN KEY ( idReqFunc, nomProyecto ) REFERENCES ReqFuncional ( id, nomProyecto ),
	CONSTRAINT FK_Usuario_GestCambios	FOREIGN KEY ( RealizadoPor ) REFERENCES Usuario ( cedula )
										ON UPDATE CASCADE
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


