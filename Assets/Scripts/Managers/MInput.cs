using Singleton;
using UniRx;
using UnityEngine;

namespace Managers
{
    public sealed class MInput : Singleton<MInput>
    {
        public readonly BoolReactiveProperty IsEnable = new BoolReactiveProperty();
        
        public readonly ReactiveProperty<Vector3> OnInputStart = new ReactiveProperty<Vector3>();
        public readonly ReactiveProperty<Vector3> OnInputHold = new ReactiveProperty<Vector3>();
        public readonly ReactiveProperty<Vector3> OnInputEnd = new ReactiveProperty<Vector3>();

        private readonly CompositeDisposable _inputDisposable = new CompositeDisposable();

        private void OnEnable()
        {
            IsEnable
                .Where(value => value)
                .Subscribe(_ =>
                {
                    Observable
                        .EveryUpdate()
                        .Subscribe(_ => UpdateInput())
                        .AddTo(_inputDisposable);
                })
                .AddTo(this);

            IsEnable
                .Where(value => !value)
                .Subscribe(_ => _inputDisposable.Clear())
                .AddTo(this);
        }

        private void OnDisable()
        {
            _inputDisposable.Clear();
        }

        private void UpdateInput()
        {
        
#if UNITY_EDITOR
        
            Vector3 mousePosition = Input.mousePosition;
        
            if (Input.GetMouseButtonDown(0))
            {
                InputStart(mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                InputHold(mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                InputEnd(mousePosition);
            }
#endif

#if UNITY_ANDROID

            Touch touch = new Touch();
            Vector3 touchPosition = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    InputStart(touchPosition);
                    break;
                case TouchPhase.Moved:
                    InputHold(touchPosition);
                    break;
                case TouchPhase.Stationary:
                    InputHold(touchPosition);
                    break;
                case TouchPhase.Ended:
                    InputEnd(touchPosition);
                    break;
                case TouchPhase.Canceled:
                    InputEnd(touchPosition);
                    break;
            }
#endif
        }

        private void InputStart(Vector3 input)
        {
            OnInputStart.Value = input;
        }
    
        private void InputHold(Vector3 input)
        {
            OnInputHold.Value = input;
        }
    
        private void InputEnd(Vector3 input)
        {
            OnInputEnd.Value = input;
        }
    }
}