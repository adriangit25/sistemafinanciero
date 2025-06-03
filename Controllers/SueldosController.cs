using Microsoft.AspNetCore.Mvc;
using SF.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SF.Controllers
{
    public class SueldosController : Controller
    {
        private readonly string _connectionString;

        public SueldosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Método auxiliar para cargar empleados (evita duplicación)
        private List<Usuario> CargarEmpleados()
        {
            var empleados = new List<Usuario>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT reg_id, reg_nombre, reg_usuario FROM tbl_registro", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                empleados.Add(new Usuario
                {
                    reg_id = reader.GetInt32("reg_id"),
                    reg_nombre = reader.GetString("reg_nombre"),
                    reg_usuario = reader.GetString("reg_usuario")
                });
            }
            return empleados;
        }

        // LISTADO con filtro por empleado
        public IActionResult Index(int? reg_id)
        {
            var rol = HttpContext.Session.GetString("rol");
            var sueldos = new List<Sueldo>();
            var empleados = CargarEmpleados();

            ViewBag.Empleados = empleados;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var sql = @"
                SELECT s.*, r.reg_nombre, r.reg_usuario 
                FROM tbl_sueldo s
                JOIN tbl_registro r ON s.reg_id = r.reg_id ";

            if (reg_id.HasValue && reg_id > 0)
                sql += "WHERE s.reg_id = @reg_id ";

            sql += "ORDER BY s.fecha DESC, s.sueldo_id DESC";

            using var cmd = new MySqlCommand(sql, conn);
            if (reg_id.HasValue && reg_id > 0)
                cmd.Parameters.AddWithValue("@reg_id", reg_id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sueldos.Add(new Sueldo
                {
                    sueldo_id = reader.GetInt32("sueldo_id"),
                    reg_id = reader.GetInt32("reg_id"),
                    sueldo = reader.GetDecimal("sueldo"),
                    bono = reader.GetDecimal("bono"),
                    total_ingresos = reader.GetDecimal("total_ingresos"),
                    deduccion_ap_pers = reader.GetDecimal("deduccion_ap_pers"),
                    anticipo = reader.GetDecimal("anticipo"),
                    total_deducciones = reader.GetDecimal("total_deducciones"),
                    liquido_recibir = reader.GetDecimal("liquido_recibir"),
                    fecha = reader.GetDateTime("fecha"),
                    empleado_nombre = reader["reg_nombre"].ToString(),
                    empleado_usuario = reader["reg_usuario"].ToString()
                });
            }

            return View(sueldos);
        }

        // GET: Crear sueldo
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("rol") != "administrador")
                return RedirectToAction("Index", "Dashboard");

            ViewBag.Empleados = CargarEmpleados();
            return View();
        }

        // POST: Crear sueldo
        [HttpPost]
        public IActionResult Crear(Sueldo nuevoSueldo) // renombrado para evitar el warning MVC1004
        {
            if (HttpContext.Session.GetString("rol") != "administrador")
                return RedirectToAction("Index", "Dashboard");

            // --- RE-CALCULA AQUÍ TODOS LOS CAMPOS ---
            nuevoSueldo.total_ingresos = nuevoSueldo.sueldo + nuevoSueldo.bono;
            nuevoSueldo.deduccion_ap_pers = nuevoSueldo.total_ingresos * 0.0945M;
            nuevoSueldo.total_deducciones = nuevoSueldo.deduccion_ap_pers + nuevoSueldo.anticipo;
            nuevoSueldo.liquido_recibir = nuevoSueldo.total_ingresos - nuevoSueldo.total_deducciones;
            // ----------------------------------------

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = new MySqlCommand(@"
                INSERT INTO tbl_sueldo
                (reg_id, sueldo, bono, total_ingresos, deduccion_ap_pers, anticipo, total_deducciones, liquido_recibir, fecha)
                VALUES (@reg_id, @sueldo, @bono, @total_ingresos, @deduccion_ap_pers, @anticipo, @total_deducciones, @liquido_recibir, CURDATE())", conn);

            cmd.Parameters.AddWithValue("@reg_id", nuevoSueldo.reg_id);
            cmd.Parameters.AddWithValue("@sueldo", nuevoSueldo.sueldo);
            cmd.Parameters.AddWithValue("@bono", nuevoSueldo.bono);
            cmd.Parameters.AddWithValue("@total_ingresos", nuevoSueldo.total_ingresos);
            cmd.Parameters.AddWithValue("@deduccion_ap_pers", nuevoSueldo.deduccion_ap_pers);
            cmd.Parameters.AddWithValue("@anticipo", nuevoSueldo.anticipo);
            cmd.Parameters.AddWithValue("@total_deducciones", nuevoSueldo.total_deducciones);
            cmd.Parameters.AddWithValue("@liquido_recibir", nuevoSueldo.liquido_recibir);

            cmd.ExecuteNonQuery();

            TempData["Mensaje"] = "Registro guardado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
