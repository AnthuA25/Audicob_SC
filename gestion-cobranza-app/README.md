# Frontend - Gestión de Cobranza

Este proyecto corresponde al **frontend** del sistema de **Gestión de Cobranza**, desarrollado con **React + Vite**.  
Su función es consumir la API del backend en .NET y mostrar las vistas del sistema, como login, dashboard, clientes, asesores, reportes, morosidad y alertas.

---

## Tecnologías usadas

- React
- Vite
- JavaScript
- React Router DOM
- Axios
- Recharts
- CSS / estilos del proyecto

---

## Estructura general del proyecto

```bash
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
    ├── .gitignore
    ├── package.json
    ├── vite.config.js
    └── README.md


```


## Estructura de `src`

``` bash
src/
├── api/
│   ├── axiosClient.js
│   ├── authApi.js
│   ├── dashboardApi.js
│   ├── clienteApi.js
│   ├── asesorApi.js
│   ├── morosidadApi.js
│   ├── alertaApi.js
│   └── reporteApi.js
│
├── assets/
│   ├── images/
│   ├── icons/
│   └── logos/
│
├── components/
│   ├── common/
│   │   ├── Sidebar.jsx
│   │   ├── Navbar.jsx
│   │   ├── Loader.jsx
│   │   ├── ErrorMessage.jsx
│   │   └── ProtectedRoute.jsx
│   │
│   ├── dashboard/
│   │   ├── MetricCard.jsx
│   │   ├── CobranzaLineChart.jsx
│   │   ├── DistribucionClientesChart.jsx
│   │   └── RendimientoAsesoresTable.jsx
│   │
│   ├── clientes/
│   │   ├── ClienteTable.jsx
│   │   ├── ClienteCard.jsx
│   │   ├── ClienteFiltro.jsx
│   │   └── ClienteEstadoBadge.jsx
│   │
│   ├── asesores/
│   │   ├── AsesorTable.jsx
│   │   ├── AsesorCard.jsx
│   │   └── AsesorRendimientoCard.jsx
│   │
│   ├── morosidad/
│   │   ├── MorosidadTable.jsx
│   │   ├── MorosidadFiltro.jsx
│   │   └── MorosidadResumenCard.jsx
│   │
│   ├── alertas/
│   │   ├── AlertaCard.jsx
│   │   ├── AlertaTable.jsx
│   │   └── AlertaFiltro.jsx
│   │
│   ├── reportes/
│   │   ├── ReporteFiltroForm.jsx
│   │   ├── ReporteTable.jsx
│   │   └── ReporteExportButtons.jsx
│   │
│   ├── forms/
│   │   ├── LoginForm.jsx
│   │   ├── ClienteForm.jsx
│   │   ├── AsesorForm.jsx
│   │   └── ReporteForm.jsx
│   │
│   └── ui/
│       ├── Button.jsx
│       ├── Input.jsx
│       ├── Select.jsx
│       ├── Modal.jsx
│       ├── Table.jsx
│       ├── Card.jsx
│       └── Badge.jsx
│
├── context/
│   └── AuthContext.jsx
│
├── hooks/
│   ├── useAuth.js
│   ├── useDashboard.js
│   ├── useClientes.js
│   ├── useAsesores.js
│   ├── useMorosidad.js
│   └── useAlertas.js
│
├── layouts/
│   ├── AuthLayout.jsx
│   └── DashboardLayout.jsx
│
├── pages/
│   ├── auth/
│   │   └── LoginPage.jsx
│   │
│   ├── dashboard/
│   │   ├── DashboardAdminPage.jsx
│   │   └── DashboardAsesorPage.jsx
│   │
│   ├── clientes/
│   │   ├── ClientesPage.jsx
│   │   ├── ClienteDetallePage.jsx
│   │   └── RegistrarClientePage.jsx
│   │
│   ├── asesores/
│   │   ├── AsesoresPage.jsx
│   │   ├── AsesorDetallePage.jsx
│   │   └── RegistrarAsesorPage.jsx
│   │
│   ├── morosidad/
│   │   └── MorosidadPage.jsx
│   │
│   ├── alertas/
│   │   └── AlertasPage.jsx
│   │
│   └── reportes/
│       └── ReportesPage.jsx
│
├── routes/
│   └── AppRouter.jsx
│
├── services/
│   ├── authService.js
│   ├── dashboardService.js
│   ├── clienteService.js
│   ├── asesorService.js
│   ├── morosidadService.js
│   ├── alertaService.js
│   └── reporteService.js
│
├── utils/
│   ├── formatCurrency.js
│   ├── formatDate.js
│   ├── storage.js
│   └── permissions.js
│
├── constants/
│   ├── roles.js
│   ├── routes.js
│   └── messages.js
│
├── styles/
│   ├── globals.css
│   ├── dashboard.css
│   ├── login.css
│   ├── clientes.css
│   └── reportes.css
│
├── App.jsx
└── main.jsx

```

### Qué va en cada carpeta
-  api/

Aquí van las llamadas directas al backend.

Ejemplos:

login
dashboard
clientes
asesores
morosidad
alertas
reportes

Regla:
Aquí solo va la comunicación HTTP con la API.
No debe haber lógica visual ni componentes.

-  assets/
Aquí van los recursos estáticos del proyecto.

Ejemplos:

imágenes
logos
íconos
ilustraciones

-  components/
Aquí van los componentes reutilizables de la interfaz.

-  components/common/
Componentes generales reutilizables en varias vistas.

Ejemplos:

Sidebar
Navbar
Loader
ErrorMessage
ProtectedRoute

-  components/dashboard/
Componentes exclusivos del dashboard.

Ejemplos:

tarjetas de métricas
gráfico de línea
gráfico de distribución
tabla de rendimiento

-  components/clientes/
Componentes relacionados a clientes.

Ejemplos:

tabla de clientes
filtros
badges de estado
cards de resumen

-  components/asesores/
Componentes relacionados a asesores.

Ejemplos:

tabla de asesores
cards de asesores
indicadores de rendimiento

-  components/morosidad/
Componentes para visualizar y filtrar la morosidad.

-  components/alertas/
Componentes para mostrar alertas y notificaciones de cobranza.

-  components/reportes/
Componentes para filtros, tablas y exportación de reportes.

-  components/forms/
Formularios del sistema.

Ejemplos:

LoginForm
ClienteForm
AsesorForm
ReporteForm

-  components/ui/
Componentes visuales genéricos.

Ejemplos:

Button
Input
Select
Modal
Table
Card
Badge

Regla:
Los componentes deben ser reutilizables y tener una sola responsabilidad.

-  context/
Aquí va el estado global de la aplicación.

Ejemplo:

sesión del usuario
token
datos básicos del usuario autenticado

-  hooks/

Aquí van hooks personalizados para reutilizar lógica.

Ejemplos:

autenticación
dashboard
clientes
morosidad
alertas

Regla:
Aquí va lógica reutilizable, no UI.

-  layouts/

Aquí van las estructuras base de las páginas.

Ejemplos:

AuthLayout para login
DashboardLayout para páginas internas con sidebar y navbar

-  pages/
Aquí van las vistas completas del sistema.

Regla:
Cada archivo en pages representa una pantalla completa.

Ejemplos:

LoginPage
DashboardAdminPage
DashboardAsesorPage
ClientesPage
AsesoresPage
MorosidadPage
AlertasPage
ReportesPage

-  routes/
Aquí se centralizan las rutas del sistema.

Uso:

definir rutas públicas
definir rutas protegidas
redirigir según rol

-  services/
Aquí va lógica de negocio del frontend.

Uso:

transformar respuestas del backend
formatear datos antes de enviarlos a pantalla
encapsular lógica reutilizable

Regla:
Si una lógica no pertenece al componente ni a la llamada HTTP directa, puede ir aquí.

-  utils/
Aquí van funciones auxiliares reutilizables.

Ejemplos:

formatear moneda
formatear fecha
manejo de localStorage
validaciones auxiliares
permisos

-  constants/
Aquí van constantes globales del sistema.

Ejemplos:

roles
rutas
mensajes del sistema

-  styles/
Aquí van los estilos globales o por módulo.

Ejemplos:

estilos generales
estilos del dashboard
estilos del login
estilos de clientes
estilos de reportes

-  App.jsx
Es el componente principal de la aplicación.
Normalmente solo carga el router principal.

-  main.jsx
Es el punto de entrada del frontend.
Aquí se renderiza la aplicación React.

---

## Reglas de organización del equipo

1. No mezclar responsabilidades
- `api/` → llamadas HTTP al backend  
- `services/` → lógica de negocio del frontend  
- `components/` → UI reutilizable  
- `pages/` → vistas completas  
- `hooks/` → lógica reutilizable  
- `layouts/` → estructura visual base

2. No llamar a la API desde cualquier archivo

- Las llamadas al backend deben centralizarse en api/.

3. No poner demasiada lógica dentro de los componentes

- Si la lógica crece, moverla a hooks/ o services/.

4. Nombrar archivos claramente

Ejemplos:

- `LoginPage.jsx`
- `LoginForm.jsx`
- `authApi.js`
- `dashboardService.js`

5. Un archivo = una responsabilidad

Cada archivo debe encargarse de una sola cosa.

6. Mantener nombres consistentes
Componentes: PascalCase (UpperCamelCase)
Hooks: camelCase comenzando con `use` (lowerCamelCase)
Servicios/api/utils/constants: camelCase(lowerCamelCase)

## Cómo clonar el proyecto

```bash
git clone <URL_DEL_REPOSITORIO>
```

Luego ingresar a la carpeta del frontend:

```bash
cd frontend/gestion-cobranza-app
```

Si el frontend no está dentro de `frontend/`, ingresar directamente a la carpeta donde esté el `package.json`.

## Qué instalar después de clonar

Instalar dependencias del proyecto:

```bash
npm install
```

Si el proyecto aún no tiene algunas librerías base, instalar:

```bash
npm install react-router-dom axios recharts
```
Si también van a usar formularios y validaciones:
```bash
npm install react-hook-form zod @hookform/resolvers
```

## Cómo correr el proyecto
```bash
npm run dev
```
Eso levantará el frontend en desarrollo, normalmente en:

```bash
http://localhost:5173
```
## Cómo conectarlo con el backend

El frontend consume la API del backend .NET, por lo tanto el backend también debe estar corriendo.

Ejemplo de base URL en axiosClient.js:

```bash
import axios from "axios";

const axiosClient = axios.create({
  baseURL: "https://localhost:5001/api",
  headers: {
    "Content-Type": "application/json",
  },
});

export default axiosClient;
```

La URL puede cambiar según el puerto en que esté corriendo tu backend.

##Flujo de ejecución recomendado

1. Levantar el backend

Desde el proyecto API de .NET:
```bash
dotnet run
```
2. Levantar el frontend

Desde la carpeta del frontend:
```bash
npm run dev
```
3. Verificar conexión
- backend corriendo
- frontend corriendo
- rutas configuradas correctamente
- CORS habilitado en el backend


## Librerías recomendadas para este proyecto
- `react-router-dom` → navegación
- `axios` → consumo de la API
- `recharts` → gráficos
- `react-hook-form` → formularios
- `zod` → validaciones

## Comandos principales

### Instalar dependencias
```bash
npm install
```
### Ejecutar en desarrollo
```bash
npm run dev
```
### Generar build
```bash
npm run build
```
### Previsualizar build
```bash
npm run preview
```