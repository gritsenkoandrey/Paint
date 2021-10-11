using Managers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class UIBrushSizeView : MonoBehaviour
    {
        [SerializeField] private Text _brushSize;

        private MGame _game;

        private void Awake()
        {
            _game = MGame.Instance;

            _game.OnBrushSize
                .Subscribe(value => _brushSize.text = value.ToString())
                .AddTo(this);
        }
    }
}