using MassTransit;
using RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdbiorcaA
{

    public class YourMessage { public string Text { get; set; } }
    class Program
    {
        public static Task Handle(ConsumeContext<YourMessage> ctx)
        {
            foreach (var hdr in ctx.Headers.GetAll())
            {
                Console.WriteLine("{0}: {1}", hdr.Key, hdr.Value);
            }
            return Console.Out.WriteLineAsync($"received: {ctx.Message.Text}");
        }
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"),
                h => { h.Username("guest"); h.Password("guest"); });
                sbc.ReceiveEndpoint(host, "recvqueue", ep =>
                {
                    ep.Handler<YourMessage>(Handle);
                });
            });
            bus.Start();
            Console.WriteLine("odbiorca wystartował");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
