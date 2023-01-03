using MySql.Data.MySqlClient;

namespace NotesApp1.Services
{
    public class ConnectionService
    {
        private string connectionString = "";
        private MySqlConnection connection;

        public MySqlConnection GetMySqlConnection()
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            return connection;
        }
    }

    /*
    public static class ConnectionServiceExtensions
    {
        public static IServiceCollection AddConnectionService(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionService>();
            return services;
        }
    }
    */
}
