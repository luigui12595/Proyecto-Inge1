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
	lider			CHAR(9)			NOT NULL,
	
	CONSTRAINT PK_Proyecto	PRIMARY KEY CLUSTERED ( nombre ASC ),
	CONSTRAINT FK_Usuario_Proyecto	FOREIGN KEY ( lider ) REFERENCES Usuario ( cedula )
									ON UPDATE CASCADE
);
-- Atributo Duración es un atributo calculado, y por ende no se almacena en esta tabla.


CREATE TABLE ReqFuncional(
	id				SMALLINT		NOT NULL,				
	nombre			VARCHAR(20),
	sprint			TINYINT,
	modulo			TINYINT,
	estado			VARCHAR(10),
	fechaInicial	DATE,
	fechaFinal		DATE,
	observaciones	VARCHAR(50),
	descripcion		VARCHAR(50),
	esfuerzo		SMALLINT,	
	prioridad		SMALLINT,
	imagen			VARBINARY,
	fuente			CHAR(9),
	responsable1	CHAR(9),
	responsable2	CHAR(9),
	nomProyecto		VARCHAR(30)		NOT NULL,
	
	CONSTRAINT PK_ReqFuncional 	PRIMARY KEY CLUSTERED ( id ASC ),
	
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
	idReqFunc		SMALLINT,
	realizadoPor	CHAR(9),
	
	CONSTRAINT CHK_realizadoPor_gestCambios	CHECK (realizadoPor LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_GestionCambios	 PRIMARY KEY CLUSTERED ( idReqFunc,Fecha ASC ),
	CONSTRAINT FK_ReqFunc_GestCambios	FOREIGN KEY ( idReqFunc ) REFERENCES ReqFuncional ( id ),
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
	CONSTRAINT CHK_numero_telefono	CHECK (usuario LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT PK_Telefono	PRIMARY KEY CLUSTERED ( usuario, numero ASC ),
	CONSTRAINT FK_Usuario_Telefono	FOREIGN KEY ( usuario ) REFERENCES Usuario ( cedula )
									ON UPDATE CASCADE,
);

CREATE TABLE CriterioAceptacion(
	idReqFunc	SMALLINT			NOT NULL,	
	criterio	VARCHAR(128),
	
	CONSTRAINT PK_CritAceptacion	PRIMARY KEY CLUSTERED ( idReqFunc, criterio ASC ),
	CONSTRAINT FK_ReqFunc_CritAceptacion 	FOREIGN KEY ( idReqFunc ) REFERENCES ReqFuncional ( id )
											ON UPDATE CASCADE
);


