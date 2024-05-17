using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace Odbiorca1
{
    class Program
    {
        static readonly object lockObject = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Odbiorca 1");
            Console.WriteLine("Nacisnij przycisk zeby zaczac");
            Console.ReadKey();

            string queueName = "queueZadanie6";

            var factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "localhost",
                VirtualHost = "/"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    lock (lockObject)
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"Received: '{message}' from {queueName}");
                        Thread.Sleep(2000);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine("Nacisnij przycisk zeby skonczyc");
                Console.ReadKey();
            }
        }
    }
}