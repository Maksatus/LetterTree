using Scellecs.Morpeh;
using TMPro;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Runtime.Components
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct StarComponent : IComponent
    {
        public GameObject RootGameObject;
        public GameObject LetterGameObject;
        public TMP_Text LetterText;
        public Transform RootTransform;
    }
}