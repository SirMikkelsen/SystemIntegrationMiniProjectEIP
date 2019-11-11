using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        var lines = System.IO.File.ReadAllLines(@"C:\Users\Gordon\Desktop\convert\hotels.csv");
                        var xml = new XElement("TopElement",
                            lines.Select(line => new XElement("Item",
                                line.Split(';')
                                    .Select((column, index) => new XElement("Column" + index, column)))));
                        xml.Save(@"C:\Users\Gordon\Desktop\convert\xmloutputhotels.xml");

                       // Console.WriteLine("convertet hotel from csv to xml");
                        break;
                    }
                    if (input == "b")
                    {
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
        }
    }
}
