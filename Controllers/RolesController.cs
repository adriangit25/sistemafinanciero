using Microsoft.AspNetCore.Mvc;
using SF.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace SF.Controllers
{
    public class RolesController : Controller
    {
        private readonly string _connectionString;
        public RolesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Listar roles
        public IActionResult Index()
        {
            List<Rol> roles = new List<Rol>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerRoles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Rol
                            {
                                rol_id = reader.GetInt32("rol_id"),
                                rol_nombre = reader.GetString("rol_nombre"),
                                rol_descripcion = reader.GetString("rol_descripcion"),
                                rol_estado = reader.GetString("rol_estado")
                            });
                        }
                    }
                }
            }
            return View(roles);
        }

        // Crear (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // Crear (POST)
        [HttpPost]
        public IActionResult Crear(Rol rol)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CrearRol", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_rol_nombre", rol.rol_nombre);
                    cmd.Parameters.AddWithValue("@p_rol_descripcion", rol.rol_descripcion);
                    cmd.Parameters.AddWithValue("@p_rol_estado", rol.rol_estado ?? "activo");
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // Editar (GET)
        public IActionResult Editar(int id)
        {
            Rol? rol = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerRolPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_rol_id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rol = new Rol
                            {
                                rol_id = reader.GetInt32("rol_id"),
                                rol_nombre = reader.GetString("rol_nombre"),
                                rol_descripcion = reader.GetString("rol_descripcion"),
                                rol_estado = reader.GetString("rol_estado")
                            };
                        }
                    }
                }
            }
            if (rol == null)
                return NotFound();
            return View(rol);
        }

        // Editar (POST)
        [HttpPost]
        public IActionResult Editar(Rol rol)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ActualizarRol", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_rol_id", rol.rol_id);
                    cmd.Parameters.AddWithValue("@p_rol_nombre", rol.rol_nombre);
                    cmd.Parameters.AddWithValue("@p_rol_descripcion", rol.rol_descripcion);
                    cmd.Parameters.AddWithValue("@p_rol_estado", rol.rol_estado);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // Eliminar (GET)
        public IActionResult Eliminar(int id)
        {
            Rol? rol = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerRolPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_rol_id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rol = new Rol
                            {
                                rol_id = reader.GetInt32("rol_id"),
                                rol_nombre = reader.GetString("rol_nombre"),
                                rol_descripcion = reader.GetString("rol_descripcion"),
                                rol_estado = reader.GetString("rol_estado")
                            };
                        }
                    }
                }
            }
            if (rol == null)
                return NotFound();
            return View(rol);
        }

        // Eliminar (POST)
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int rol_id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("EliminarRol", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_rol_id", rol_id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // ASIGNAR ROL A USUARIO (GET)
        public IActionResult Asignar()
        {
            // Solo admin puede asignar
            var rolSesion = HttpContext.Session.GetString("rol");
            if (rolSesion != "administrador")
                return RedirectToAction("Index", "Dashboard");

            var model = new AsignarRolViewModel();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // Obtener usuarios
                using (var cmd = new MySqlCommand("SELECT reg_id, reg_usuario, reg_nombre FROM tbl_registro", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Usuarios.Add(new Usuario
                            {
                                reg_id = reader.GetInt32("reg_id"),
                                reg_usuario = reader.GetString("reg_usuario"),
                                reg_nombre = reader.GetString("reg_nombre")
                            });
                        }
                    }
                }

                // Obtener roles
                using (var cmd = new MySqlCommand("ObtenerRoles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Roles.Add(new Rol
                            {
                                rol_id = reader.GetInt32("rol_id"),
                                rol_nombre = reader.GetString("rol_nombre")
                            });
                        }
                    }
                }
            }

            return View(model);
        }

        // ASIGNAR ROL A USUARIO (POST)
        [HttpPost]
        public IActionResult Asignar(AsignarRolViewModel model)
        {
            // Solo admin puede asignar
            var rolSesion = HttpContext.Session.GetString("rol");
            if (rolSesion != "administrador")
                return RedirectToAction("Index", "Dashboard");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("AsignarRolUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reg_id", model.reg_id);
                    cmd.Parameters.AddWithValue("@p_rol_id", model.rol_id);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Rol asignado correctamente.";
            return RedirectToAction("Asignar");
        }
    }
}
