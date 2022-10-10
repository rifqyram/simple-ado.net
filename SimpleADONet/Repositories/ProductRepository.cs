using Npgsql;
using SimpleADONet.Config;
using SimpleADONet.Models;

namespace SimpleADONet.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnector _connector;

        public ProductRepository(DbConnector connector) => _connector = connector;

        public void CreateTable()
        {
            var connection = _connector.Connection();
            try
            {
                const string query = @"CREATE TABLE IF NOT EXISTS product(
                 product_id serial PRIMARY KEY,
                 name varchar,
                 price int,
                 stock int
                 )";

                var sqlCommand = new NpgsqlCommand(query, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();

                Console.WriteLine("Table Created Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void Save(Product product)
        {
            var connection = _connector.Connection();
            connection.Open();

            try
            {
                const string query = @"INSERT INTO product (name, price, stock) VALUES ($1, $2, $3)";
                var command = new NpgsqlCommand(query, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter { Value = product.Name },
                        new NpgsqlParameter { Value = Convert.ToInt32(product.Price) },
                        new NpgsqlParameter { Value = Convert.ToInt32(product.Stock) },
                    }
                };

                var executeNonQuery = command.ExecuteNonQuery();

                if (executeNonQuery == 0)
                {
                    throw new Exception("Error: create new product");
                }

                Console.Write("Successfully create new product");
            }
            finally
            {
                connection.Close();
            }
        }

        public void Create(Product product)
        {
            throw new NotImplementedException();
        }

        public List<Product> FindAll()
        {
            var connection = _connector.Connection();
            connection.Open();
            var result = new List<Product>();

            try
            {
                const string query = "SELECT product_id name, price, stock FROM product";
                var cmd = new NpgsqlCommand(query, connection);

                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Data is empty");
                }

                while (reader.Read())
                {
                    var product = new Product
                    {
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        Name = reader["name"].ToString(),
                        Price = Convert.ToInt32(reader["price"]),
                        Stock = Convert.ToInt32(reader["stock"])
                    };

                    result.Add(product);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public Product? FindById(int id)
        {
            Product? product = null;
            var connection = _connector.Connection();
            connection.Open();

            try
            {
                const string query = "SELECT product_id, name, price, stock FROM product WHERE product_id = $1";
                var cmd = new NpgsqlCommand(query, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter
                        {
                            Value = id
                        }
                    }
                };

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product = new Product
                    {
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        Name = reader["name"].ToString(),
                        Price = Convert.ToInt32(reader["price"]),
                        Stock = Convert.ToInt32(reader["stock"])
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return product;
        }

        public void Update(Product productUpdate)
        {
            var connection = _connector.Connection();
            var currentProduct = FindById(productUpdate.ProductId);

            try
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                const string queryUpdate = "UPDATE product SET name = $1, price = $2, stock = $3 WHERE product_id = $4";

                var updateCmd = new NpgsqlCommand(queryUpdate, connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter()
                            { Value = productUpdate.Name != null ? currentProduct?.Name : productUpdate.Name },
                        new NpgsqlParameter()
                            { Value = productUpdate.Price == 0 ? currentProduct?.Price : productUpdate.Price },
                        new NpgsqlParameter()
                            { Value = productUpdate.Stock == 0 ? currentProduct?.Stock : productUpdate.Stock },
                        new NpgsqlParameter() { Value = productUpdate.ProductId },
                    }
                };

                var execUpdate = updateCmd.ExecuteNonQuery();
                if (execUpdate == 0) transaction.Rollback();

                Console.WriteLine("Successfully update product!");
                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteById(int id)
        {
            var connection = _connector.Connection();

            try
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                const string query = "DELETE FROM product WHERE product_id = $1";
                var cmd = new NpgsqlCommand(query, connection)
                {
                    Parameters = { new NpgsqlParameter() { Value = Convert.ToInt32(id) } }
                };

                var cmdExec = cmd.ExecuteNonQuery();

                if (cmdExec == 0)
                {
                    Console.WriteLine("Failed to delete product");
                    transaction.Rollback();
                }
                
                transaction.Commit();
                Console.WriteLine("Successfully delete product");
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}