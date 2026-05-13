using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionCobranza_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialLocalCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rol_pkey", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_rol = table.Column<int>(type: "integer", nullable: false),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dni = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'ACTIVO'::character varying"),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuario_pkey", x => x.id_usuario);
                    table.ForeignKey(
                        name: "fk_usuario_rol",
                        column: x => x.id_rol,
                        principalTable: "rol",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_asesor = table.Column<int>(type: "integer", nullable: true),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dni = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    estado_cliente = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValueSql: "'NUEVO'::character varying"),
                    riesgo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'BAJO'::character varying"),
                    observacion = table.Column<string>(type: "text", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cliente_pkey", x => x.id_cliente);
                    table.ForeignKey(
                        name: "fk_cliente_asesor",
                        column: x => x.id_asesor,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "importacion_datos",
                columns: table => new
                {
                    id_importacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ruta_archivo = table.Column<string>(type: "text", nullable: true),
                    total_registros = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    estado_importacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValueSql: "'PROCESADO'::character varying"),
                    observacion = table.Column<string>(type: "text", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("importacion_datos_pkey", x => x.id_importacion);
                    table.ForeignKey(
                        name: "fk_importacion_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "reporte_generado",
                columns: table => new
                {
                    id_reporte = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    nombre_reporte = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    tipo_reporte = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_desde = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_hasta = table.Column<DateOnly>(type: "date", nullable: true),
                    archivo_url = table.Column<string>(type: "text", nullable: true),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reporte_generado_pkey", x => x.id_reporte);
                    table.ForeignKey(
                        name: "fk_reporte_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "deuda",
                columns: table => new
                {
                    id_deuda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    monto_total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    monto_pagado = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    saldo_pendiente = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_emision = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_vencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    dias_atraso = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    estado_deuda = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'PENDIENTE'::character varying"),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deuda_pkey", x => x.id_deuda);
                    table.ForeignKey(
                        name: "fk_deuda_cliente",
                        column: x => x.id_cliente,
                        principalTable: "cliente",
                        principalColumn: "id_cliente");
                });

            migrationBuilder.CreateTable(
                name: "gestion_cobranza",
                columns: table => new
                {
                    id_gestion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    tipo_gestion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    fecha_gestion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    resultado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    proxima_accion = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gestion_cobranza_pkey", x => x.id_gestion);
                    table.ForeignKey(
                        name: "fk_gestion_cliente",
                        column: x => x.id_cliente,
                        principalTable: "cliente",
                        principalColumn: "id_cliente");
                    table.ForeignKey(
                        name: "fk_gestion_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "alerta",
                columns: table => new
                {
                    id_alerta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: true),
                    id_deuda = table.Column<int>(type: "integer", nullable: true),
                    tipo_alerta = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: false),
                    prioridad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    leido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_alerta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("alerta_pkey", x => x.id_alerta);
                    table.ForeignKey(
                        name: "fk_alerta_cliente",
                        column: x => x.id_cliente,
                        principalTable: "cliente",
                        principalColumn: "id_cliente");
                    table.ForeignKey(
                        name: "fk_alerta_deuda",
                        column: x => x.id_deuda,
                        principalTable: "deuda",
                        principalColumn: "id_deuda");
                });

            migrationBuilder.CreateTable(
                name: "pago",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deuda = table.Column<int>(type: "integer", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_pago = table.Column<DateOnly>(type: "date", nullable: false),
                    metodo_pago = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    comprobante_url = table.Column<string>(type: "text", nullable: true),
                    nota = table.Column<string>(type: "text", nullable: true),
                    estado_pago = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'CONFIRMADO'::character varying"),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuario_modificacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pago_pkey", x => x.id_pago);
                    table.ForeignKey(
                        name: "fk_pago_deuda",
                        column: x => x.id_deuda,
                        principalTable: "deuda",
                        principalColumn: "id_deuda");
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerta_id_cliente",
                table: "alerta",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_alerta_id_deuda",
                table: "alerta",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "cliente_dni_key",
                table: "cliente",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cliente_id_asesor",
                table: "cliente",
                column: "id_asesor");

            migrationBuilder.CreateIndex(
                name: "IX_deuda_id_cliente",
                table: "deuda",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_gestion_cobranza_id_cliente",
                table: "gestion_cobranza",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_gestion_cobranza_id_usuario",
                table: "gestion_cobranza",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_importacion_datos_id_usuario",
                table: "importacion_datos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_pago_id_deuda",
                table: "pago",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "IX_reporte_generado_id_usuario",
                table: "reporte_generado",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "rol_nombre_key",
                table: "rol",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_id_rol",
                table: "usuario",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "usuario_correo_key",
                table: "usuario",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "usuario_dni_key",
                table: "usuario",
                column: "dni",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerta");

            migrationBuilder.DropTable(
                name: "gestion_cobranza");

            migrationBuilder.DropTable(
                name: "importacion_datos");

            migrationBuilder.DropTable(
                name: "pago");

            migrationBuilder.DropTable(
                name: "reporte_generado");

            migrationBuilder.DropTable(
                name: "deuda");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "rol");
        }
    }
}
