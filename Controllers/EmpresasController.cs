using Microsoft.AspNetCore.Mvc;
using SF.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace SF.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly string _connectionString;
        public EmpresasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // LISTAR EMPRESAS
        public IActionResult Index()
        {
            List<Empresa> empresas = new List<Empresa>();
            int reg_id = int.Parse(HttpContext.Session.GetString("reg_id") ?? "0");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerEmpresasPorUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reg_id", reg_id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empresas.Add(new Empresa
                            {
                                emp_id = reader.GetInt32("emp_id"),
                                emp_nombre = reader.GetString("emp_nombre"),
                                emp_direccion = reader.GetString("emp_direccion"),
                                emp_contacto = reader.GetString("emp_contacto"),
                                emp_razonsocial = reader.GetString("emp_razonsocial"),
                                emp_estado = reader.GetString("emp_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            });
                        }
                    }
                }
            }
            return View(empresas);
        }

        // CREAR (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        public IActionResult Crear(Empresa empresa)
        {
            int reg_id = int.Parse(HttpContext.Session.GetString("reg_id") ?? "0");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CrearEmpresa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_nombre", empresa.emp_nombre);
                    cmd.Parameters.AddWithValue("@p_emp_direccion", empresa.emp_direccion);
                    cmd.Parameters.AddWithValue("@p_emp_contacto", empresa.emp_contacto);
                    cmd.Parameters.AddWithValue("@p_emp_razonsocial", empresa.emp_razonsocial);
                    cmd.Parameters.AddWithValue("@p_emp_estado", empresa.emp_estado ?? "activo");
                    cmd.Parameters.AddWithValue("@p_reg_id", reg_id);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // EDITAR (GET)
        public IActionResult Editar(int id)
        {
            Empresa? empresa = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerEmpresaPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empresa = new Empresa
                            {
                                emp_id = reader.GetInt32("emp_id"),
                                emp_nombre = reader.GetString("emp_nombre"),
                                emp_direccion = reader.GetString("emp_direccion"),
                                emp_contacto = reader.GetString("emp_contacto"),
                                emp_razonsocial = reader.GetString("emp_razonsocial"),
                                emp_estado = reader.GetString("emp_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            };
                        }
                    }
                }
            }
            if (empresa == null)
                return NotFound();
            return View(empresa);
        }

        // EDITAR (POST)
        [HttpPost]
        public IActionResult Editar(Empresa empresa)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ActualizarEmpresa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_id", empresa.emp_id);
                    cmd.Parameters.AddWithValue("@p_emp_nombre", empresa.emp_nombre);
                    cmd.Parameters.AddWithValue("@p_emp_direccion", empresa.emp_direccion);
                    cmd.Parameters.AddWithValue("@p_emp_contacto", empresa.emp_contacto);
                    cmd.Parameters.AddWithValue("@p_emp_razonsocial", empresa.emp_razonsocial);
                    cmd.Parameters.AddWithValue("@p_emp_estado", empresa.emp_estado);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // ELIMINAR (GET)
        public IActionResult Eliminar(int id)
        {
            Empresa? empresa = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerEmpresaPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empresa = new Empresa
                            {
                                emp_id = reader.GetInt32("emp_id"),
                                emp_nombre = reader.GetString("emp_nombre"),
                                emp_direccion = reader.GetString("emp_direccion"),
                                emp_contacto = reader.GetString("emp_contacto"),
                                emp_razonsocial = reader.GetString("emp_razonsocial"),
                                emp_estado = reader.GetString("emp_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            };
                        }
                    }
                }
            }
            if (empresa == null)
                return NotFound();
            return View(empresa);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int emp_id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("EliminarEmpresa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_id", emp_id);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
