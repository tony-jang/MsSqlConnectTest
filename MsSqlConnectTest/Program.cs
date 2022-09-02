using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

public class Class
{
    public static void Main(string[] args)
    {
        int? port = null;
        Console.Write("Host to Connect: ");
        var host = Console.ReadLine();

        while (!port.HasValue)
        {
            try
            {
                Console.Write("Port to Connect: ");
                port = int.Parse(Console.ReadLine()!);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Port");
            }
        }

        Console.Write("UserID to Connect: ");
        var userId = Console.ReadLine();

        Console.Write("Password to Connect: ");
        var password = GetPassword();

        var logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File("connect_test.log")
            .CreateLogger();

        Log.Logger = logger;

        Log.Information("");
        Log.Information("<MS-SQL Connect Test>");
        Log.Information("Connecting...");

        SqlConnection? connection;

        try
        {
            connection = CreateConnection(host, port.Value, userId, password);
            connection.InfoMessage += (_, e) => { Log.Warning(e.Message); };
            connection.Open();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while connect to server..");
            return;
        }

        Log.Information("Connect Successfully!");
        Log.Information("Try to query: SELECT 1");

        try
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.ExecuteNonQuery();

            Log.Information("Query Successfully!");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while test querying..");
            return;
        }

        while (true)
        {
            Console.Write("Query: ");
            var query = Console.ReadLine();

            Log.Information("Try to query: {query}", query);

            try
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();

                Log.Information("Query succeed!");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while querying..");
            }
        }
    }

    private static SqlConnection CreateConnection(string? host, int port, string? id, string? password)
    {
        var connectionBuilder = new SqlConnectionStringBuilder
        {
            UserID = id,
            Password = password,
            DataSource = $"{host},{port}"
        };

        return new SqlConnection(connectionBuilder.ToString());
    }

    private static string GetPassword()
    {
        var pwd = new StringBuilder();
        while (true)
        {
            var i = Console.ReadKey(true);
            if (i.Key == ConsoleKey.Enter)
                break;

            if (i.Key == ConsoleKey.Backspace)
            {
                if (pwd.Length <= 0)
                    continue;

                pwd.Remove(pwd.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (i.KeyChar != '\u0000')
            {
                pwd.Append(i.KeyChar);
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return pwd.ToString();
    }
}