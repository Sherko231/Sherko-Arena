using System.Collections.Generic;
using UnityEngine;

namespace Sherko.Registries
{
    public interface IRegistry<T> where T : MonoBehaviour
    {
        Dictionary<int, T> RegistryDictionary { get; }
        void Register(T unit);
        void Unregister(int instanceID);
        bool Contains(int instanceID);
        T Get(int instanceID);
    }
}

