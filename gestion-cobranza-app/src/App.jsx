import './App.css'

function App() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-panel">
          <div className="brand-icon">A</div>
          <div>
            <p className="brand-title">Sistema de Gestión de Cobranza</p>
            <span className="brand-subtitle">Audicob</span>
          </div>
        </div>

        <nav className="menu">
          <button className="menu-item active">Dashboard</button>
          <button className="menu-item">Mis clientes</button>
          <button className="menu-item">Pagos</button>
          <button className="menu-item">Alertas</button>
          <button className="menu-item">Reportes</button>
        </nav>

        <button className="sign-out">Cerrar Sesión</button>
      </aside>

      <main className="dashboard">
        <header className="dashboard-header">
          <div>
            <h1>Panel de Asesor</h1>
            <p>Tu rendimiento y clientes asignados</p>
          </div>

          <div className="header-actions">
            <label className="search-box">
              <span className="search-icon">🔍</span>
              <input type="search" placeholder="Buscar clientes..." />
            </label>
            <button className="icon-button" aria-label="Notificaciones">
              <span className="bell">🔔</span>
              <span className="notification-badge" />
            </button>
            <div className="profile-card">
              <div className="avatar">R</div>
              <div>
                <span className="profile-name">Ronny Sanchez</span>
                <small>Asesor de cobranza</small>
              </div>
            </div>
          </div>
        </header>

        <section className="stats-grid">
          <article className="stat-card">
            <div className="stat-title">Total de clientes</div>
            <div className="stat-value">238</div>
            <div className="stat-caption positive">+12 este mes</div>
          </article>
          <article className="stat-card">
            <div className="stat-title">Deudas pendientes</div>
            <div className="stat-value">S/. 45,280</div>
            <div className="stat-caption negative">-8% vs mes anterior</div>
          </article>
          <article className="stat-card">
            <div className="stat-title">Pagos realizados</div>
            <div className="stat-value">S/. 73,000</div>
            <div className="stat-caption positive">+15% este mes</div>
          </article>
          <article className="stat-card">
            <div className="stat-title">Clientes en morosidad</div>
            <div className="stat-value">40</div>
            <div className="stat-caption positive">-5 vs semana pasada</div>
          </article>
        </section>

        <section className="charts-grid">
          <article className="panel card-chart">
            <div className="panel-header">
              <h2>Distribución de Clientes</h2>
            </div>
            <div className="chart-content">
              <div className="chart-legend">
                <span className="legend-dot" /> Clientes
              </div>
              <div className="chart-legend secondary">
                <span className="legend-dot" /> Pagos
              </div>
              <div className="line-chart">
                <svg viewBox="0 0 360 180" aria-hidden="true">
                  <defs>
                    <linearGradient id="lineGradient1" x1="0%" y1="0%" x2="100%" y2="0%">
                      <stop offset="0%" stopColor="#3b82f6" />
                      <stop offset="100%" stopColor="#8b5cf6" />
                    </linearGradient>
                    <linearGradient id="lineGradient2" x1="0%" y1="0%" x2="100%" y2="0%">
                      <stop offset="0%" stopColor="#f97316" />
                      <stop offset="100%" stopColor="#ef4444" />
                    </linearGradient>
                  </defs>
                  <polyline points="20,120 70,90 120,100 170,110 220,60 310,50" fill="none" stroke="url(#lineGradient1)" strokeWidth="4" strokeLinejoin="round" strokeLinecap="round" />
                  <polyline points="20,135 70,115 120,140 170,130 220,125 310,140" fill="none" stroke="url(#lineGradient2)" strokeWidth="4" strokeLinejoin="round" strokeLinecap="round" />
                  {[20,70,120,170,220,310].map((x, index) => (
                    <circle key={index} cx={x} cy={[120,90,100,110,60,50][index]} r="5" fill="#3b82f6" />
                  ))}
                  {[20,70,120,170,220,310].map((x, index) => (
                    <circle key={`p-${index}`} cx={x} cy={[135,115,140,130,125,140][index]} r="5" fill="#f97316" />
                  ))}
                  <g stroke="#d1d5db" strokeWidth="1">
                    <line x1="20" y1="30" x2="320" y2="30" />
                    <line x1="20" y1="70" x2="320" y2="70" />
                    <line x1="20" y1="110" x2="320" y2="110" />
                    <line x1="20" y1="150" x2="320" y2="150" />
                  </g>
                  <g fill="#6b7280" fontSize="11" textAnchor="middle">
                    {['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun'].map((month, index) => (
                      <text key={month} x={[20,70,120,170,220,310][index]} y="175">{month}</text>
                    ))}
                  </g>
                </svg>
              </div>
            </div>
          </article>

          <article className="panel card-chart pie-panel">
            <div className="panel-header">
              <h2>Clasificación de deudores</h2>
            </div>
            <div className="pie-chart">
              <svg viewBox="0 0 240 240" aria-hidden="true">
                <circle cx="120" cy="120" r="100" fill="none" stroke="#10b981" strokeWidth="80" strokeDasharray="65 35" strokeLinecap="butt" transform="rotate(-90 120 120)" />
                <circle cx="120" cy="120" r="100" fill="none" stroke="#f59e0b" strokeWidth="80" strokeDasharray="18 82" strokeLinecap="butt" transform="rotate(-24 120 120)" />
                <circle cx="120" cy="120" r="100" fill="none" stroke="#ef4444" strokeWidth="80" strokeDasharray="12 88" strokeLinecap="butt" transform="rotate(42 120 120)" />
                <circle cx="120" cy="120" r="100" fill="none" stroke="#c2410c" strokeWidth="80" strokeDasharray="5 95" strokeLinecap="butt" transform="rotate(86 120 120)" />
              </svg>
              <div className="pie-labels">
                <div><span className="pie-key green" /> Al día 65%</div>
                <div><span className="pie-key amber" /> Atraso leve 18%</div>
                <div><span className="pie-key red" /> Morosidad 12%</div>
                <div><span className="pie-key dark-red" /> Crítico 5%</div>
              </div>
            </div>
          </article>
        </section>

        <section className="panel wide-panel">
          <div className="panel-header">
            <h2>Tendencia de morosidad</h2>
          </div>
          <div className="bar-chart">
            <svg viewBox="0 0 740 260" aria-hidden="true">
              {[0, 1, 2, 3, 4, 5].map((index) => {
                const heights = [180, 170, 90, 70, 110, 150]
                const x = 60 + index * 110
                return (
                  <g key={index}>
                    <rect x={x} y={220 - heights[index]} width="36" height={heights[index]} rx="8" fill="#b91c1c" />
                    <text x={x + 18} y="245" textAnchor="middle" fontSize="14" fill="#6b7280">
                      {['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun'][index]}
                    </text>
                  </g>
                )
              })}
              <g stroke="#e5e7eb" strokeWidth="1">
                <line x1="40" y1="40" x2="700" y2="40" />
                <line x1="40" y1="100" x2="700" y2="100" />
                <line x1="40" y1="160" x2="700" y2="160" />
                <line x1="40" y1="220" x2="700" y2="220" />
              </g>
              <g fill="#6b7280" fontSize="12" textAnchor="end">
                {['1000', '750', '500', '250'].map((value, index) => (
                  <text key={value} x="32" y={50 + index * 60}>{value}</text>
                ))}
              </g>
            </svg>
          </div>
        </section>
      </main>
    </div>
  )
}

export default App
