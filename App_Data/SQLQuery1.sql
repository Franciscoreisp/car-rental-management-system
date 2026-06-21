CREATE TABLE utilizador (
    idutil INT PRIMARY KEY IDENTITY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(100) NOT NULL,
    nome NVARCHAR(100)
)

INSERT INTO utilizador VALUES ('admin', 'admin123', 'Administrador')