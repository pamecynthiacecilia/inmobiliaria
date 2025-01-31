﻿
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;


    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {
        this.configuration = configuration;
        connectionString = configuration["ConnectionsStrings:DefaultConnection"];
    }



		
		 
	 public List<Inmueble> ObtenerDisponible()
        {
            List<Inmueble> inmueble = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id,tipo,uso,direccion,cantAmbientes,precio,PropietarioId,disponible, p.nombre,p.apellido,p.dni,p.tel" +
                    $" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.id" +
                    $" WHERE disponibilidad= {1} ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble i = new Inmueble
                        {
							Id = reader.GetInt32(0),
							Tipo = reader.GetString(1),
							Uso = reader.GetString(2),
							Direccion = reader.GetString(3),
							CantAmbientes = reader.GetInt32(4),
							Precio = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Disponible = reader.GetInt32(7),

							PropietarioInmueble = new Propietario
							{
								Id = reader.GetInt32(6),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
								Dni = reader.GetString(10),
								Tel = reader.GetString(11),

							} 
                        };
                        inmueble.Add(i);
                    }
                    connection.Close();
                }
            }
            return inmueble;
        }

		 
		 


		public List<Inmueble> inmueblesPorPropietario(int id)
        {
			var res = new List<Inmueble>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.Id,tipo,uso,direccion,cantAmbientes,precio,PropietarioId,disponible, p.nombre,p.apellido,p.dni,p.tel FROM Inmuebles i, Propietarios p WHERE i.PropietarioId = p.Id and (p.Id=@id)"; 

		
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					connection.Open();
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble e = new Inmueble
						{
							Id = reader.GetInt32(0),
							Tipo = reader.GetString(1),
							Uso = reader.GetString(2),
							Direccion = reader.GetString(3),
							CantAmbientes = reader.GetInt32(4),
							Precio = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Disponible = reader.GetInt32(7),

							PropietarioInmueble = new Propietario
							{
								Id = reader.GetInt32(6),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
								Dni = reader.GetString(10),
								Tel = reader.GetString(11),

							}
						};
						res.Add(e);
					}
					connection.Close();
				}
				return res;
			}

		}

		public List<Inmueble> ObtenerTodos()
		{
			var res = new List<Inmueble>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.id, tipo, uso, direccion, cantAmbientes, precio, propietarioId, disponible," +
					" p.nombre,p.apellido FROM Inmuebles i, Propietarios p WHERE i.propietarioId = p.id"; ;

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble e = new Inmueble
						{
							Id = reader.GetInt32(0),
							Tipo = reader.GetString(1),
							Uso = reader.GetString(2),
							Direccion = reader.GetString(3),
							CantAmbientes = reader.GetInt32(4),
							Precio = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Disponible = reader.GetInt32(7),

							PropietarioInmueble = new Propietario
							{
								Id = reader.GetInt32(6),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),

							}
						};
						res.Add(e);
					}
					connection.Close();
				}
				return res;
			}
		
		}



		public int Agregar(Inmueble inmueble)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmuebles (tipo,uso,direccion, cantAmbientes, precio,propietarioId,disponible,imagen)" +
					"VALUES (@tipo,@uso,@direccion,@cantAmbientes, @precio, @propietarioId, @disponible,@imagen);" + 
					"SELECT SCOPE_IDENTITY();";

				using (var command = new SqlCommand(sql, connection))
				{

					command.Parameters.AddWithValue("@tipo", inmueble.Tipo);
					command.Parameters.AddWithValue("@uso", inmueble.Uso);
					command.Parameters.AddWithValue("@direccion", inmueble.Direccion); 
					command.Parameters.AddWithValue("@cantAmbientes", inmueble.CantAmbientes);
					command.Parameters.AddWithValue("@precio", inmueble.Precio);
					command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
					command.Parameters.AddWithValue("@disponible", inmueble.Disponible);
					if (String.IsNullOrEmpty(inmueble.Imagen))
						command.Parameters.AddWithValue("@imagen", DBNull.Value);
					else
						command.Parameters.AddWithValue("@imagen", inmueble.Imagen);
					
					connection.Open();

					res = Convert.ToInt32(command.ExecuteScalar());
					inmueble.Id = res;
					connection.Close();

				}
			}
			return res;
		}

		virtual public Inmueble ObtenerPorId(int id)
		{
			Inmueble e = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.id,tipo,uso,direccion, cantAmbientes, precio,propietarioId,disponible, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.propietarioId = p.id" +
					$" WHERE i.id=@id";


				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Inmueble
						{
							Id = reader.GetInt32(0),
							Tipo = reader.GetString(1),
							Uso = reader.GetString(2),
							Direccion = reader.GetString(3),
							CantAmbientes = reader.GetInt32(4),
						
							Precio = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Disponible = reader.GetInt32(7),

							PropietarioInmueble = new Propietario
							{
								Id = reader.GetInt32(6),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
					}
					connection.Close();
				}
				return e;
			}

		}

		public int Editar(Inmueble e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmuebles SET direccion=@direccion, cantAmbientes=@cantAmbientes, tipo=@tipo, uso=@uso, precio=@precio, " +
					"propietarioId=@propietarioId,disponible=@disponible , imagen=@imagen " +
					"WHERE id = @id";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id",e.Id);
					command.Parameters.AddWithValue("@direccion", e.Direccion);
					command.Parameters.AddWithValue("@cantAmbientes", e.CantAmbientes);
					command.Parameters.AddWithValue("@tipo", e.Tipo);
					command.Parameters.AddWithValue("@uso", e.Uso);
					command.Parameters.AddWithValue("@precio", e.Precio);
					command.Parameters.AddWithValue("@propietarioId", e.PropietarioId);
					command.Parameters.AddWithValue("@disponible", e.Disponible);
					command.Parameters.AddWithValue("@imagen", e.Imagen);

					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}





		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inmuebles WHERE Id = {id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		


	}
}
