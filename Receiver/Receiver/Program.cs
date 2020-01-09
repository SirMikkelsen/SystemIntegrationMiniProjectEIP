using System;
using System.Collections.Generic;
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
            password=root;database=skole";

            string csSQLSERVER = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SuveyServiceContext-b900fe09-1066-455e-8030-f2154abf1dd6;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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

                    if(input == "c")
                    {

                        // Retrive id from  local sqlserver datbase


                        using (connection)
                        {
                            SqlCommand command = new SqlCommand(
                              "SELECT ID, FROM dbo.Suvey[data];",
                              connection);
                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine("{0}\t{1}", reader.GetInt32(0),
                                        reader.GetString(1));
                                }
                            }
                            else
                            {
                                Console.WriteLine("No rows found.");
                            }
                            reader.Close();
                        }
                     
                        //  store id in hotel database

                    }






                    break;
                }

            
                Console.WriteLine(" Press [enter] to exit.");
              
                Console.ReadLine();

            }
           
        }
    }
}
