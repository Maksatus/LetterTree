using Runtime.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class GeneratorNameListSystem : ISystem 
    {
        public World World { get; set; }
        private Filter _filter;
        
        public void OnAwake() 
        {
            Debug.LogWarning("OnAwake");
            _filter = World.Filter.With<UserComponent>().Build();
        }

        public void OnUpdate(float deltaTime) 
        {
            foreach (var entity in _filter) 
            {
                ref var userComponent = ref entity.GetComponent<UserComponent>();
                Debug.LogWarning(userComponent.Name);
            }
        }

        public void Dispose()
        {
            
        }
    }
}