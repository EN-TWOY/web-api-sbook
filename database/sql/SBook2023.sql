use master
go

if exists(Select * from sys.databases  Where name='Sbook2023')
Begin
	Drop Database Sbook2023
End
go

create database Sbook2023
go

use Sbook2023
go

set dateformat ymd
go

CREATE TABLE tb_categorias (
  IdCategoria int IDENTITY(1,1) PRIMARY KEY,
  NombreCategoria varchar(20) NOT NULL,
  Descripcion text
)

INSERT INTO tb_categorias VALUES
('Misterio', 'Historias que involucran elementos sobrenaturales o inexplicables'),
('Fantas�a', 'Historias que involucran elementos imaginarios o fant�sticos'),
('Ciencia Ficci�n', 'Historias que involucran ciencia y tecnolog�a futurista'),
('Terror', 'Historias que buscan provocar miedo o terror en el lector'),
('Historia', 'Historias basadas en eventos y personajes hist�ricos reales'),
('Biograf�as', 'Libros que cuentan la vida de personas famosas o notables'),
('Romance', 'Historias de amor y relaciones rom�nticas'),
('Arte', 'Libros que exploran y analizan el arte y sus formas'),
('Aventura', 'Historias emocionantes y llenas de acci�n'),
('Autoayuda', 'Libros que brindan consejos y t�cnicas para mejorar la vida personal'),
('Cocina', 'Libros de recetas y t�cnicas de cocina'),
('Negocios', 'Libros relacionados con el mundo empresarial y las estrategias comerciales'),
('Fantas�a �pica', 'Historias de fantas�a con mundos extensos y tramas complejas'),
('Policial', 'Historias de detectives y cr�menes'),
('Infantil', 'Libros dirigidos a ni�os y ni�as'),
('Suspenso', 'Historias que mantienen al lector en constante expectativa y tensi�n'),
('Cient�fico', 'Libros que exploran conceptos cient�ficos y descubrimientos'),
('Viajes', 'Libros sobre experiencias de viajes y lugares interesantes'),
('Historia Antigua', 'Historias basadas en eventos y personajes de la antig�edad'),
('Ciencia', 'Libros que abordan temas cient�ficos y descubrimientos recientes');
go

CREATE TABLE tb_paises (
  Idpais int IDENTITY(1,1) PRIMARY KEY,
  NombrePais varchar(40) not null
  )
go

INSERT INTO tb_paises VALUES
('Per�'),
('Estados Unidos'),
('Canad�'),
('Espa�a'),
('Alemania'),
('Francia'),
('Reino Unido'),
('China'),
('Jap�n'),
('Brasil');
go

CREATE TABLE tb_clientes (
  IdCliente int IDENTITY(1,1) PRIMARY KEY,
  NombreCia varchar(40) not null,
  Direccion varchar(60) not null,
  idpais int references tb_paises,
  Telefono varchar(24) not null
)
go

INSERT INTO tb_clientes VALUES
('Alexander Luna Sorian Valdez', 'Av. Sotomayor Str 57', '2', '924345345'),
('Mar�a Fernanda Rodr�guez L�pez', 'Avenida Libertad 456', '7', '923456789'),
('Luisa Gabriela Garc�a Mendoza', 'Carrera del Sol 789', '1', '934567890'),
('Carlos Eduardo L�pez Ram�rez', 'Pasaje Flores 321', '5', '945678901'),
('Laura Alejandra Hern�ndez Torres', 'Ruta del Mar 654', '3', '956789012'),
('Pedro Antonio G�mez S�nchez', 'Callej�n Norte 987', '6', '967890123'),
('Ana Mar�a Ram�rez Vargas', 'Avenida Principal 234', '9', '978901234'),
('Roberto Andr�s Torres Gonz�lez', 'Calle del Bosque 567', '2', '989012345'),
('Sof�a Valentina Navarro Vega', 'Calle Mayor 890', '8', '990123456'),
('Javier Alejandro Mendez Ortiz', 'Avenida del Sol 012', '10', '991234567');
go

CREATE TABLE tb_proveedores (
  IdProveedor int IDENTITY(1,1) PRIMARY KEY,
  NombreCia varchar(40) not null,
  NombreContacto varchar(30) not null,
  CargoContacto varchar(30) not null,
  Direccion varchar(60) not null,
  idpais int references tb_paises,
  Telefono varchar(24) not null,
  Fax varchar(24) not null
)
go

INSERT INTO tb_proveedores VALUES
('Editorial Per�', 'Cristian P�rez Pari', 'Gerente de compras', 'Calle Principal 123', '1', '912345678', 'juan@ediperu.com'),
('Libros Andinos', 'Mar�a Rodr�guez Meli', 'Gerente de compras', 'Avenida Libertad 456', '1', '923456789', 'maria@libandinos.com'),
('Editorial Inka', 'Luisa Garc�a Vazquez', 'Gerente de compras', 'Carrera del Sol 789', '1', '934567890', 'luisa@editinka.com'),
('P�ginas Peruanas', 'Carlos L�pez Soto', 'Gerente de compras', 'Pasaje Flores 321', '1', '945678901', 'carlos@pagperuanas.com'),
('Ediciones Lima', 'Laura Hern�ndez Solar', 'Gerente de compras', 'Ruta del Mar 654', '1', '956789012', 'laura@edilima.com'),
('Editorial Andes', 'Pedro G�mez Dias', 'Gerente de compras', 'Callej�n Oscuro 987', '1', '967890123', 'pedro@editandes.com'),
('Librer�a Nacional', 'Ana Ram�rez Fial', 'Gerente de compras', 'Avenida Principal 234', '1', '978901234', 'ana@libnacional.com'),
('Editorial Lima', 'Roberto Torres Rashford', 'Gerente de compras', 'Calle del Bosque 567', '1', '989012345', 'roberto@edilima.com'),
('Libros Peruanos', 'Sof�a Navarro Luna', 'Gerente de compras', 'Calle Mayor 890', '1', '990123456', 'sofia@librosperuanos.com'),
('Ediciones Cusco', 'Javier Mendez Ferran', 'Gerente de compras', 'Avenida del Sol 012', '1', '991234567', 'javier@edicionscusco.com');
go

CREATE TABLE tb_productos (
  IdProducto int IDENTITY(1,1) PRIMARY KEY,
  NombreProducto varchar(40) not null,
  IdProveedor int References tb_proveedores,
  IdCategoria int References tb_categorias,
  umedida varchar(100),
  PrecioUnidad decimal(10,0) not null,
  UnidadesEnExistencia smallint not null)
go

INSERT INTO tb_productos VALUES
('Cien a�os de soledad', '1', '1','Gabriel Garc�a M�rquez' ,'18', '39'),
('El gran Gatsby', '2', '2', 'F. Scott Fitzgerald', '20', '45'),
('Don Quijote de la Mancha', '3', '3', 'Miguel de Cervantes', '25', '55'),
('Orgullo y prejuicio', '4', '4', 'Jane Austen', '15', '32'),
('1984', '5', '5', 'George Orwell', '22', '47'),
('Matar a un ruise�or', '6', '8', 'Harper Lee', '19', '41'),
('Ulises', '7', '3', 'James Joyce', '24', '52'),
('En busca del tiempo perdido', '8', '6', 'Marcel Proust', '28', '61'),
('La Odisea', '9', '2', 'Homero', '18', '39'),
('Hamlet', '10', '7', 'William Shakespeare', '21', '46'),
('Cr�nica de una muerte anunciada', '7', '5', 'Gabriel Garc�a M�rquez', '17', '36'),
('El alquimista', '6', '3', 'Paulo Coelho', '8', '12'),
('La sombra del viento', '7', '4', 'Carlos Ruiz Zaf�n', '9', '15'),
('El nombre del viento', '8', '5', 'Patrick Rothfuss', '7', '10'),
('Las intermitencias de la muerte', '5', '2', 'Jos� Saramago', '6', '8'),
('Fahrenheit 451', '7', '3', 'Ray Bradbury', '5', '9');
go

CREATE TABLE tb_empleados (
  IdEmpleado int IDENTITY(1,1) PRIMARY KEY,
  ApeEmpleado varchar(50) not null,
  NomEmpleado varchar(50) not null,
  FecNac datetime not null,
  DirEmpleado varchar(60) not null,
  idDistrito int,
  fonoEmpleado varchar(15) NULL,
  idCargo int,
  FecContrata datetime not null
)
go

INSERT INTO tb_empleados VALUES
('Manuel Sorial', 'Vargas Mendez', '1988-12-08','Calle Las Magnolias 123', 2,'987657334', 4,'2010-02-10'),
('Manuel Sorial', 'Vargas Mendez', '1988-12-08','Calle Las Magnolias 123', 2,'987657334', 4,'2010-02-10'),
('John Doe', 'Smith Tomas', '1990-05-15', '123 Main Street', 1, '123456789', 3, '2015-06-20'),
('Jane Smith', 'Johnson Real', '1992-08-22', '456 Elm Avenue', 1, '987654321', 2, '2014-03-10'),
('Maria Garcia', 'Lopez Mareh', '1985-07-12', '789 Oak Lane', 3, '456789123', 1, '2009-11-05'),
('Pedro Rodriguez', 'Martinez Salar', '1993-03-28', '321 Pine Road', 2, '789123456', 2, '2018-09-15'),
('Laura Fernandez', 'Gomez Borth', '1991-09-20', '567 Cedar Street', 3, '654321987', 3, '2013-08-08'),
('Carlos Sanchez', 'Perez Antu', '1987-06-05', '987 Willow Avenue', 1, '321654987', 4, '2012-04-25'),
('Sofia Ramirez', 'Torres Veriz', '1994-01-10', '654 Birch Lane', 2, '987123654', 2, '2016-07-12'),
('Daniel Castro', 'Rojas Alars', '1989-04-18', '852 Maple Drive', 3, '654987321', 1, '2011-12-01'),
('Ana Morales', 'Hernandez Suares', '1992-11-30', '753 Elm Street', 1, '321987654', 3, '2017-03-18');
go

CREATE TABLE tb_pedidoscabe (
  IdPedido int IDENTITY(1,1) PRIMARY KEY,
  IdCliente int references tb_clientes,
  IdEmpleado int references tb_empleados,
  FechaPedido datetime not null Default(getdate()),
  FechaEntrega datetime null,
  FechaEnvio datetime null,
  Envio char(1) null Default('0'),
  Cargo decimal(10,0) null,
  Destinatario varchar(40) null,
  DireccionDestinatario varchar(60) null,
  CiudadDestinatario varchar(15) null,
  RegionDestinatario varchar(15) null,
  CodPostalDestinatario varchar(10) null,
  PaisDestinatario varchar(15) null,  
) 
go

INSERT INTO tb_pedidoscabe VALUES
('1', '1', '2010-07-04', '2011-08-01', '1996-07-16', '1', '32', 'Charlotte Villanueva Solar', 'Keskuskatu 45', 'Lima', 'Chorrillos', '21240', 'Peru')
go

CREATE TABLE tb_pedidosdeta (
  IdPedido int References tb_pedidoscabe,
  IdProducto int References tb_productos,
  PrecioUnidad decimal(10,0) not null,
  Cantidad smallint not null,
  Descuento float not null
)
go

INSERT INTO tb_pedidosdeta VALUES
('1', '11', '14', '12', '0')
go

/* news */
create table tb_pedidos(
	idpedido int primary key,
	fpedido datetime default(getdate()),
	dni varchar(8),
	nombre varchar(255),
	email varchar(255)
)
go

create table tb_pedidos_deta(
	idpedido int references tb_pedidos,
	idproducto int references tb_productos,
	cantidad int,
	precio decimal
)
go

ALTER TABLE tb_clientes
ADD dni INT;
go

select * from tb_categorias
select * from tb_paises
select * from tb_clientes
select * from tb_proveedores
select * from tb_productos
select * from tb_empleados
select * from tb_pedidos
select * from tb_pedidos_deta
go