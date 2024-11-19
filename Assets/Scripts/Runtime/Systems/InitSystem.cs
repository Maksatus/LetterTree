using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(InitSystem))]
    public sealed class InitSystem : UpdateSystem
    {
        private SystemsGroup _systemsGroup;
        
        public override void OnAwake()
        {
            _systemsGroup = World.CreateSystemsGroup();
            _systemsGroup.AddSystem(new ReadExelSystem());
            _systemsGroup.AddSystem(new GeneratorNameListSystem());
            _systemsGroup.Initialize();
        }

        public override void OnUpdate(float deltaTime)
        {
            _systemsGroup.Update(deltaTime);
        }
    }
}