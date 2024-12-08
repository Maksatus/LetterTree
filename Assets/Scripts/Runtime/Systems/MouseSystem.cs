using Runtime.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class MouseSystem : ISystem
    {
        public World World { get; set; }

        private Camera _mainCamera;
        private Transform _vfxTransform;
        private Mouse _currentMouse;
        
        public void OnAwake()
        {
            ref var dataComponent = ref World.Filter.With<GeneralGameDataComponent>().Build().First().GetComponent<GeneralGameDataComponent>();
            _mainCamera = dataComponent.Camera;
            _vfxTransform = dataComponent.VfxMouseTransform;
            _currentMouse = Mouse.current;
        }
        
        public void OnUpdate(float deltaTime)
        {
            var mousePosition = _currentMouse.position.ReadValue();
            var worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _mainCamera.nearClipPlane));
            _vfxTransform.position = new Vector3(worldPosition.x, worldPosition.y, _vfxTransform.position.z);
        }
        
        public void Dispose()
        {
            _mainCamera = null;
            _vfxTransform = null;
            _currentMouse = null;
        }
    }
}