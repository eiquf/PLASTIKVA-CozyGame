using System;

public interface IAnimation<T>
{
    void PlayAnimation(T t, Action onComplete = null);
}