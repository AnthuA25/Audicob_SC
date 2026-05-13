
# Sistema de Gestión de Cobranza - Backend

Backend desarrollado en **ASP.NET Core Web API** para el sistema de gestión de cobranza **Audicob**, orientado a la administración de clientes, seguimiento de cobranzas y visualización de dashboards según roles (Administrador y Asesor).

---

# Arquitectura del Proyecto

El sistema sigue una **arquitectura en capas (Layered Architecture)**, permitiendo separación de responsabilidades, mantenibilidad y escalabilidad.

## 🔹 Estructura actual

```text
Controllers/
Dtos/
Models/
Program.cs
appsettings.json
```

## 🔹 Arquitectura propuesta (estándar profesional)

```text
Controllers → Services → Repositories → DbContext → Base de Datos
```

---

# Estructura del Proyecto

```text
GestionCobranza-backend/
│
├── Controllers/        # Manejo de endpoints HTTP
├── Dtos/               # Objetos de transferencia de datos
├── Models/             # Entidades del sistema (BD)
├── Properties/         # Configuración del proyecto
│
├── Program.cs          # Configuración principal
├── appsettings.json    # Configuración (BD, CORS, etc)
├── README.md           # Documentación del proyecto
└── .gitignore
```

---

# Tecnologías utilizadas

- ASP.NET Core Web API
- C#
- Entity Framework Core (ORM)
- PostgreSQL
- Autenticación (JWT o Cookies - según implementación)
- CORS habilitado para integración con frontend

---

# Flujo de una petición

1. El cliente (frontend) realiza una solicitud HTTP
2. El **Controller** recibe la petición
3. Se procesa la lógica (actualmente en controller, futuro en service)
4. Se accede a los datos (Models / futura capa Repository)
5. Se devuelve una respuesta en formato JSON

---

# Roles del sistema

## Administrador

- Visualiza todos los clientes
- Accede a dashboards globales
- Gestiona cobranzas

## Asesor

- Visualiza solo sus clientes asignados
- Accede a su propio dashboard
- Consulta detalle de clientes

---

# Endpoints (ejemplo)

```http
GET    /api/clientes
GET    /api/clientes/{id}
GET    /api/clientes/mis-clientes

POST   /api/cobranzas
PUT    /api/clientes/{id}
DELETE /api/clientes/{id}
```

---

# Buenas prácticas aplicadas

- Separación de responsabilidades (Controllers, DTOs, Models)
- Uso de DTOs para evitar exponer entidades directamente
- Convenciones REST en endpoints
- Uso de async/await para operaciones
- Configuración centralizada en `Program.cs`
- Manejo de CORS para frontend

---

# Estándares de desarrollo

## Convenciones de nombres

- Clases: `PascalCase`
- Métodos: `PascalCase`
- Variables: `camelCase`
- Interfaces: `I + Nombre` (ej: `IClienteService`)

---

## Métodos asíncronos

Todos los métodos que acceden a datos deben ser async:

```csharp
Task<List<ClienteDto>> ObtenerClientesAsync()
```

---

## Uso de DTOs

Se utilizan DTOs para:

- Controlar la información enviada
- Mejorar seguridad
- Optimizar respuestas

---

## Respuestas HTTP

| Código | Uso |
|------|-----|
| 200 | OK |
| 201 | Creado |
| 400 | Error de validación |
| 401 | No autorizado |
| 403 | Prohibido |
| 404 | No encontrado |
| 500 | Error interno |

---

# Seguridad

- Autenticación basada en token o sesión
- Validación de roles (Administrador / Asesor)
- Protección de endpoints sensibles
- Configuración de CORS

---

# Mejoras futuras (Arquitectura Pro)

Para escalar el sistema, se recomienda implementar:

- 📁 `Services/` → lógica de negocio
- 📁 `Repositories/` → acceso a datos
- 📁 `Data/` → DbContext separado
- 📁 `Middleware/` → manejo global de errores
- 📁 `Mappings/` → AutoMapper

---

# Principios aplicados

- ✅ SOLID
- ✅ Separación de capas
- ✅ Bajo acoplamiento
- ✅ Alta cohesión

---

# Configuración del entorno

## 1. Clonar repositorio

```bash
git clone <URL_DEL_REPO>
cd GestionCobranza-backend
```

---

## 2. Configurar base de datos

Editar `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=DB;Username=usuario;Password=clave"
}
```

---

## 3. Ejecutar proyecto

```bash
dotnet restore
dotnet run
```


# Convención de commits

| Tipo | Uso |
|------|-----|
| feat | Nueva funcionalidad |
| fix | Corrección de errores |
| refactor | Mejora interna |
| docs | Documentación |
| style | Formato |
| test | Pruebas |

Ejemplo:

```bash
feat: implementar listado de clientes por asesor
fix: corregir filtro por usuario logueado
```

---

# Autores

Proyecto desarrollado como parte del sistema **Audicob - Gestión de Cobranza**.

---

# Conclusión

El backend está construido bajo una arquitectura escalable basada en capas, preparada para evolucionar hacia un modelo más robusto como **Clean Architecture**, permitiendo una mejor organización del código, facilidad de mantenimiento y crecimiento del sistema.