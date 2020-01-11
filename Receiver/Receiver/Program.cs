using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {


            string connectionString = @"server=localhost;userid=root;
            password=root;database=hoteldb";

            string sqlServerCS = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SuveyServiceContext-b900fe09-1066-455e-8030-f2154abf1dd6;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


            //  Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = SuveyServiceContext - b900fe09 - 1066 - 455e-8030 - f2154abf1dd6; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False



            var reader = new StreamReader(File.OpenRead(@"C:\test\kage.csv"));

            MySqlConnection connectionn = null;
            connectionn = new MySqlConnection(connectionString);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    Console.WriteLine();
                    Console.WriteLine(" Select a for hotel and hit enter");
                    Console.WriteLine();
                    Console.WriteLine(" select b for room and hit enter");
                    Console.WriteLine();
                    Console.WriteLine(" select c for survey and hit enter");
                };
                channel.BasicConsume(queue: "hello",
                    autoAck: true,
                    consumer: consumer);


                while (true)
                {
                    var input = Console.ReadLine();
                    Console.WriteLine();
                    if (input == "a")
                    {
                        var lines = System.IO.File.ReadAllLines(@"C:\test\kage.csv");
                        var xml = new XElement("TopElement",
                            lines.Select(line => new XElement("Item",
                                line.Split(';')
                                    .Select((column, index) => new XElement("Column" + index, column)))));
                        xml.Save(@"C:\test\vedik.xml");


                        break;
                    }
                    if (input == "b")
                    {

                        connectionn.Open();
                        while (!reader.EndOfStream)
                        {



                            MySqlCommand cmd = new MySqlCommand();
                            cmd.Connection = connectionn;

                            var line = reader.ReadLine();

                            var values = line.Split(' ');
                            var a = values[0].Split(';');





                            cmd.CommandText = "INSERT INTO guest(name, passport_number) VALUES(@name, @passport_number)";
                            cmd.Prepare();


                            cmd.Parameters.AddWithValue("@Name", a[0]);
                            cmd.Parameters.AddWithValue("@passport_number", a[1]);
                            cmd.ExecuteNonQuery();


                        }


                        connectionn.Close();
                    }

                    if (input.Split(new char[0])[0] == "c")
                    {

                        SqlConnection conection2 = null;
                        conection2 = new SqlConnection(sqlServerCS);
                        // Retrive id from local sqlserver datbase
                        conection2.Open();


                        using (SqlCommand command = new SqlCommand(
                        "INSERT INTO dbo.Suvey (NumberOfKids, BookingEXperince, SatisfactionWitHStaff, SatisfactionWithFood, SatisfactionWithCleaning, OtherComments) VALUES (@param1, @param2, @param3, @param4, @param5, @param6);",
                          conection2))
                        {
                            command.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = -1;
                            command.Parameters.Add("@param2", System.Data.SqlDbType.Int).Value = -1;
                            command.Parameters.Add("@param3", System.Data.SqlDbType.Int).Value = -1;
                            command.Parameters.Add("@param4", System.Data.SqlDbType.Int).Value = -1;
                            command.Parameters.Add("@param5", System.Data.SqlDbType.Int).Value = -1;
                            command.Parameters.Add("@param6", System.Data.SqlDbType.VarChar).Value = "";
                            command.CommandType = System.Data.CommandType.Text;
                            command.ExecuteNonQuery();
                        }

                        string query = "SELECT TOP 1 * FROM dbo.Suvey ORDER BY id DESC";

                        SqlCommand command2 = new SqlCommand(query, conection2);

                        SqlDataReader reader2 = command2.ExecuteReader();


                        string qurryString = "SELECT TOP 1 * FROM [TABLENAME] ORDER BY id DESC";

                        List<Int32> list = new List<Int32>();

                        if (reader2.HasRows)
                        {

                            while (reader2.Read())
                            {
                               list.Add(reader2.GetInt32(0));
                            }
                           
                        }
                        else
                        {
                            Console.WriteLine("No rows found.");
                        }

                       
                        reader2.Close();


                        //store id in hotel database

                        MySqlCommand command3 = new MySqlCommand();
                        connectionn.Open();
                        command3.Connection = connectionn;

                        command3.CommandText = "INSERT INTO hotel_survey (hotel_id, survey_id) VALUES (@param1, @param2)";
                        command3.Prepare();

                        command3.Parameters.AddWithValue("@param1", input.Split(new char[0])[1]);
                        command3.Parameters.AddWithValue("@param2", list[0]);
                        command3.ExecuteNonQuery();
                        connectionn.Close();

                        break;
                    }

                }

                Console.WriteLine(" Press [enter] to exit.");

                Console.ReadLine();

            }

        }
    }
}

