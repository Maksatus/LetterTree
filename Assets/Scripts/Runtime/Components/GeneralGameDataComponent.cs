using Runtime.Providers;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Runtime.Components
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct GeneralGameDataComponent : IComponent
    {
        public string RelativePath;
        public UserProvider RefUserProvider;
        public Transform RootUsers;
        public StarProvider RefStar;
        public string Chars;
    }
}