﻿namespace Framework.Core;

public interface IEventListener
{
    void Bind<TEvent, THandler>() where THandler : IEventHandler<TEvent>;
    void SubscribeTo(IEventStore eventStore, string prefix = "");

    [Obsolete]
    Task ProcessEvents(IEventStore eventStore, string prefix = "");
}