using Npgsql;

namespace SimpleADONet.Config
{
    public class DbConnector
    {
        public NpgsqlConnection Connection()
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var username = Environment.GetEnvironmentVariable("DB_USERNAME");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            var connectionString = $"Host={host};Username={username};Password={password};Database=fullstuck_db";
            NpgsqlConnection conn;

            try
            {
                conn = new NpgsqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return conn;
        }
    }
}