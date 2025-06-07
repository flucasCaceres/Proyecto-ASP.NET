CREATE DATABASE Proyecto;
USE Proyecto;

CREATE USER 'DBA_Proyecto'@'localhost' IDENTIFIED BY 'DBA020625';
GRANT ALL PRIVILEGES ON Proyecto.* TO 'DBA_Proyecto'@'localhost';

CREATE USER 'User_1'@'localhost' IDENTIFIED BY 'USER1020625';

show tables from Proyecto;

describe aspnetuserclaims;

select * from aspnetuserclaims; -- contraseña: Lucas2025!
select * from aspnetusers; -- contraseña: Test123!
select * from aspnetroles;

Start TRANSACTION;

-- 1. Eliminar de tablas dependientes
DELETE FROM AspNetUserLogins WHERE UserId = (SELECT Id FROM AspNetUsers WHERE email = 'lucascaceres614@gmail.com');
DELETE FROM AspNetUserRoles WHERE UserId = (SELECT Id FROM AspNetUsers WHERE email = 'lucascaceres614@gmail.com');
DELETE FROM AspNetUserClaims WHERE UserId = (SELECT Id FROM AspNetUsers WHERE email = 'lucascaceres614@gmail.com');

SET SQL_SAFE_UPDATES = 0;
-- Tu consulta DELETE aquí
DELETE FROM AspNetUsers WHERE email = 'lucascaceres614@gmail.com';
SET SQL_SAFE_UPDATES = 1; -- Reactivar modo seguro

COMMIT;