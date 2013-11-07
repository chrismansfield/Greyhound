using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Greyhound.Filters;

namespace Greyhound.Playground
{
    class Program
    {
        static void Main()
        {
            var bus = new GreyhoundBus();
            bus.AddSubscriber(new MyEventSubscriber());
            bus.AddSubscriber(new AllSubscriber());
            bus.AddSubscriber(new MyErrorSubscriber());
            bus.AddSubscriber(new MySubscriber());

            
            bus.Restore();

            for (var i = 1; i < 1000; i++)
            {
                bus.PutMessage(Message.Create(new MyMessage { Counter = i }));
            }

            //bus.PutMessage(Message.Create(new MyMessage { Counter = 2 }));
            Console.WriteLine("All messages has been put on the queue!");

            Console.ReadLine();
        }
    }

    internal interface IStuff
    {

        int Counter { get; set; }
    }

    internal class MyMessage : IStuff
    {
        public int Counter { get; set; }
    }

    class MyEvent : IStuff
    {
        public string Whatever { get; set; }
        
        public int Counter { get; set; }
    }

    internal class MySubscriber : AsyncSubscriber<MyMessage>
    {
        readonly Random _random = new Random();

        public override IEnumerable<IFilter<MyMessage>> GetFilters()
        {
            yield return new BasicFilter<MyMessage>(x => x.Data.Counter % 2 == 0);
        }

        public override async Task OnMessageAsync(IMessageContext<MyMessage> messageContext)
        {
            await Task.Delay(_random.Next(1000, 3000));
            if (messageContext.Message.Data.Counter % 10 == 0)
                throw new AbandonedMutexException("LOL");
            Console.WriteLine("Message recieved: {0}", messageContext.Message.Data.Counter);
            messageContext.PutEvent(
                Message.Create(new MyEvent
                {
                    Counter = messageContext.Message.Data.Counter,
                    Whatever = "Hello"
                }));
        }
    }

    class MyEventSubscriber : ISubscriber<MyEvent>
    {
        readonly Random _random = new Random();

        public IEnumerable<IFilter<MyEvent>> GetFilters()
        {
            return Filter.NoFilter<MyEvent>();
        }

        public void OnMessage(IMessageContext<MyEvent> messageContext)
        {
            Thread.Sleep(_random.Next(1000, 3000));
            Console.WriteLine(messageContext.Message.Data.Whatever);
        }
    }

    class AllSubscriber : ISubscriber<IStuff>
    {
        readonly Random _random = new Random();
        public IEnumerable<IFilter<IStuff>> GetFilters()
        {
            return Filter.NoFilter<IStuff>();
        }

        public void OnMessage(IMessageContext<IStuff> messageContext)
        {
            Thread.Sleep(_random.Next(1000, 3000));
            Console.WriteLine("CatchAll: {0}", messageContext.Message.Data.Counter);
        }
    }

    internal class MyErrorSubscriber : ErrorSubscriber<MyMessage>
    {
        protected override void OnError(ErrorMessage<MyMessage> message)
        {

            Console.WriteLine("An error occured on subscriber {0}. Exception: {1}", message.SubscriberName, message.Exception);
        }
    }
}
