using System;

public interface IObservable 
{
    abstract void AddObserver(Action action);
    abstract void RemoveObserver(Action action);
}
