using DG.Tweening;
using Runtime.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class RollSystem : ISystem
    {
        public World World { get; set; }
        private Entity _uiCanvasEntity;

        private int _totalChars;
        private int _currentIndex = int.MaxValue;
        private float _totalTime;
        private Button _rollButton;
        private SystemsGroup _systemsGroup;
        private AnimationStarSystem _animationStarSystem;
        private Sequence _mySequence;
        private RectTransform[] _chars;
        private RectTransform _arrow;

        public void OnAwake()
        {
            _uiCanvasEntity = World.Filter.With<UICanvasComponent>().Build().First();
            ref var uiCanvasComponent = ref _uiCanvasEntity.GetComponent<UICanvasComponent>();
            _rollButton = uiCanvasComponent.RollButton;
            _rollButton.onClick.AddListener(ClickButton);
            _chars = uiCanvasComponent.Chars;
            _arrow = uiCanvasComponent.Arrow;

            var generalGameDataComponent = World.Filter.With<GeneralGameDataComponent>().Build().First();
            ref var generalGameData = ref generalGameDataComponent.GetComponent<GeneralGameDataComponent>();
            _totalChars = generalGameData.GetLengthChars();

            _mySequence = DOTween.Sequence();
        }

        private void ClickButton()
        {
            _animationStarSystem = new AnimationStarSystem();
            _systemsGroup = World.CreateSystemsGroup();
            _systemsGroup.AddSystem(_animationStarSystem);

            _currentIndex = 0;
            _totalTime = 0;

            OnEnableAnimation(_currentIndex);
            _rollButton.enabled = false;
            _rollButton.onClick.RemoveListener(ClickButton);
        }


        public void OnUpdate(float deltaTime)
        {
            if (_systemsGroup == null)
                return;

            _systemsGroup.Update(deltaTime);
            _totalTime += deltaTime;

            if (_totalTime >= 0.5f && _currentIndex < _totalChars - 1 && _animationStarSystem.GetNumParticlesAlive() == 0)
            {
                _currentIndex++;
                OnEnableAnimation(_currentIndex);
                _totalTime = 0f;
            }
        }

        private void OnEnableAnimation(int index)
        {
            _mySequence.Kill();
            _mySequence = DOTween.Sequence();

            var anglePerChar = 360f / _totalChars;
            var targetAngle = index * anglePerChar;
            _arrow.DORotate(new Vector3(0, 0, -targetAngle), .25f).SetEase(Ease.OutBack);
            _mySequence.Append(_chars[index].DOScale(Vector3.one * 1.3f, 0.15f).SetEase(Ease.OutCubic));
            _mySequence.AppendInterval(0.125f);
            _mySequence.Append(_chars[index].DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutCubic));


            _mySequence.OnComplete(() => { _animationStarSystem.SetLetter(index); });
        }


        public void Dispose()
        {
            _mySequence = null;
            _systemsGroup = null;
            _animationStarSystem = null;
        }
    }
}