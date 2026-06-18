# Sistema de Gestión de Cobranza - AUDICOB

## Descripción

**AUDICOB** es un sistema web para la gestión de cobranza de productos de belleza, desarrollado bajo una arquitectura cliente-servidor. El sistema permite administrar clientes, asesores de cobranza, seguimiento de pagos, indicadores de morosidad, reportes y dashboards personalizados según el rol del usuario.

El proyecto está conformado por:

- **Frontend:** React + Vite
- **Backend:** ASP.NET Core Web API
- **Base de Datos:** PostgreSQL

---

# Arquitectura General

```
                ┌───────────────────────┐
                │       Frontend        │
                │     React + Vite      │
                └──────────┬────────────┘
                           │
                     HTTP (Axios)
                           │
                           ▼
                ┌───────────────────────┐
                │ ASP.NET Core Web API  │
                │      Controllers      │
                │         DTOs          │
                │        Models         │
                └──────────┬────────────┘
                           │
                  Entity Framework Core
                           │
                           ▼
                ┌───────────────────────┐
                │      PostgreSQL       │
                └───────────────────────┘
```

---

# Tecnologías Utilizadas

## Frontend

- React
- Vite
- JavaScript
- React Router DOM
- Axios
- Recharts
- React Hook Form
- Zod
- CSS

## Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL

---

# Estructura General del Proyecto

```
GestionCobranza/

├── frontend/
│   └── gestion-cobranza-app/
│
├── backend/
│   └── GestionCobranza-backend/
│
└── README.md
```

---

# Frontend

El frontend consume la API desarrollada en ASP.NET Core y presenta la interfaz del sistema para Administradores y Asesores.

## Estructura

```
frontend/
└── gestion-cobranza-app/
    ├── public/
    ├── src/
    │   ├── api/
    │   ├── assets/
    │   ├── components/
    │   ├── context/
    │   ├── hooks/
    │   ├── layouts/
    │   ├── pages/
    │   ├── routes/
    │   ├── services/
    │   ├── utils/
    │   ├── constants/
    │   ├── styles/
    │   ├── App.jsx
    │   └── main.jsx
    ├── package.json
    ├── vite.config.js
    └── README.md
```

## Organización de Carpetas

### api/

Contiene todas las llamadas HTTP hacia la API del backend.

Ejemplos:

- authApi.js
- dashboardApi.js
- clienteApi.js
- asesorApi.js
- morosidadApi.js
- alertaApi.js
- reporteApi.js

---

### assets/

Recursos estáticos del proyecto.

- Imágenes
- Logos
- Iconos

---

### components/

Componentes reutilizables de la interfaz.

```
components/
│
├── common/
├── dashboard/
├── clientes/
├── asesores/
├── morosidad/
├── alertas/
├── reportes/
├── forms/
└── ui/
```

---

### context/

Estado global de la aplicación.

Ejemplo:

- Usuario autenticado
- Token
- Información de sesión

---

### hooks/

Hooks personalizados para reutilizar lógica.

Ejemplos:

- useAuth
- useClientes
- useDashboard
- useMorosidad

---

### layouts/

Plantillas base del sistema.

- AuthLayout
- DashboardLayout

---

### pages/

Representan las vistas completas del sistema.

Ejemplos:

- LoginPage
- DashboardAdminPage
- DashboardAsesorPage
- ClientesPage
- AsesoresPage
- MorosidadPage
- AlertasPage
- ReportesPage

---

### routes/

Configuración centralizada de rutas.

- Rutas públicas
- Rutas protegidas
- Redirección según rol

---

### services/

Contiene la lógica de negocio del frontend.

Ejemplos:

- Transformación de datos
- Reglas antes de enviar información
- Procesamiento de respuestas

---

### utils/

Funciones auxiliares reutilizables.

Ejemplos:

- Formato de moneda
- Formato de fechas
- LocalStorage
- Validaciones

---

### constants/

Constantes globales.

- Roles
- Rutas
- Mensajes

---

### styles/

Archivos CSS globales y específicos por módulo.

---

# Backend

El backend fue desarrollado con **ASP.NET Core Web API**, siguiendo una **Arquitectura en Capas**, permitiendo una mejor organización del código y facilitando su mantenimiento y escalabilidad.

## Arquitectura

```
Controllers
      │
      ▼
Services (propuesto)
      │
      ▼
Repositories (propuesto)
      │
      ▼
DbContext
      │
      ▼
PostgreSQL
```

Actualmente la lógica del proyecto se encuentra principalmente en los Controllers, con una arquitectura preparada para evolucionar hacia una estructura más robusta basada en Services y Repositories.

## Estructura

```
GestionCobranza-backend/
│
├── Controllers/
├── Dtos/
├── Models/
├── Properties/
├── Program.cs
├── appsettings.json
├── README.md
└── .gitignore
```

---

## Organización

### Controllers/

Gestionan las peticiones HTTP y exponen los endpoints de la API.

### Dtos/

Objetos de transferencia de datos utilizados para controlar la información enviada y recibida por la API.

### Models/

Entidades que representan las tablas de la base de datos.

### Program.cs

Configuración principal del proyecto.

### appsettings.json

Configuración de:

- Cadena de conexión
- CORS
- Parámetros generales

---

# Flujo de una Petición

```
Usuario

    │

Frontend (React)

    │ Axios

    ▼

ASP.NET Core Web API

    │

Controllers

    │

Entity Framework Core

    │

PostgreSQL

    │

Respuesta JSON

    │

Frontend

    │

Interfaz de Usuario
```

---

# Roles del Sistema

## Administrador

- Dashboard general
- Gestión de clientes
- Gestión de asesores
- Reportes
- Morosidad
- Alertas

## Asesor

- Dashboard personal
- Clientes asignados
- Consulta de información de cobranza

---

# Endpoints Principales

```
GET     /api/clientes

GET     /api/clientes/{id}

GET     /api/clientes/mis-clientes

POST    /api/cobranzas

PUT     /api/clientes/{id}

DELETE  /api/clientes/{id}
```

---

# Instalación del Proyecto

## 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
```

---

# Configuración del Backend

Ingresar al proyecto:

```bash
cd backend/GestionCobranza-backend
```

Restaurar dependencias:

```bash
dotnet restore
```

Editar el archivo:

```
appsettings.json
```

Cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=GestionCobranza;Username=usuario;Password=clave"
}
```

Ejecutar:

```bash
dotnet run
```

---

# Configuración del Frontend

Ingresar al proyecto:

```bash
cd frontend/gestion-cobranza-app
```

Instalar dependencias:

```bash
npm install
```

Si fuera necesario instalar dependencias adicionales:

```bash
npm install react-router-dom axios recharts react-hook-form zod @hookform/resolvers
```

Ejecutar:

```bash
npm run dev
```

El proyecto estará disponible en:

```
http://localhost:5173
```

---

# Configuración de Axios

```javascript
import axios from "axios";

const axiosClient = axios.create({
  baseURL: "https://localhost:5001/api",
  headers: {
    "Content-Type": "application/json",
  },
});

export default axiosClient;
```

---

# Flujo de Ejecución

1. Iniciar PostgreSQL.
2. Configurar la cadena de conexión en `appsettings.json`.
3. Ejecutar el backend.

```bash
dotnet run
```

4. Ejecutar el frontend.

```bash
npm run dev
```

5. Verificar:

- Backend en ejecución.
- Frontend en ejecución.
- Comunicación entre ambos.
- CORS habilitado.

---

# Buenas Prácticas

- Separación de responsabilidades.
- Arquitectura en capas.
- Componentes reutilizables.
- Hooks personalizados.
- Uso de DTOs.
- Convenciones REST.
- Métodos asíncronos.
- Configuración centralizada.
- Organización modular.

---

# Convenciones de Nombres

## Backend

| Elemento | Convención |
|----------|------------|
| Clases | PascalCase |
| Métodos | PascalCase |
| Variables | camelCase |
| Interfaces | I + Nombre |

## Frontend

| Elemento | Convención |
|----------|------------|
| Componentes | PascalCase |
| Hooks | use + camelCase |
| Servicios | camelCase |
| Utilidades | camelCase |

---

# Comandos Principales

## Backend

```bash
dotnet restore

dotnet run
```

## Frontend

```bash
npm install

npm run dev

npm run build

npm run preview
```

---

# Mejoras Futuras

- Implementar Services.
- Implementar Repositories.
- Separar DbContext.
- Middleware para manejo global de errores.
- AutoMapper.
- Validaciones centralizadas.
- Clean Architecture.
- Pruebas automatizadas.
- Integración continua (CI/CD).

---

# Principios Aplicados

- Arquitectura Cliente-Servidor.
- Arquitectura en Capas (Layered Architecture).
- Principios SOLID.
- Separación de responsabilidades.
- Alta cohesión.
- Bajo acoplamiento.
- Convenciones REST.

---

# Autores

Proyecto desarrollado como parte del **Sistema de Gestión de Cobranza AUDICOB**, orientado a optimizar el proceso de cobranza mediante una aplicación web desarrollada con React, ASP.NET Core Web API y PostgreSQL.

---

# Conclusión

El Sistema de Gestión de Cobranza AUDICOB integra un frontend desarrollado con React y un backend construido con ASP.NET Core Web API bajo una arquitectura cliente-servidor y una organización en capas. Esta estructura permite una adecuada separación de responsabilidades, facilitando el mantenimiento, la escalabilidad y la evolución del sistema hacia arquitecturas más robustas como Clean Architecture, además de favorecer la incorporación de nuevas funcionalidades y buenas prácticas de desarrollo.
