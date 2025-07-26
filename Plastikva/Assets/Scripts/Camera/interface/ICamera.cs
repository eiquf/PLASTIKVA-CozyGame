using UnityEngine;

public interface ICamera<T>
{
    void Execute(Transform transform, T param);
}