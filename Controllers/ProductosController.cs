using Microsoft.AspNetCore.Mvc;
using SF.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace SF.Controllers
{
    public class ProductosController : Controller
    {
        private readonly string _connectionString;
        public ProductosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // LISTAR PRODUCTOS
        public IActionResult Index()
        {
            List<Producto> productos = new List<Producto>();
            int reg_id = int.Parse(HttpContext.Session.GetString("reg_id") ?? "0");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerProductosPorUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reg_id", reg_id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                pro_id = reader.GetInt32("pro_id"),
                                pro_nombre = reader.GetString("pro_nombre"),
                                pro_detalle = reader.GetString("pro_detalle"),
                                pro_estado = reader.GetString("pro_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            });
                        }
                    }
                }
            }
            return View(productos);
        }

        // CREAR (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        public IActionResult Crear(Producto producto)
        {
            int reg_id = int.Parse(HttpContext.Session.GetString("reg_id") ?? "0");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CrearProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_pro_nombre", producto.pro_nombre);
                    cmd.Parameters.AddWithValue("@p_pro_detalle", producto.pro_detalle);
                    cmd.Parameters.AddWithValue("@p_pro_estado", producto.pro_estado ?? "activo");
                    cmd.Parameters.AddWithValue("@p_reg_id", reg_id);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // EDITAR (GET)
        public IActionResult Editar(int id)
        {
            Producto? producto = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerProductoPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_pro_id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                pro_id = reader.GetInt32("pro_id"),
                                pro_nombre = reader.GetString("pro_nombre"),
                                pro_detalle = reader.GetString("pro_detalle"),
                                pro_estado = reader.GetString("pro_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            };
                        }
                    }
                }
            }
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // EDITAR (POST)
        [HttpPost]
        public IActionResult Editar(Producto producto)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ActualizarProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_pro_id", producto.pro_id);
                    cmd.Parameters.AddWithValue("@p_pro_nombre", producto.pro_nombre);
                    cmd.Parameters.AddWithValue("@p_pro_detalle", producto.pro_detalle);
                    cmd.Parameters.AddWithValue("@p_pro_estado", producto.pro_estado);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // ELIMINAR (GET)
        public IActionResult Eliminar(int id)
        {
            Producto? producto = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("ObtenerProductoPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_pro_id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                pro_id = reader.GetInt32("pro_id"),
                                pro_nombre = reader.GetString("pro_nombre"),
                                pro_detalle = reader.GetString("pro_detalle"),
                                pro_estado = reader.GetString("pro_estado"),
                                reg_id = reader.GetInt32("reg_id")
                            };
                        }
                    }
                }
            }
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int pro_id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("EliminarProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_pro_id", pro_id);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
