namespace Recellection.Code.Utility.Events
{
    using System;

    public delegate void Publish<T>(object publisher, Event<T> ev);
}
