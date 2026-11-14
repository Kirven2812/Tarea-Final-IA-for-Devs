# SistemaBiblioteca

Sistema integral para la gestión de biblioteca desarrollado con arquitectura en capas sobre **.NET 10**, **ASP.NET Core Web API**, **Entity Framework Core (SQL Server)** y **Blazor WebAssembly**, con autenticación y autorización basada en **JWT (JSON Web Tokens)**.

---

## Tabla de Contenidos

- [Stack Tecnológico](#stack-tecnológico)
- [Arquitectura del Proyecto](#arquitectura-del-proyecto)
- [Estructura del Repositorio](#estructura-del-repositorio)
- [Funcionalidades Técnicas por Módulo](#funcionalidades-técnicas-por-módulo)
  - [1. Autenticación y Seguridad (JWT)](#1-autenticación-y-seguridad-jwt)
  - [2. Gestión de Usuarios](#2-gestión-de-usuarios)
  - [3. Catálogo y Gestión de Libros](#3-catálogo-y-gestión-de-libros)
  - [4. Gestión de Préstamos](#4-gestión-de-préstamos)
- [Endpoints de la API](#endpoints-de-la-api)
- [Requisitos e Instalación Local](#requisitos-e-instalación-local)
- [Integrantes del Proyecto](#integrantes-del-proyecto)

---

## Stack Tecnológico

| Capa | Tecnología / Paquete | Versión |
| :--- | :--- | :--- |
| **Runtime / SDK** | .NET | `10.0` (`net10.0`) |
| **Backend API** | ASP.NET Core Web API | `10.0` |
| **Documentación API** | Swashbuckle.AspNetCore (Swagger / OpenAPI) | `10.2.3` / `2.7.5` |
| **Autenticación** | Microsoft.AspNetCore.Authentication.JwtBearer | `10.0.12` |
| **ORM y Base de Datos** | Entity Framework Core SqlServer / Design / Tools | `10.0.13` / `10.0.12` |
| **Motor de BD** | SQL Server / LocalDB (`Biblioteca.bak`) | - |
| **Frontend** | Blazor WebAssembly (`Microsoft.AspNetCore.Components.WebAssembly`) | `10.0.10` |
| **Autorización Web** | `Microsoft.AspNetCore.Components.Authorization` | `10.0.12` |
| **Almacenamiento Local** | `Blazored.LocalStorage` (`Blazor.Storage`) | `5.0.0` |
| **Estilos UI** | Bootstrap 5 + Scoped CSS (`.razor.css`) | - |

---

## Arquitectura del Proyecto

La solución sigue una **Arquitectura en Capas (Clean Architecture / N-Tier)** separando responsabilidades de negocio, persistencia, exposición REST y cliente web:

1. **`SistemaBiblioteca.Dominio`**: Contiene las entidades del dominio (`Usuario`, `Libro`, `Prestamo`, `Autor`, `Categoria`) con encapsulamiento de propiedades (`private set`), validaciones de invariantes y enumeraciones (`RolUsuario`, `EstadoLibro`, `EstadoPrestamo`).
2. **`SistemaBiblioteca.Aplicacion`**: Define los contratos de repositorios y servicios (`IUsuarioRepository`, `ILibroRepository`, `IPrestamoRepository`, `IAutorRepository`, `ICategoriaRepository`, `IJwtTokenService`, `IPasswordHashService`), el modelo genérico `ResultadoPaginado<T>` y los casos de uso (`UsuarioService`, `LibroService`, `PrestamoService`).
3. **`SistemaBiblioteca.Infraestructura`**: Implementa el acceso a datos mediante `BibliotecaDbContext` (EF Core + SQL Server), los repositorios concretos, el historial de migraciones y el hashing de contraseñas (`PasswordHashService` usando `IPasswordHasher<Usuario>`).
4. **`SistemaBiblioteca.API`**: Expone los controladores REST (`UsuariosController`, `LibrosController`, `PrestamosController`), DTOs de entrada/salida, generación de tokens (`JwtTokenService`), políticas CORS (`BlazorClient`) y Swagger con esquema Bearer.
5. **`SistemaBiblioteca.Web`**: Aplicación SPA en Blazor WebAssembly con `CustomAuthenticationStateProvider` (parseo y verificación de expiración de JWT desde `localStorage`), servicios HTTP (`AuthService`, `LibroService`, `UsuarioService`) y vistas Razor protegidas por rol.

---

## Estructura del Repositorio

```text
Tarea-Final-IA-for-Devs/
├── .gitattributes
├── .gitignore
├── LICENSE
├── README.md
└── SistemaBiblioteca/
    ├── SistemaBiblioteca.slnx
    ├── BaseDeDatos/
    │   └── Biblioteca.bak
    │
    ├── SistemaBiblioteca.Dominio/
    │   ├── Entidades/
    │   │   ├── Autor.cs
    │   │   ├── Categoria.cs
    │   │   ├── Libro.cs
    │   │   ├── Prestamo.cs
    │   │   └── Usuario.cs
    │   └── Enums/
    │       ├── EstadoLibro.cs
    │       ├── EstadoPrestamo.cs
    │       └── RolUsuario.cs
    │
    ├── SistemaBiblioteca.Aplicacion/
    │   ├── Interfaces/
    │   │   ├── IAutorRepository.cs
    │   │   ├── ICategoriaRepository.cs
    │   │   ├── IJwtTokenService.cs
    │   │   ├── ILibroRepository.cs
    │   │   ├── IPasswordHashService.cs
    │   │   ├── IPrestamoRepository.cs
    │   │   └── IUsuarioRepository.cs
    │   ├── Modelos/
    │   │   └── ResultadoPaginado.cs
    │   └── Servicios/
    │       ├── LibroService.cs
    │       ├── PrestamoService.cs
    │       └── UsuarioService.cs
    │
    ├── SistemaBiblioteca.Infraestructura/
    │   ├── Migrations/
    │   ├── Persistencia/
    │   │   └── BibliotecaDbContext.cs
    │   ├── Repositorios/
    │   │   ├── AutorRepository.cs
    │   │   ├── CategoriaRepository.cs
    │   │   ├── LibroRepository.cs
    │   │   ├── PrestamoRepository.cs
    │   │   └── UsuarioRepository.cs
    │   └── Seguridad/
    │       └── PasswordHashService.cs
    │
    ├── SistemaBiblioteca.API/
    │   ├── Program.cs
    │   ├── appsettings.json
    │   ├── Controllers/
    │   │   ├── LibrosController.cs
    │   │   ├── PrestamosController.cs
    │   │   └── UsuariosController.cs
    │   ├── DTOs/
    │   │   ├── CambiarPasswordDto.cs
    │   │   ├── CambiarRolUsuarioDto.cs
    │   │   ├── CrearPrestamoDto.cs
    │   │   ├── CrearUsuarioDto.cs
    │   │   ├── LibroDto.cs
    │   │   ├── LoginDto.cs
    │   │   ├── PrestamoDto.cs
    │   │   └── UsuarioDto.cs
    │   └── Servicios/
    │       └── JwtTokenService.cs
    │
    └── SistemaBiblioteca.Web/
        ├── App.razor
        ├── Program.cs
        ├── _Imports.razor
        ├── Authenttication/
        │   └── CustomAuthenticationStateProvider.cs
        ├── Layout/
        │   ├── MainLayout.razor
        │   └── NavMenu.razor
        ├── Modelos/
        │   ├── CambiarPasswordModel.cs
        │   ├── CambiarRolModel.cs
        │   ├── CrearUsuarioModel.cs
        │   ├── LibroModel.cs
        │   ├── LoginRequest.cs
        │   ├── LoginResponse.cs
        │   ├── ResultadoPaginadoModel.cs
        │   └── UsuarioModel.cs
        ├── Pages/
        │   ├── Catalogo.razor
        │   ├── Login.razor
        │   ├── NotFound.razor
        │   ├── RedirectToLogin.razor
        │   ├── Usuarios.razor
        │   └── Usuarios.razor.css
        └── Servicios/
            ├── AuthService.cs
            ├── LibroService.cs
            └── UsuarioService.cs
```

---

## Funcionalidades Técnicas por Módulo

### 1. Autenticación y Seguridad (JWT)
- **Emisión de Tokens (`JwtTokenService`)**: Firma tokens con `HmacSha256` incluyendo los claims `NameIdentifier` (ID de usuario), `Name`, `Email` y `Role` (`Usuario` o `Administrador`).
- **Validación en Backend**: Configurada mediante `JwtBearerDefaults` validando `Issuer`, `Audience`, `Lifetime` e `IssuerSigningKey`.
- **Estado de Sesión en Frontend (`CustomAuthenticationStateProvider`)**:
  - Almacena y recupera el JWT desde `localStorage` bajo la clave `authToken`.
  - Decodifica el payload Base64 del JWT, verifica expiración (`exp`) y configura el encabezado `Authorization: Bearer <token>` en `HttpClient`.
  - Redirige a `/login` (`RedirectToLogin.razor`) ante rutas protegidas no autorizadas.

### 2. Gestión de Usuarios
- **Backend (`UsuariosController` / `UsuarioService`)**:
  - Alta de usuarios validando unicidad de correo electrónico y generando hash seguro de contraseña.
  - Cambio de contraseña y consulta de perfil autenticado (`/mi-perfil`).
  - Cambio de rol (`Usuario` / `Administrador`) con protección para impedir que un administrador modifique su propio rol.
  - Baja y alta lógica (`Activar` / `Desactivar`) con validación para evitar la autodesactivación del administrador autenticado.
- **Frontend (`/administracion/usuarios` - `Usuarios.razor`)**:
  - Acceso restringido con `@attribute [Authorize(Roles = "Administrador")]`.
  - **Tarjetas KPI**: Conteo en tiempo real de Total de Usuarios, Activos, Inactivos y Administradores.
  - **Filtros combinados**: Búsqueda reactiva por nombre/apellido/correo, selector de rol y selector de estado.
  - **Modal de Registro**: Ventana modal centrada con validaciones `DataAnnotations` (`CrearUsuarioModel`).
  - **Acciones directas**: Cambio de rol, activación/desactivación y panel de restablecimiento de contraseña.

### 3. Catálogo y Gestión de Libros
- **Backend (`LibrosController` / `LibroService`)**:
  - CRUD completo de libros identificados por `ISBN` (`Crear`, `Actualizar`, `Eliminar` exclusivos para `Administrador`).
  - Búsqueda paginada (`GET api/Libros/buscar`) con filtros opcionales por `titulo`, `autor`, `categoria`, `estado`, `pagina` y `tamañoPagina`.
- **Frontend (`/catalogo` - `Catalogo.razor`)**:
  - Acceso autenticado (`@attribute [Authorize]`).
  - Barra de filtros por título, autor, categoría y estado (`Disponible` / `Prestado`), mostrando tarjetas de libros y paginación.

### 4. Gestión de Préstamos
- **Backend (`PrestamosController` / `PrestamoService`)**:
  - Registro de préstamo (`POST api/Prestamos`) asociando automáticamente al usuario autenticado mediante el claim `NameIdentifier`.
  - Devolución de libro (`PUT api/Prestamos/{id}/devolver`) con validación de propiedad/autorización (`403 Forbidden` si no corresponde).
  - Consulta de préstamos propios (`GET api/Prestamos/mis-prestamos`) y listado global exclusivo para administradores (`GET api/Prestamos`).

---

## Endpoints de la API

### Usuarios (`/api/Usuarios`)
| Método | Ruta | Acceso | Descripción |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Usuarios/login` | Público | Autentica credenciales y devuelve usuario + token JWT |
| `GET` | `/api/Usuarios/mi-perfil` | Autenticado | Devuelve la información del usuario autenticado |
| `PUT` | `/api/Usuarios/{id}/password` | Autenticado | Actualiza la contraseña del usuario |
| `GET` | `/api/Usuarios` | Administrador | Lista todos los usuarios registrados |
| `GET` | `/api/Usuarios/{id}` | Administrador | Obtiene un usuario por su ID |
| `POST` | `/api/Usuarios` | Administrador | Crea un nuevo usuario |
| `PUT` | `/api/Usuarios/{id}/rol` | Administrador | Cambia el rol (`0 = Usuario`, `1 = Administrador`) |
| `PUT` | `/api/Usuarios/{id}/activar` | Administrador | Activa un usuario |
| `PUT` | `/api/Usuarios/{id}/desactivar` | Administrador | Desactiva un usuario (borrado lógico) |

### Libros (`/api/Libros`)
| Método | Ruta | Acceso | Descripción |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Libros` | Autenticado | Lista todos los libros |
| `GET` | `/api/Libros/{isbn}` | Autenticado | Obtiene un libro por ISBN |
| `GET` | `/api/Libros/buscar` | Autenticado | Búsqueda paginada con filtros |
| `POST` | `/api/Libros` | Administrador | Registra un nuevo libro |
| `PUT` | `/api/Libros/{isbn}` | Administrador | Actualiza los datos de un libro |
| `DELETE` | `/api/Libros/{isbn}` | Administrador | Elimina un libro por ISBN |

### Préstamos (`/api/Prestamos`)
| Método | Ruta | Acceso | Descripción |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Prestamos` | Autenticado | Registra un nuevo préstamo |
| `PUT` | `/api/Prestamos/{id}/devolver` | Autenticado | Registra la devolución de un préstamo |
| `GET` | `/api/Prestamos/mis-prestamos` | Autenticado | Lista los préstamos del usuario actual |
| `GET` | `/api/Prestamos/{id}` | Público / API | Obtiene el detalle de un préstamo |
| `GET` | `/api/Prestamos` | Administrador | Lista todos los préstamos del sistema |

---

## Requisitos e Instalación Local

1. **Requisitos previos**:
   - [.NET 10 SDK](https://dotnet.microsoft.com/download)
   - SQL Server / SQL Server LocalDB

2. **Base de datos**:
   - Restaura el respaldo ubicado en `SistemaBiblioteca/BaseDeDatos/Biblioteca.bak`, o bien ejecuta las migraciones de Entity Framework Core sobre `SistemaBiblioteca.Infraestructura`:
     ```bash
     cd SistemaBiblioteca
     dotnet ef database update --project SistemaBiblioteca.Infraestructura --startup-project SistemaBiblioteca.API
     ```
   - Configura la cadena de conexión `"Biblioteca"` y la clave `"Jwt:Key"` en `SistemaBiblioteca.API/appsettings.json` o mediante `dotnet user-secrets`.

3. **Ejecutar el Backend (API + Swagger)**:
   ```bash
   dotnet run --project SistemaBiblioteca/SistemaBiblioteca.API/SistemaBiblioteca.API.csproj --launch-profile http
   ```
   - URL de la API / Swagger: `http://127.0.0.1:5000`

4. **Ejecutar el Frontend (Blazor WebAssembly)**:
   ```bash
   dotnet run --project SistemaBiblioteca/SistemaBiblioteca.Web/SistemaBiblioteca.Web.csproj --launch-profile http
   ```
   - URL de la Web: `http://localhost:5279`

---

## Integrantes del Proyecto

| Nombre Completo | Correo Electrónico |
| :--- | :--- |
| Rosa Gladis Ruiz Tejeda | rossyruizr123@gmail.com |
| Pedro Emerson Morales Ferrer | pedromoralesferrer4@gmail.com |
| Carlos Isaac Flores Aparicio | carlosisaac03@gmail.com |
