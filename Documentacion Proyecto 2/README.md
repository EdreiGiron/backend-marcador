# Documentación Proyecto I - Segunda parte : Marcador de baloncesto en tiempo real

## 1. Introducción
API REST en .NET 8 (C#) para un marcador de baloncesto en tiempo real.  
Para la Segunda Entrega se incorporan capacidades administrativas (ABM de equipos, jugadores, torneos y partidos), autenticación/autorización con JWT, filtros avanzados de historial y lineamientos de despliegue en VPS Linux.

---

## 2. Alcance y Requisitos

### 2.1 Requisitos funcionales
- **Marcador en tiempo real**: puntos (±1/2/3), faltas, cuartos, temporizador.
- **Gestión de partidos**: creación, estados (Creado/EnCurso/Suspendido/Finalizado), roster por partido e historial.
- **Administración**:
  - **Equipos**: crear/editar/eliminar; búsqueda/filtrado; evitar duplicados por nombre.
  - **Jugadores**: crear/editar/eliminar; asociados a equipo; lista por equipo (campos sugeridos: nombre completo, número, posición, altura, edad, nacionalidad).
  - **Torneos**: crear/editar/eliminar; unicidad por Nombre + Año.
  - **Partidos**: crear con equipos, torneo (opcional), ronda/fecha/hora, duración de cuartos; asignar roster.
- **Historial**: listado con filtros por equipo, torneo (por nombre) y ronda; re-apertura por matchId.

### 2.2 Requisitos no funcionales
- **Autenticación/Autorización**: JWT Bearer, expiración/refresh; rutas protegidas para administración.
- **Integridad de datos**: validaciones de duplicados; índices únicos.
- **Observabilidad**: logging estructurado; correlación por matchId.
- **Despliegue**: Docker y/o VPS Linux con systemd.
- **Seguridad**: CORS restringido, validación de entrada, manejo centralizado de errores.

---

## 3. Arquitectura y Componentes
- **API REST ASP.NET Core 8** + **SignalR** (hub para broadcasting por matchId).
- **EF Core** + **SQL Server 2022**.
- **JWT** (Issuer, Audience, Key por variables de entorno).
- **Capas sugeridas**:
  - Controllers: Auth, Equipos, Jugadores, Torneos, Partidos, Historial.
  - Servicios: reglas de negocio y orquestación.
  - Repositorios: acceso a datos (opcional si usas EF directo).
  - DTOs / Validadores: DataAnnotations/FluentValidation.
  - Infraestructura: DbContext, migraciones, configuración.
  - Tiempo real: MarcadorHub (SignalR) con grupos por matchId.

---

## 4. Modelo de Datos

**Equipo**  
- `Id (PK)`, `Nombre`, `Ciudad`, `LogoUrl`, `FechaCreacion`.

**Jugador**  
- `Id (PK)`, `EquipoId (FK)`, `NombreCompleto`, `Numero`, `Posicion`, `Altura`, `Edad`, `Nacionalidad`, `Activo`.

**Torneo**  
- `Id (PK)`, `Nombre`, `Anio`, `UniqueIndex(Nombre,Anio)`.

**Partido**  
- `Id (PK)`, `MatchId (unique)`, `EquipoLocalId (FK)`, `EquipoVisitanteId (FK)`, `TorneoId (FK, null)`, `Ronda`, `FechaHora`, `DuracionCuartoSeg`, `Estado`.

**PartidoJugador**  
- `Id (PK)`, `PartidoId (FK)`, `JugadorId (FK)`, `EquipoId (FK)`, `Unico(PartidoId,JugadorId)`.

**MarcadorActual**  
- `PartidoId (PK/FK)`, `PtsLocal`, `PtsVisitante`, `FaltasLocal`, `FaltasVisitante`, `Cuarto`, `TiempoRestanteSeg`, `ActualizadoEn`.

**Usuario**, **Rol**, **UsuarioRol**.

> **Índices/Únicos**:  
> - `Equipo.Nombre`  
> - `Torneo (Nombre, Anio)`  
> - `Partido.MatchId`  
> - `PartidoJugador (PartidoId, JugadorId)`

---

## 5. Seguridad (JWT)
- **Login**: POST /api/auth/login → { accessToken, refreshToken, expiresIn }.
- **Refresh**: POST /api/auth/refresh → nuevo accessToken.
- **Protección**: [Authorize] en endpoints administrativos; políticas por rol si aplica.
- **Buenas prácticas**: expiración corta del Access Token; rotación de Refresh Token; invalidación en logout.

### 5.1 Configuración en `Program.cs`
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["JWT:Issuer"],
      ValidAudience = builder.Configuration["JWT:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(
         Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
    };
  });

builder.Services.AddAuthorization();
```

---

## 6. Endpoints REST

### 6.1 Autenticación
- `POST /api/auth/login` → 200 con tokens; 401 credenciales inválidas.
- `POST /api/auth/refresh` → 200 con nuevo accessToken; 401/400 si inválido/expirado.
- `POST /api/auth/logout`.

### 6.2 Equipos (Admin) 
- `GET /api/equipos?search=`: lista filtrada.
- `POST /api/equipos` (valida único por nombre).
- `PUT /api/equipos/{id}`.
- `DELETE /api/equipos/{id}`.

### 6.3 Jugadores (Admin) 
- `GET /api/jugadores?equipoId=&search=`.
- `POST /api/jugadores`.
- `PUT /api/jugadores/{id}`.
- `DELETE /api/jugadores/{id}`.

### 6.4 Torneos (Admin)
- `GET /api/torneos?search=`
- `POST /api/torneos` 
- `PUT /api/torneos/{id}`
- `DELETE /api/torneos/{id}`

### 6.5 Partidos (Admin)
- `POST /api/partidos`  
  Crea partido con EquipoLocalId, EquipoVisitanteId, TorneoId, Ronda, FechaHora, DuracionCuartoSeg.  
  → 201 con matchId.
- `GET /api/partidos/{matchId}`  
  Snapshot completo (marcador actual, metadatos, equipos, torneo, ronda).
- `POST /api/partidos/{matchId}/estado` `{ estado }` → cambia estado (validaciones de transición).
- **Roster**:  
  - POST /api/partidos/{matchId}/roster { jugadores:[{jugadorId, equipoId}] }  
  - GET /api/partidos/{matchId}/roster

### 6.6 Marcador / Juego
- POST /api/partidos/{matchId}/puntos { equipoId, valor: 1|2|3, operacion: "sumar"|"restar" }
- POST /api/partidos/{matchId}/faltas { equipoId, jugadorId? }
- POST /api/partidos/{matchId}/tiempo { accion: "start"|"pause"|"reset", tiempoRestanteSeg? }
- POST /api/partidos/{matchId}/cuarto/avanzar

> Cada comando persiste y **publica** evento por **SignalR** al grupo matchId.



## 7. Validaciones y Duplicados
- **Equipo**: antes de insertar, AnyAsync(e => e.Nombre == Normalizar(dto.Nombre)).
- **Torneo**: validar par (Nombre, Anio).
- **Jugador**: (opcional) validar Numero único por EquipoId.
- **DB**: refuerzo con índices únicos; retornar 409 Conflict si viola unicidad.

---

## 8. Errores y Respuestas
- Middleware global para excepciones → JSON { traceId, message, details? }.
- Códigos comunes: 400 (validación), 401/403 (authz), 404 (no encontrado), 409 (duplicado), 422 (regla de dominio), 500 (error inesperado).
- Logging con ILogger y scopes por matchId.

---

## 9. Configuración (Program.cs — esquema)
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
builder.Services.AddAuthentication(...);
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(o => o.AddPolicy("Frontend", p => p
  .WithOrigins(builder.Configuration["App:FrontendUrl"]!)
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowCredentials()));

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MarcadorHub>("/hubs/marcador");

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.Run();
```

---

## 10. Pruebas
- **Unitarias**: servicios de dominio (puntos, transición de estado del partido, reglas de cuarto, duplicados).
- **Integración**: WebApplicationFactory para endpoints (Auth, ABM, Historial).
- **Contratos**: verificación OpenAPI/Swagger; Postman/Newman para regresión.

---

## 11. Despliegue en VPS Linux
1. **Usuario y SSH**: crear usuario, agregar llave en ~/.ssh/authorized_keys.
2. **Publicar API**: dotnet publish -c Release → carpeta publish/.
3. **Systemd unit** /etc/systemd/system/marcador-api.service:
   ```ini
   [Unit]
   Description=Marcador API
   After=network.target

   [Service]
   WorkingDirectory=/opt/marcador/api
   ExecStart=/usr/bin/dotnet /opt/marcador/api/MarcadorApi.dll
   Restart=always
   Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
   Environment=ConnectionStrings__DefaultConnection=Server=...
   Environment=JWT__Issuer=...
   Environment=JWT__Audience=...
   Environment=JWT__Key=...
   User=melgust

   [Install]
   WantedBy=multi-user.target
   ```
4. sudo systemctl daemon-reload && sudo systemctl enable --now marcador-api.

---

## 12. OpenAPI 
- Versionar (/api/v1/...), describir DTOs (Auth, Equipo, Jugador, Torneo, Partido, Historial).
- Definir errores comunes (ProblemDetails), 401/403 y 409 para duplicados.

---

## 13. Integrantes
- Edrei Andrés Girón Leonardo / 7690-21-218
- Diego Fernando Velásquez Pichilla / 7690-16-3882
- Edward Alexander Aguilar Flores / 7690-21-7651
---