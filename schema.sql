


USE BD_IngeGrupo4;

CREATE TABLE Proyecto(
	nombre			VARCHAR(30),
	descripcion		VARCHAR(50),
	fechaInicio		DATE,
	fechaFinal		DATE,
	lider			CHAR(9),
	
	CONSTRAINT PK_Proyecto	PRIMARY KEY CLUSTERED ( nombre ASC ),
	CONSTRAINT FK_Usuario_Proyecto	FOREIGN KEY ( lider ) REFERENCES Usuario ( cedula )
									ON UPDATE NO ACTION
);
-- Atributo Duración es un atributo calculado, y por ende no se almacena en esta tabla.


CREATE TABLE Usuario(
	cedula			CHAR(9),
	nombre			VARCHAR(15),							
	apellidos		VARCHAR(40),
	correo			VARCHAR(40),
	lider			BIT,
--	id				NVARCHAR(128)
	
	CONSTRAINT PK_Usuario		PRIMARY KEY CLUSTERED ( cedula ASC )
--	CONSTRAINT FK_NetUsers_Usuario	FOREIGN KEY ( id ) REFERENCES AspNetUsers ( id )
);


CREATE TABLE ReqFuncional(
	id				SMALLINT,				
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
	imagen			LONGBLOB,
	fuente			CHAR(9),
	responsable1	CHAR(9),
	responsable2	CHAR(9),
	nomProyecto		VARCHAR(30),
	
	CONSTRAINT PK_ReqFuncional 	PRIMARY KEY CLUSTERED ( id ASC ),
	CONSTRAINT FK_Usuario_ReqFunc_Fuente	FOREIGN KEY ( fuente ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_ReqFunc_Resp1		FOREIGN KEY ( responsable1 ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Usuario_ReqFunc_Resp2		FOREIGN KEY ( responsable2 ) REFERENCES Usuario ( cedula ),
	CONSTRAINT FK_Proyecto_ReqFunc		FOREIGN KEY ( nomProyecto )	REFERENCES Proyecto ( nombre )
);

CREATE TABLE GestionCambios(
	Fecha			DATETIME,
	Razon			VARCHAR(50),
	idReqFunc		SMALLINT,
	RealizadoPor	CHAR(9),
	
	CONSTRAINT PK_GestionCambios	 PRIMARY KEY CLUSTERED ( idReqFunc,Fecha ASC ),
	CONSTRAINT FK_ReqFunc_GestCambios	FOREIGN KEY ( idReqFunc ) REFERENCES ReqFuncional ( id ),
	CONSTRAINT FK_Usuario_GestCambios	FOREIGN KEY ( RealizadoPor ) REFERENCES Usuario ( cedula ),
);

CREATE TABLE Permiso(
	id				SMALLINT,
	descripcion		VARCHAR(50),

	CONSTRAINT PK_Permiso	PRIMARY KEY CLUSTERED ( id ASC ),
);

CREATE TABLE ProyectoUsuario(
	proyecto	VARCHAR(30),
	usuario		CHAR(9)
	
	CONSTRAINT PK_ProyectoUsuario	PRIMARY KEY CLUSTERED ( proyecto, usuario ASC ),
	CONSTRAINT FK_Proyecto_ProyUsuario	FOREIGN KEY ( proyecto ) REFERENCES Proyecto ( nombre ),
	CONSTRAINT FK_Usuario_ProyUsuario FOREIGN KEY ( usuario ) REFERENCES Usuario ( cedula ),
);

CREATE TABLE NetRolesPermiso(
	idNetRoles	NVARCHAR(128),			
	idPermiso	SMALLINT,
	
	CONSTRAINT PK_NetRolesPermiso	PRIMARY KEY CLUSTERED ( idNetRoles, idPermiso ASC),
	CONSTRAINT FK_NetRoles_NetRolesUsers	FOREIGN KEY ( idNetRoles ) REFERENCES AspNetRoles ( id ),
	CONSTRAINT FK_Permiso_NetRolesUsers FOREIGN KEY ( idPermiso ) REFERENCES Permiso ( id )
);


CREATE TABLE Telefono(
	usuario		CHAR(9),
	numero		CHAR(8),

	CONSTRAINT PK_Telefono	PRIMARY KEY CLUSTERED ( usuario, numero ASC ),
	CONSTRAINT FK_Usuario_Telefono	FOREIGN KEY ( usuario ) REFERENCES Usuario ( cedula ),
);

CREATE TABLE CriterioAceptacion(
	idReqFunc	SMALLINT,	
	criterio	VARCHAR(128),
	
	CONSTRAINT PK_CritAceptacion	PRIMARY KEY CLUSTERED ( idReqFunc, criterio ASC ),
	CONSTRAINT FK_ReqFunc_CritAceptacion 	FOREIGN KEY ( idReqFunc ) REFERENCES ReqFuncional ( id )
);








