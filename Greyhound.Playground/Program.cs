using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Greyhound.Filters;

namespace Greyhound.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = new GreyhoundBus();
            
            bus.AddSubscriber(new MySubscriber());
            //bus.AddSubscriber(new MyErrorSubscriber());

            for (var i = 0; i < 1000; i++)
            {
                bus.PutMessage(Message.Create(new MyMessage { Counter = i }));
            }

            //bus.PutMessage(Message.Create(new MyMessage { Counter = 10 })); 

            Console.ReadLine();
        }
    }

    internal class MyMessage
    {
        [Key]
        public int Counter { get; set; }
    }

    internal class MySubscriber:ISubscriber<MyMessage>
    {
        public IEnumerable<IFilter<MyMessage>> GetFilters()
        {
            yield return new BasicFilter<MyMessage>(x => x.Data.Counter % 2 == 0);
        }
        Random random = new Random();
        public void OnMessage(MessageContext<MyMessage> messageContext)
        {
            Thread.Sleep(random.Next(1000, 3000));
            if(messageContext.Message.Data.Counter % 10 == 0)
                throw new AbandonedMutexException("LOL");
            Console.WriteLine("Message recieved: {0}", messageContext.Message.Data.Counter);
        }
    }

    internal class MyErrorSubscriber : ErrorSubscriber<MyMessage>
    {
        protected override void OnError(MessageContext<MyMessage> messageContext, ErrorMessage<MyMessage> message)
        {
            Console.WriteLine("An error occured on subscriber {0}. Exception: {1}", message.SubscriberName, message.Exception);
        }
    }
}
