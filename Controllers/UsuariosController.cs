using Microsoft.AspNetCore.Mvc;
using SF.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace SF.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly string _connectionString;
        public UsuariosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // SOLO ADMIN
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("rol");
            if (rol != "administrador")
                return RedirectToAction("Index", "Dashboard");

            List<Usuario> usuarios = new List<Usuario>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerUsuariosYRoles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                reg_id = reader.GetInt32("reg_id"),
                                reg_usuario = reader.GetString("reg_usuario"),
                                reg_nombre = reader.GetString("reg_nombre"),
                                reg_apellido = reader.IsDBNull(reader.GetOrdinal("reg_apellido")) ? "" : reader.GetString("reg_apellido"),
                                reg_telefono = reader.IsDBNull(reader.GetOrdinal("reg_telefono")) ? "" : reader.GetString("reg_telefono"),
                                reg_estado = reader.IsDBNull(reader.GetOrdinal("reg_estado")) ? "" : reader.GetString("reg_estado"),
                                rol_nombre = reader.IsDBNull(reader.GetOrdinal("rol_nombre")) ? "Sin Rol" : reader.GetString("rol_nombre")
                            });
                        }
                    }
                }
            }
            return View(usuarios);
        }
    }
}
