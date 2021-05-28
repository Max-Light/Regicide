using System;

public interface IObservable
{
    void AddObserver(Action action);
    void RemoveObserver(Action action);
}

public interface IObservable<T>
{
    void AddObserver(Action<T> action);
    void RemoveObserver(Action<T> action);
}