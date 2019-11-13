using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
<<<<<<< Updated upstream
=======
using MySql.Data.MySqlClient;
>>>>>>> Stashed changes
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< Updated upstream
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

=======


            string connectionString = @"server=localhost;userid=root;
            password=root;database=skole";
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

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                        var lines = System.IO.File.ReadAllLines(@"C:\Users\Gordon\Desktop\convert\hotels.csv");
=======
                        var lines = System.IO.File.ReadAllLines(@"C:\test\kage.csv");
>>>>>>> Stashed changes
                        var xml = new XElement("TopElement",
                            lines.Select(line => new XElement("Item",
                                line.Split(';')
                                    .Select((column, index) => new XElement("Column" + index, column)))));
<<<<<<< Updated upstream
                         xml.Save(@"C:\Users\Gordon\Desktop\convert\xmloutputhotels.xml");

                       // Console.WriteLine("convertet hotel from csv to xml");
=======
                        xml.Save(@"C:\test\vedik.xml");

               
>>>>>>> Stashed changes
                        break;
                    }
                    if (input == "b")
                    {
<<<<<<< Updated upstream
                        var lines = System.IO.File.ReadAllLines(@"C:\Users\Gordon\Desktop\convert\bookings.csv");
                        var xml = new XElement("TopElement",
                            lines.Select(line => new XElement("Item",
                                line.Split(';')
                                    .Select((column, index) => new XElement("Column" + index, column)))));
                        xml.Save(@"C:\Users\Gordon\Desktop\convert\xmloutputbookings.xml");
                    //    Console.WriteLine("convertet room data from csv to xml");
                        break;
                    }

                    Console.WriteLine("wrong input");
                    break;
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
=======

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
              
                
                    break;
                }

            
                Console.WriteLine(" Press [enter] to exit.");
              
                Console.ReadLine();

            }
           
>>>>>>> Stashed changes
        }
    }
}
