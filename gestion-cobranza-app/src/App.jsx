import './App.css'

function App() {
  return (
    <div className="dashboard-app">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-icon">A</div>
          <div>
            <p className="brand-label">Sistema de Gestión de Cobranza</p>
            <span>Audicob</span>
          </div>
        </div>

        <nav className="sidebar-nav">
          <button className="nav-item active">
            <span className="nav-icon">⌂</span>
            Dashboard
          </button>
          <button className="nav-item">
            <span className="nav-icon">👥</span>
            Asesores
          </button>
          <button className="nav-item">
            <span className="nav-icon">📋</span>
            Clientes
          </button>
          <button className="nav-item">
            <span className="nav-icon">⚠️</span>
            Morosidad
          </button>
          <button className="nav-item">
            <span className="nav-icon">🔔</span>
            Alertas
          </button>
          <button className="nav-item">
            <span className="nav-icon">📈</span>
            Reportes
          </button>
          <button className="nav-item">
            <span className="nav-icon">⬆️</span>
            Importar
          </button>
        </nav>

        <button className="logout">Cerrar Sesión</button>
      </aside>

      <main className="content">
        <header className="topbar">
          <div className="search-bar">
            <span className="search-icon">🔍</span>
            <input placeholder="Buscar clientes..." />
          </div>
          <div className="user-actions">
            <button className="icon-button" aria-label="Notificaciones">🔔</button>
            <div className="profile-card">
              <div className="avatar">JR</div>
              <div>
                <p>Jimena Rodríguez</p>
                <span>Administrador</span>
              </div>
            </div>
          </div>
        </header>

        <section className="panel-header">
          <div>
            <p className="eyebrow">Panel de Administración</p>
            <h1>Gestión y supervisión del sistema de cobranza</h1>
          </div>
        </section>

        <section className="metrics-grid">
          <article className="metric-card">
            <span className="metric-label">Total de Asesores</span>
            <strong>12</strong>
            <small>+2 este mes</small>
          </article>
          <article className="metric-card">
            <span className="metric-label">Total de Clientes</span>
            <strong>1,247</strong>
            <small>+156 este mes</small>
          </article>
          <article className="metric-card">
            <span className="metric-label">Cobranza Total</span>
            <strong>S/. 854,320</strong>
            <small>+18.2%</small>
          </article>
          <article className="metric-card">
            <span className="metric-label">Eficiencia Global</span>
            <strong>87.5%</strong>
            <small>+3.2%</small>
          </article>
        </section>

        <section className="dashboard-grid">
          <article className="panel-card chart-card">
            <div className="panel-title">Cobranza semanal</div>
            <div className="line-legend">
              <span className="legend-item">
                <span className="dot blue"></span>
                Cobranzas
              </span>
            </div>
            <div className="line-chart">
              <div className="line"></div>
              <div className="point p1"></div>
              <div className="point p2"></div>
              <div className="point p3"></div>
              <div className="point p4"></div>
              <div className="point p5"></div>
            </div>
            <div className="chart-axis">
              <span>Enero</span>
              <span>Febrero</span>
              <span>Marzo</span>
            </div>
          </article>

          <article className="panel-card donut-card">
            <div className="panel-title">Distribución de Clientes</div>
            <div className="donut-chart">
              <div className="donut-ring"></div>
            </div>
            <div className="donut-legend">
              <div>
                <span className="legend-swatch green"></span>
                Al Día
              </div>
              <div>
                <span className="legend-swatch yellow"></span>
                Alerta
              </div>
              <div>
                <span className="legend-swatch red"></span>
                Morosos
              </div>
            </div>
          </article>
        </section>

        <section className="panel-card table-card">
          <div className="panel-title">Rendimiento de Asesores</div>
          <table>
            <thead>
              <tr>
                <th>Asesor</th>
                <th>Deuda Gestionada</th>
                <th>Clientes</th>
                <th>Eficiencia</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>María López</td>
                <td>S/. 125,000</td>
                <td>145</td>
                <td>
                  <div className="progress-cell">
                    <div className="progress-bar" style={{ width: '92%' }} />
                    <span>92%</span>
                  </div>
                </td>
                <td><span className="status-pill success">Excelente</span></td>
              </tr>
              <tr>
                <td>Juan Pérez</td>
                <td>S/. 98,500</td>
                <td>112</td>
                <td>
                  <div className="progress-cell">
                    <div className="progress-bar blue" style={{ width: '82%' }} />
                    <span>82%</span>
                  </div>
                </td>
                <td><span className="status-pill success">Excelente</span></td>
              </tr>
              <tr>
                <td>Sofía Torres</td>
                <td>S/. 71,200</td>
                <td>78</td>
                <td>
                  <div className="progress-cell">
                    <div className="progress-bar orange" style={{ width: '79%' }} />
                    <span>79%</span>
                  </div>
                </td>
                <td><span className="status-pill warning">Excelente</span></td>
              </tr>
            </tbody>
          </table>
        </section>
      </main>
    </div>
  )
}

export default App
