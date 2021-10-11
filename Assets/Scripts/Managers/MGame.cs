using Singleton;
using UniRx;
using UnityEngine;

namespace Managers
{
    public sealed class MGame : Singleton<MGame>
    {
        public readonly IntReactiveProperty OnBrushSize = new IntReactiveProperty();
        public readonly ReactiveProperty<float> OnBrushAlpha = new ReactiveProperty<float>();
        public readonly ReactiveCommand<Color> OnBrushColor = new ReactiveCommand<Color>();
    }
}