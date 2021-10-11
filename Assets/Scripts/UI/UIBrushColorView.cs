using Managers;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class UIBrushColorView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        private MGame _game;

        private void Awake()
        {
            _game = MGame.Instance;

            _button
                .OnClickAsObservable()
                .Subscribe(_ => _game.OnBrushColor.Execute(_image.color))
                .AddTo(this);
        }
    }
}