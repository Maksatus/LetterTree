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
            Application.targetFrameRate = 60;
            var readExelSystem = new ReadExelSystem();
            var generatorNameListSystem = new GeneratorNameListSystem();
            var rollSystem = new RollSystem();
          
            _systemsGroup = World.CreateSystemsGroup();
            _systemsGroup.AddInitializer(readExelSystem);
            _systemsGroup.AddInitializer(generatorNameListSystem);
            
            _systemsGroup.AddSystem(rollSystem);
            _systemsGroup.Initialize();
            
            _systemsGroup.RemoveInitializer(readExelSystem);
            _systemsGroup.RemoveInitializer(generatorNameListSystem);
        }

        public override void OnUpdate(float deltaTime)
        {
            _systemsGroup.Update(deltaTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            _systemsGroup.Dispose();
            World.RemoveSystemsGroup(_systemsGroup);
        }
    }
}