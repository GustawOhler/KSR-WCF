using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using OdbiorcaA;
using OdbiorcaC;
using OdbiorcaB;

namespace MassTransit
{ 
    
    public class WiadomosciZInterfejsem : IMessage
    {
        public string text { get; set; }
    }
    public class Program
    {
        /*public static Task Handle(ConsumeContext<YourMessage> ctx)
        {
            return Console.Out.WriteLineAsync($"received: {ctx.Message.Text}");
        }*/
        static void Main(string[] args)
        {
            WiadomosciZInterfejsem nowa = new WiadomosciZInterfejsem();
            nowa.text = "wiadomosc z interfejsu";
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                var host = sbc.Host(new Uri("rabbitmq://localhost"),
                h => { h.Username("guest"); h.Password("guest"); });
            });
            bus.Start();
            Console.WriteLine("nadawca wystartował");
            for (int i = 0; i < 10; i++)
            {
                bus.Publish(new YourMessage() { Text = "message nr" + (i + 1) },
                    ctx => { ctx.Headers.Set("klucz", "wartosc"); ctx.Headers.Set("klucz2", "jakaswartosc"); });
                bus.Publish(new WiadomoscTyp3() { Text2 = "Stara tresc wiadomosci nr " + i, text = "Nowa tresc wiadomosci nr " + i});
            }
            bus.Publish(nowa);
            Console.ReadKey();
            bus.Stop();
        }

    }
}
