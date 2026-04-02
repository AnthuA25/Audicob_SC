# Backend - Gestión de Cobranza (.NET + PostgreSQL)

Este proyecto corresponde al **backend** del sistema de Gestión de Cobranza, desarrollado con **.NET (ASP.NET Core Web API)** y **PostgreSQL** siguiendo una arquitectura limpia (**Clean Architecture**).

---

## Tecnologías usadas

- .NET (ASP.NET Core Web API)
- Entity Framework Core
- PostgreSQL (Npgsql)
- JWT (autenticación)


---

## Estructura del proyecto

```bash
backend/
├── GestionCobranza.API/
├── GestionCobranza.Application/
├── GestionCobranza.Domain/
├── GestionCobranza.Infrastructure/
└── GestionCobranza.sln
```


---

## Explicación de cada capa

### 🟣 GestionCobranza.API
Capa de entrada del sistema.

**Contiene:**
- Controllers (endpoints HTTP)
- Configuración de la aplicación
- JWT (autenticación)
- CORS
- Swagger

**Responsabilidad:**
Recibir peticiones del frontend y devolver respuestas.

---

### 🟡 GestionCobranza.Application
Capa de lógica de aplicación.

**Contiene:**
- DTOs (datos de entrada/salida)
- Interfaces de servicios
- Servicios (casos de uso)
- Mapeos

**Responsabilidad:**
Implementar la lógica del negocio sin depender de infraestructura.

---

### 🔵 GestionCobranza.Domain
Capa central del negocio.

**Contiene:**
- Entidades (Usuario, Cliente, etc.)
- Enums
- Interfaces base
- Reglas de negocio

**Responsabilidad:**
Definir el modelo del sistema.

---

### 🟢 GestionCobranza.Infrastructure
Capa de acceso a datos.

**Contiene:**
- DbContext (Entity Framework)
- Repositorios
- Configuración de base de datos
- Implementaciones de interfaces

**Responsabilidad:**
Conectar con PostgreSQL y manejar persistencia.

---

## Estructura interna recomendada

### Domain
```bash
Domain/
├── Entities/
├── Enums/
├── Common/
└── Interfaces/
```

### Application
```bash
Application/
├── DTOs/
├── Interfaces/
├── Services/
└── Mappings/
```

### Infrastructure
```bash
Infrastructure/
├── Data/
├── Repositories/
├── Configurations/
└── DependencyInjection.cs
```

### API

```bash
API/
├── Controllers/
├── Middlewares/
├── appsettings.json
└── Program.cs
```

---

## Reglas de arquitectura

- Controllers no deben tener lógica de negocio
- Services contienen la lógica
- Repositories acceden a la base de datos
- Domain no depende de ninguna otra capa
- Infrastructure implementa interfaces de Application

---

## Cómo clonar el proyecto

```bash
git clone <URL_DEL_REPOSITORIO>
cd backend
```

## Requisitos previos

### Verificar instalación de .NET:
```bash
dotnet --version
```
Instalar herramienta EF (si no está instalada):
```bash
dotnet tool install --global dotnet-ef
```

### Restaurar dependencias
```bash
dotnet restore
```

### Compilar proyecto
```bash
dotnet build
```

### Configurar base de datos (PostgreSQL)

Editar archivo:

```bash
GestionCobranza.API/appsettings.json
```
Ejemplo:

```bash
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=gestion_cobranza_db;Username=postgres;Password=tu_password"
}
```

### Ejecutar migraciones

Crear migración:

```bash
dotnet ef migrations add InitialCreate --project GestionCobranza.Infrastructure --startup-project GestionCobranza.API
```
Aplicar migración:
```bash
dotnet ef database update --project GestionCobranza.Infrastructure --startup-project GestionCobranza.API
```

### Ejecutar el backend
```bash
cd GestionCobranza.API
dotnet run
```

### Flujo de ejecución
1.  Ejecutar backend:
```bash
dotnet run
```
2.  Ejecutar frontend:
```bash
npm run dev
```
3.  Verificar conexión API ↔ Frontend


### Comandos principales
```bash
dotnet restore
dotnet build
dotnet run
dotnet ef migrations add InitialCreate
dotnet ef database update
```
### Recomendaciones
- No subir credenciales reales a Git
- Usar appsettings.Development.json para entorno local
- Mantener separación de responsabilidades
- No mezclar lógica en controllers