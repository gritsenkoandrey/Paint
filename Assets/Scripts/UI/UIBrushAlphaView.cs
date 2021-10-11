using Managers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class UIBrushAlphaView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private MGame _game;

        private void Awake()
        {
            _game = MGame.Instance;

            _slider.ObserveEveryValueChanged(slider => slider.value)
                .Subscribe(value => _game.OnBrushAlpha.Value = value)
                .AddTo(this);
        }
    }
}