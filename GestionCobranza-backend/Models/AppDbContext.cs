using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alertum> Alerta { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Deudum> Deuda { get; set; }

    public virtual DbSet<GestionCobranza> GestionCobranzas { get; set; }

    public virtual DbSet<ImportacionDato> ImportacionDatos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<ReporteGenerado> ReporteGenerados { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=dpg-d7cmq6vlk1mc73e8stpg-a.oregon-postgres.render.com;Port=5432;Database=cobranzas_belleza;Username=admin;Password=OdQ3ebn4tjziIPaLZG4KTVPI9V76FwDp;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alertum>(entity =>
        {
            entity.HasKey(e => e.IdAlerta).HasName("alerta_pkey");

            entity.ToTable("alerta");

            entity.Property(e => e.IdAlerta).HasColumnName("id_alerta");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.FechaAlerta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alerta");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdDeuda).HasColumnName("id_deuda");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje).HasColumnName("mensaje");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(20)
                .HasColumnName("prioridad");
            entity.Property(e => e.TipoAlerta)
                .HasMaxLength(30)
                .HasColumnName("tipo_alerta");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Alerta)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("fk_alerta_cliente");

            entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.Alerta)
                .HasForeignKey(d => d.IdDeuda)
                .HasConstraintName("fk_alerta_deuda");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("cliente_pkey");

            entity.ToTable("cliente");

            entity.HasIndex(e => e.Dni, "cliente_dni_key").IsUnique();

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .HasColumnName("dni");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.EstadoCliente)
                .HasMaxLength(30)
                .HasDefaultValueSql("'NUEVO'::character varying")
                .HasColumnName("estado_cliente");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdAsesor).HasColumnName("id_asesor");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.Observacion).HasColumnName("observacion");
            entity.Property(e => e.Riesgo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'BAJO'::character varying")
                .HasColumnName("riesgo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdAsesorNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdAsesor)
                .HasConstraintName("fk_cliente_asesor");
        });

        modelBuilder.Entity<Deudum>(entity =>
        {
            entity.HasKey(e => e.IdDeuda).HasName("deuda_pkey");

            entity.ToTable("deuda");

            entity.Property(e => e.IdDeuda).HasColumnName("id_deuda");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DiasAtraso)
                .HasDefaultValue(0)
                .HasColumnName("dias_atraso");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.EstadoDeuda)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PENDIENTE'::character varying")
                .HasColumnName("estado_deuda");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.MontoPagado)
                .HasPrecision(10, 2)
                .HasColumnName("monto_pagado");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(10, 2)
                .HasColumnName("monto_total");
            entity.Property(e => e.SaldoPendiente)
                .HasPrecision(10, 2)
                .HasColumnName("saldo_pendiente");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Deuda)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_deuda_cliente");
        });

        modelBuilder.Entity<GestionCobranza>(entity =>
        {
            entity.HasKey(e => e.IdGestion).HasName("gestion_cobranza_pkey");

            entity.ToTable("gestion_cobranza");

            entity.Property(e => e.IdGestion).HasColumnName("id_gestion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.FechaGestion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_gestion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.ProximaAccion).HasColumnName("proxima_accion");
            entity.Property(e => e.Resultado)
                .HasMaxLength(50)
                .HasColumnName("resultado");
            entity.Property(e => e.TipoGestion)
                .HasMaxLength(30)
                .HasColumnName("tipo_gestion");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.GestionCobranzas)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gestion_cliente");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.GestionCobranzas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gestion_usuario");
        });

        modelBuilder.Entity<ImportacionDato>(entity =>
        {
            entity.HasKey(e => e.IdImportacion).HasName("importacion_datos_pkey");

            entity.ToTable("importacion_datos");

            entity.Property(e => e.IdImportacion).HasColumnName("id_importacion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.EstadoImportacion)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PROCESADO'::character varying")
                .HasColumnName("estado_importacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(200)
                .HasColumnName("nombre_archivo");
            entity.Property(e => e.Observacion).HasColumnName("observacion");
            entity.Property(e => e.RutaArchivo).HasColumnName("ruta_archivo");
            entity.Property(e => e.TotalRegistros)
                .HasDefaultValue(0)
                .HasColumnName("total_registros");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ImportacionDatos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_importacion_usuario");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("pago_pkey");

            entity.ToTable("pago");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ComprobanteUrl).HasColumnName("comprobante_url");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.EstadoPago)
                .HasMaxLength(20)
                .HasDefaultValueSql("'CONFIRMADO'::character varying")
                .HasColumnName("estado_pago");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdDeuda).HasColumnName("id_deuda");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(30)
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Nota).HasColumnName("nota");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdDeuda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pago_deuda");
        });

        modelBuilder.Entity<ReporteGenerado>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("reporte_generado_pkey");

            entity.ToTable("reporte_generado");

            entity.Property(e => e.IdReporte).HasColumnName("id_reporte");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ArchivoUrl).HasColumnName("archivo_url");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.FechaDesde).HasColumnName("fecha_desde");
            entity.Property(e => e.FechaGeneracion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_generacion");
            entity.Property(e => e.FechaHasta).HasColumnName("fecha_hasta");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreReporte)
                .HasMaxLength(150)
                .HasColumnName("nombre_reporte");
            entity.Property(e => e.TipoReporte)
                .HasMaxLength(50)
                .HasColumnName("tipo_reporte");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ReporteGenerados)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reporte_usuario");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("rol_pkey");

            entity.ToTable("rol");

            entity.HasIndex(e => e.Nombre, "rol_nombre_key").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "usuario_correo_key").IsUnique();

            entity.HasIndex(e => e.Dni, "usuario_dni_key").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .HasColumnName("dni");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ACTIVO'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(100)
                .HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
