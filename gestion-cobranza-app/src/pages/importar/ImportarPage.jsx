import { useState, useRef } from "react";
import { Upload, FileSpreadsheet, Download } from "lucide-react";
import "../../../src/styles/importar.css";

const importacionesRecientes = [
  {
    id: 1,
    nombre: "clientes_marzo_2026.xlsx",
    fecha: "2026-03-18",
    registros: 25,
  },
  { id: 2, nombre: "nuevos_clientes.xlsx", fecha: "2026-03-10", registros: 12 },
];

const ImportarPage = () => {
  const [archivoSeleccionado, setArchivoSeleccionado] = useState(null);
  const [dragActivo, setDragActivo] = useState(false);
  const inputRef = useRef();

  const handleArchivo = (file) => {
    if (file) setArchivoSeleccionado(file);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    setDragActivo(false);
    const file = e.dataTransfer.files[0];
    handleArchivo(file);
  };

  return (
    <div>
      <div className="importar-header">
        <h1>Importación de Datos</h1>
        <p>Importa clientes masivamente desde archivos Excel</p>
      </div>

      <div className="importar-card">
        <h2>Instrucciones</h2>
        <div className="instrucciones-list">
          <div className="instruccion-item">
            <div className="instruccion-numero">1</div>
            <div className="instruccion-texto">
              <strong>Descarga la plantilla Excel</strong>
              <span>
                Usa nuestra plantilla para asegurar el formato correcto
              </span>
            </div>
          </div>
          <div className="instruccion-item">
            <div className="instruccion-numero">2</div>
            <div className="instruccion-texto">
              <strong>Completa los datos del cliente</strong>
              <span>
                Incluye: Nombre, Email, Teléfono, Deuda Total, Asesor Asignado
              </span>
            </div>
          </div>
          <div className="instruccion-item">
            <div className="instruccion-numero">3</div>
            <div className="instruccion-texto">
              <strong>Sube el archivo</strong>
              <span>
                Arrastra y suelta o haz clic para seleccionar el archivo
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="importar-card">
        <h2>Plantilla de importación</h2>
        <div className="plantilla-row">
          <div className="plantilla-info">
            <div className="plantilla-icon">
              <FileSpreadsheet size={20} color="#16a34a" />
            </div>
            <div className="plantilla-texto">
              <strong>plantilla_clientes.xlsx</strong>
              <span>Plantilla Excel con formato validado</span>
            </div>
          </div>
          <button className="btn-descargar">
            <Download size={16} /> Descargar Plantilla
          </button>
        </div>
      </div>

      <div className="importar-card">
        <h2>Subir Archivo</h2>
        <div
          className={`dropzone ${dragActivo ? "activo" : ""}`}
          onDragOver={(e) => {
            e.preventDefault();
            setDragActivo(true);
          }}
          onDragLeave={() => setDragActivo(false)}
          onDrop={handleDrop}
          onClick={() => inputRef.current.click()}
        >
          <Upload size={28} color="#94a3b8" />
          <p>Arrastra y suelta tu archivo Excel aquí</p>
          <p>o haz clic para seleccionar un archivo</p>
          <button
            className="btn-seleccionar"
            onClick={(e) => {
              e.stopPropagation();
              inputRef.current.click();
            }}
          >
            Seleccionar Archivo
          </button>
          <small>Formatos soportados: .xlsx, .xls, .csv (máx. 10MB)</small>
          <input
            ref={inputRef}
            type="file"
            accept=".xlsx,.xls,.csv"
            style={{ display: "none" }}
            onChange={(e) => handleArchivo(e.target.files[0])}
          />
        </div>
        {archivoSeleccionado && (
          <div className="archivo-seleccionado">
            <FileSpreadsheet size={16} />
            {archivoSeleccionado.name} seleccionado correctamente
          </div>
        )}
      </div>

      <div className="importar-card">
        <h2>Importaciones Recientes</h2>
        <div className="importaciones-list">
          {importacionesRecientes.map((imp) => (
            <div className="importacion-item" key={imp.id}>
              <div className="importacion-info">
                <div className="importacion-icon">
                  <FileSpreadsheet size={18} color="#3b82f6" />
                </div>
                <div className="importacion-texto">
                  <strong>{imp.nombre}</strong>
                  <span>
                    {imp.fecha} • {imp.registros} registros
                  </span>
                </div>
              </div>
              <button className="btn-reimportar">
                <Download size={16} />
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ImportarPage;
