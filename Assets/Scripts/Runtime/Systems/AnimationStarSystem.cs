using Runtime.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class AnimationStarSystem : ISystem
    {
        public World World { get; set; }
        private Filter _filter;
        private int _currentIndex;
        private ParticleSystem _vfxStar;
        private Camera _camera;
        private readonly ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[512];
        private readonly Dictionary<uint, AnimationFlightParameters> _particleDataDictionary = new();
        

        private const float DestinationRadius = 0.1f;
        private const float DelayRate = 0.00f;
        private const float MaxSpeed = 0.115f;

        public void OnAwake()
        {
            var generalGameDataComponent = World.Filter.With<GeneralGameDataComponent>().Build().First();
            ref var data = ref generalGameDataComponent.GetComponent<GeneralGameDataComponent>();
            _vfxStar = Object.Instantiate(data.VfxStars);
            _camera = data.Camera;
        }

        public void OnUpdate(float deltaTime)
        {
            Attract(deltaTime);
        }

        public void SetLetter(int value)
        {
            _particleDataDictionary.Clear();
            _currentIndex = value;
            _filter = World.Filter.With<StarComponent>().Build();
            var uiCanvasEntity = World.Filter.With<UICanvasComponent>().Build().First();
            ref var uiCanvasComponent = ref uiCanvasEntity.GetComponent<UICanvasComponent>();
            var fromScreenPos = ScreenToWorld(uiCanvasComponent.Chars[_currentIndex]);
            _vfxStar.transform.position = fromScreenPos;
            
            foreach (var entity in _filter)
            {
                ref var starComponent = ref entity.GetComponent<StarComponent>();
                if (starComponent.indexChar == _currentIndex)
                {
                    var targetPosition = ScreenToWorld(starComponent.RootTransform);
                    var randomSeed = (uint)Random.Range(1, uint.MaxValue);
                    var emitParams = new ParticleSystem.EmitParams
                    {
                        randomSeed = randomSeed,
                    };
                    
                    var particleData = new AnimationFlightParameters
                    {
                        FinishPosition = targetPosition,
                        Letter = starComponent.LetterGameObject,
                        RootTransform = starComponent.RootTransform,
                    };
                    _particleDataDictionary.Add(randomSeed, particleData);
                    _vfxStar.Emit(emitParams,1);
                }
            }
        }

        private void Attract(float deltaTime)
        {
            var numParticlesAlive = GetNumParticlesAlive();
            if (numParticlesAlive == 0)
                return;

            for (var i = 0; i < numParticlesAlive; i++)
            {
                var particle = _particles[i];
                var randomSeed = particle.randomSeed;
                if (_particleDataDictionary.TryGetValue(randomSeed, out var particleData))
                {
                    if (Vector3.Distance(particle.position, particleData.FinishPosition) < DestinationRadius)
                    {
                        particleData.RootTransform.DOPunchScale(Vector3.one * 1.075f, 0.5f, 4,1);
                        particleData.Letter.SetActive(true);
                        particle.remainingLifetime = 0.0f;
                        
                        _particles[i] = particle;
                        continue;
                    }

                    // var delayTime = particle.startLifetime * DelayRate;
                    // var duration = particle.startLifetime - delayTime;
                    // var time = Mathf.Max(0, particle.startLifetime - particle.remainingLifetime - delayTime);
                    //
                    // if (time <= 0) continue;


                    particle.position = GetAttractedPosition(particle.position, particleData.FinishPosition, deltaTime);
                    _particles[i] = particle;
                }
            }

            _vfxStar.SetParticles(_particles, numParticlesAlive);
        }

        public int GetNumParticlesAlive()
        {
            return _vfxStar.GetParticles(_particles);
        }

        private Vector3 ScreenToWorld(RectTransform rectTransform)
        {
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, rectTransform.position);
            return _camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, _camera.nearClipPlane + 10));
        }

        private static Vector3 GetAttractedPosition(Vector3 current, Vector3 target, float deltaTime)
        {
            return Vector3.MoveTowards(current, target, MaxSpeed * 60 * deltaTime);
        }

        public void Dispose()
        {
        }
    }
}
