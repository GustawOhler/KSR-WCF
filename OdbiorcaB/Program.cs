using MassTransit;
using RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdbiorcaA;
using OdbiorcaC;

namespace OdbiorcaB
{

    public class WiadomoscTyp3 : YourMessage, IMessage
    {
        public string text { get; set; }
        public string Text2 { get; set; }
    }
    public class Licznik
    {
        public int licznik { get; set; }
        public Licznik()
        {
            licznik = 0;
        }
    }

    public class HandlerClass : IConsumer<WiadomoscTyp3>
    {
        private Licznik licz;
        private readonly object _locker = new object();

        public HandlerClass(Licznik l)
        {
            licz = l;
        }
        public async Task Consume(ConsumeContext<WiadomoscTyp3> ctx)
        {
            lock (_locker)
            {
                licz.licznik++;
                Console.Out.WriteLine($"Liczba otrzymanych wiadomosci: {licz.licznik}");
            }
            /*foreach (var hdr in ctx.Headers.GetAll())
            {
                await Console.Out.WriteLineAsync($"{hdr.Key}: {hdr.Value}");
            }*/
            await Console.Out.WriteLineAsync($"Otrzymano wiadomosc ze stara trescia: {ctx.Message.Text2} oraz nowa: {ctx.Message.text}");
        }
    }

    class Program
    {
        /*public static Task Handle(ConsumeContext<YourMessage> ctx)
        {
            foreach (var hdr in ctx.Headers.GetAll())
            {
                Console.WriteLine("{0}: {1}", hdr.Key, hdr.Value);
            }
            return Console.Out.WriteLineAsync($"received: {ctx.Message.Text}");
        }*/
        static void Main(string[] args)
        {
            Licznik licznik = new Licznik();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"),
                h => { h.Username("guest"); h.Password("guest"); });
                sbc.ReceiveEndpoint(host, "asyncqueue", ep =>
                {
                    ep.Consumer(() => new HandlerClass(licznik));
                });
            });
            bus.Start();
            Console.WriteLine("odbiorca wystartował");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
