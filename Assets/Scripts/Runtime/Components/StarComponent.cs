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
        public GameObject LetterGameObject;
        public TMP_Text LetterText;
        public RectTransform RootTransform;
        public int indexChar;
        public AnimationFlightParameters AnimationFlightParameters;
    }

    public struct AnimationFlightParameters
    {
        public Vector3 FinishPosition;
        public GameObject Letter;
        public RectTransform RootTransform;
    }
}