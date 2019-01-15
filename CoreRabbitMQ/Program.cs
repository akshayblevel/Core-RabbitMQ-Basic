using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreRabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            const string queueName = "MyQueue";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);

                // consumer

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queueName, true, consumer);

                // publisher

                var props = channel.CreateBasicProperties();
                int i = 0;

                while (true)
                {
                    var messageBody = Encoding.UTF8.GetBytes($"Message {++i}");
                    channel.BasicPublish("", queueName, props, messageBody);
                    Thread.Sleep(50);
                }
            }

        }

        private static async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body);

            Console.WriteLine($"Begin processing {message}");

            await Task.Delay(250);

            Console.WriteLine($"End processing {message}");
        }

    }
}
