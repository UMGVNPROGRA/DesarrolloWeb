-- db_ac0b5e_hospitalqueue.dbo.Clinicas definition

-- Drop table

-- DROP TABLE db_ac0b5e_hospitalqueue.dbo.Clinicas;

CREATE TABLE db_ac0b5e_hospitalqueue.dbo.Clinicas (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Descripcion nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Activa bit DEFAULT 1 NOT NULL,
	CONSTRAINT PK__Clinicas__3214EC079BA63FDC PRIMARY KEY (Id)
);


-- db_ac0b5e_hospitalqueue.dbo.Pacientes definition

-- Drop table

-- DROP TABLE db_ac0b5e_hospitalqueue.dbo.Pacientes;

CREATE TABLE db_ac0b5e_hospitalqueue.dbo.Pacientes (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Identificacion nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FechaNacimiento date NOT NULL,
	Genero nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Sintomas nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	FechaRegistro datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK__Paciente__3214EC078A83D3B1 PRIMARY KEY (Id),
	CONSTRAINT UQ__Paciente__D6F931E555AC69CA UNIQUE (Identificacion)
);
ALTER TABLE db_ac0b5e_hospitalqueue.dbo.Pacientes WITH NOCHECK ADD CONSTRAINT CK__Pacientes__Gener__34C8D9D1 CHECK (([Genero]='Otro' OR [Genero]='Femenino' OR [Genero]='Masculino'));


-- db_ac0b5e_hospitalqueue.dbo.Turnos definition

-- Drop table

-- DROP TABLE db_ac0b5e_hospitalqueue.dbo.Turnos;

CREATE TABLE db_ac0b5e_hospitalqueue.dbo.Turnos (
	Id int IDENTITY(1,1) NOT NULL,
	PacienteId int NOT NULL,
	ClinicaId int NOT NULL,
	NumeroTurno int NOT NULL,
	Estado nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT 'Pendiente' NOT NULL,
	FechaCreacion datetime2 DEFAULT getutcdate() NOT NULL,
	FechaLlamado datetime2 NULL,
	FechaAtencion datetime2 NULL,
	MedicoId int NULL,
	Observaciones nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK__Turnos__3214EC07B76A6916 PRIMARY KEY (Id),
	CONSTRAINT FK__Turnos__ClinicaI__3C69FB99 FOREIGN KEY (ClinicaId) REFERENCES db_ac0b5e_hospitalqueue.dbo.Clinicas(Id),
	CONSTRAINT FK__Turnos__MedicoId__403A8C7D FOREIGN KEY (MedicoId) REFERENCES db_ac0b5e_hospitalqueue.dbo.Usuarios(Id),
	CONSTRAINT FK__Turnos__Paciente__3B75D760 FOREIGN KEY (PacienteId) REFERENCES db_ac0b5e_hospitalqueue.dbo.Pacientes(Id)
);
ALTER TABLE db_ac0b5e_hospitalqueue.dbo.Turnos WITH NOCHECK ADD CONSTRAINT CK__Turnos__Estado__3E52440B CHECK (([Estado]='Cancelado' OR [Estado]='Atendido' OR [Estado]='EnAtencion' OR [Estado]='Llamado' OR [Estado]='Pendiente'));

-- db_ac0b5e_hospitalqueue.dbo.Usuarios definition

-- Drop table

-- DROP TABLE db_ac0b5e_hospitalqueue.dbo.Usuarios;

CREATE TABLE db_ac0b5e_hospitalqueue.dbo.Usuarios (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PasswordHash nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Rol nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FechaCreacion datetime2 DEFAULT getutcdate() NOT NULL,
	CONSTRAINT PK__Usuarios__3214EC07AD21373E PRIMARY KEY (Id),
	CONSTRAINT UQ__Usuarios__A9D10534941EB2D2 UNIQUE (Email)
);
ALTER TABLE db_ac0b5e_hospitalqueue.dbo.Usuarios WITH NOCHECK ADD CONSTRAINT CK__Usuarios__Rol__300424B4 CHECK (([Rol]='Medico' OR [Rol]='Enfermero' OR [Rol]='Recepcion'));