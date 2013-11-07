Greyhound
====

Greyhound is a very tiny, in-memory message bus. It supports basic features like message filtering, 
error handling and persistance.

**Simple Usage**

    var bus = new GreyhoundBus();
    bus.AddSubscriber(new MySubscriber());
    bus.PutMessage(Message.Create("Hello"));

    public class MySubscriber : ISubscriber<string>
    {
        public IEnumerable<IFilter<string>> GetFilters()
        {
            return Filter.NoFilter<string>(); //"Synthetic sugar" instead of returning empty list
        }

        public void OnMessage(IMessageContext<string> messageContext)
        {
            //Handle message here
        }
    }

**Async Await support**

If you have a long-running process inside a subscriber, you might want to do it asynchronously. 
This can be achieved by inheriting from ´AsyncSubscriber´ (which in turn implements `ISubscriber`)

In an `AsyncSubscriber` you will instead override the `OnMessageAsync` method, returning a `Task`.

    public class MyAsyncSubscriber : AsyncSubscriber<string>
    {
        public overrides IEnumerable<IFilter<string>> GetFilters()
        {
            return Filter.NoFilter<string>(); //"Synthetic sugar" instead of returning empty list
        }

        public overrides async Task OnMessageAsync(IMessageContext<string> messageContext)
        {
            await DoSomethingAsync();
        }
    }

**Filtering**

Each subscriber is responsible for returning any filters in the `GetFilters()` method.
If the subscriber doesn't have any filters, an empty collection must be returned. 
Alternatively you can use the `Filter.NoFilter<T>()` method for added readability.

For simple filtering, Greyhound offers the BasicFilter class, which accepts a delegate that
becomes the filter:

    new BasicFilter(x => x.SomeProperty == true);

_Important:_ When returning multiple filters, it is recommended to use yield return, 
as filters are evaluated sequentially

**Message Type Filtering**

In addition to normal filters, each subscriber automatically filters messages on its 
generic type parameter. For instance, a implementation of `ISubscriber<SomeClass>` will
only recieve messages of type `IMessage<SomeClass>` **or any message with a subclass of SomeClass**.
This allow us to create "CatchAll" or "CatchMany" subscribers that can catch messages based on
types they have in common.

Example:

We have two message types; `InitCommand` and `StartCommand`, both of which implement `ICommand`. 
We can now define two subscribers, implementing `ISubscriber<InitCommand>` and `ISubscriber<StartCommand>` 
respectively. In addition to this we can implement a subscriber implementing `ISubscriber<ICommand>` that will
catch all messages that implement `ICommand`

**Putting messages from subscribers**

If you want to put a new message on the bus from inside a subscriber, this can be accomplished using the
`IMessageContext<T>.PutEvent<TEvent>(IMessage<TEvent>)` method like so:

     public void OnMessage(IMessageContext<InitCommand> messageContext)
    {
        messageContext.PutEvent(Message.Create(new StartCommand());
    }

**Handling errors**

By default, if a subscriber throws an exception, this exception is discarded. However you may register 
one or more `ErrorSubscriber`s on the bus to catch these exceptions yourself.

An ErrorSubscriber is implemented in exactly the same way as a normal subscriber. It does not allow basic 
filtering, however the type filtering rules still apply. An `ErrorSubscriber` is registered the same way
as normal subscribers.

_Example:_

    //Because of the type matching mechanism, this will catch all errors regardless of message type
    public class CatchAllErrorSubscriber : ErrorSubscriber<object>
    {
        public override void OnError(ErrorMessage<object> errorMessage)
        {
            Log.Error(String.Format("An error occured on subscriber {0}. Exception: {1}",
                         errorMessage.SubscriberName, errorMessage.Exception);
        }
    }
    
    //Add the subscriber as any subscriber
    bus.AddSubscriber(new CatchAllErrorSubscriber());

_Remarks_
An `ErrorSubscriber` cannot put anything back on the bus, nor does any exception it throws get handled.
ErrorSubscribers are only intended to perform things like logging

**Persistance**

Greyhound supports persisting messages on any storage space, in case the app domain should shut down unexpectedly.
Per default, messages are only stored in memory, however you may implement your own persistance by implementing
`IPersistor`, and applying it to the bus's `Pipeline` property:

    var bus = new GreyhoundBus("MyBus"); //Provide our own name to the bus.
    
    bus.Pipeline.Persistor = new MyCustomRavenDbPersistor();

    //Add subscribers here

    bus.Restore(); //Finally, restore the bus

    public class MyCustomRavenDbPersistor : IPersistor
    {
        public void Persist(string busName, Guid key, IMessage<object> message)
        {
            //Magic
        }

        public void Delete(Guid key)
        {
            //More Magic
        }

         //This is where our custom name comes in handy   
        public IEnumerable<IMessage<object>> Restore(string busName)
        {
            //Restoration magic
        }
    }

**Message preprocessing**

For all your corner-case needs, we've included the ability to preprocces messages! Simply implement
`IMessageProcessor<T>` and add it to the bus's pipeline. The same type filtering rules as with
subscribers apply.

_Example:_

    bus.Pipeline.AddMessageProcessor(new InitMessageProcessor());

    public class InitMessageProcessor : IMessageProcessor<InitCommand>
    {
        void ProcessMessage(IMessagePipelineContext<T> context)
        {
            if(context.Message.Data.SomeProperty == "invalid")
                context.Cancel = true;            
        }
    }
