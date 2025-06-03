using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SF.Models;
using System.Data;

namespace SF.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString;
        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string reg_usuario, string reg_contrasenia)
        {
            Usuario? usuario = null;
            string? rol = null;

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // 1. Consultar usuario
                using (var cmd = new MySqlCommand("LoginUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_usuario", reg_usuario);
                    cmd.Parameters.AddWithValue("@p_contrasenia", reg_contrasenia);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                reg_id = reader.GetInt32("reg_id"),
                                reg_usuario = reader.GetString("reg_usuario"),
                                reg_nombre = reader.GetString("reg_nombre")
                                // Otros campos si quieres
                            };
                        }
                    }
                }

                // 2. Si existe, obtener el rol principal
                if (usuario != null)
                {
                    // Abre nueva consulta porque el reader anterior ya cerró la conexión a ese DataReader
                    using (var rolCmd = new MySqlCommand("SELECT r.rol_nombre FROM tbl_usuario_rol ur JOIN tbl_rol r ON ur.rol_id = r.rol_id WHERE ur.reg_id = @reg_id LIMIT 1", conn))
                    {
                        rolCmd.Parameters.AddWithValue("@reg_id", usuario.reg_id);
                        var result = rolCmd.ExecuteScalar();
                        rol = result?.ToString() ?? "";
                    }
                }
            }

            if (usuario != null)
            {
                // Guarda nombre, reg_id y rol en sesión
                HttpContext.Session.SetString("usuario", usuario.reg_nombre ?? usuario.reg_usuario ?? "Usuario");
                HttpContext.Session.SetString("reg_id", usuario.reg_id.ToString());
                HttpContext.Session.SetString("rol", rol ?? "");

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(Usuario user)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("RegistrarUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_usuario", user.reg_usuario);
                    cmd.Parameters.AddWithValue("@p_contrasenia", user.reg_contrasenia);
                    cmd.Parameters.AddWithValue("@p_nombre", user.reg_nombre);
                    cmd.Parameters.AddWithValue("@p_apellido", user.reg_apellido);
                    cmd.Parameters.AddWithValue("@p_telefono", user.reg_telefono ?? "");
                    cmd.Parameters.AddWithValue("@p_estado", "activo");
                    cmd.Parameters.AddWithValue("@p_fecha_ingreso", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p_fecha_actual", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p_fecha_final", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            ViewBag.Message = "Usuario registrado exitosamente";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
