using System.Diagnostics.CodeAnalysis;
using Enum;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
public sealed class Texture : MonoBehaviour
{
    [SerializeField] private TexturePaintType _paintType;
    [SerializeField] private Material _material;
    
    [SerializeField] private Texture2D _texture;
    [Range(2, 512), SerializeField] private int _resolution = 128;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private TextureWrapMode _textureWrapMode;
    [Range(0f, 1f), SerializeField] private float _radiusOut = 0.5f;
    [Range(0f, 1f), SerializeField] private float _radiusIn = 0.3f;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Gradient _gradient;

    private void Start()
    {
        _texture = new Texture2D(_resolution, _resolution);
        _material.mainTexture = _texture;
    }

    private void OnValidate()
    {
        // if (!_texture)
        // {
        //     _texture = new Texture2D(_resolution, _resolution);
        //     _material.mainTexture = _texture;
        // }

        if (_texture.width != _resolution)
        {
            _texture.Resize(_resolution, _resolution);
        }

        _texture.filterMode = _filterMode;
        _texture.wrapMode = _textureWrapMode;

        float step = 1f / _resolution;

        for (int y = 0; y < _resolution; y++)
        {
            for (int x = 0; x < _resolution; x++)
            {
                switch (_paintType)
                {
                    case TexturePaintType.RandomValue:
                        _texture.SetPixel(x, y, SetRandomColor());
                        break;
                    case TexturePaintType.GradientValue:
                        _texture.SetPixel(x, y, SetGradient(x, y, step));
                        break;
                    case TexturePaintType.PaintGrid:
                        if (x % 5 == 0 || y % 5 == 0) _texture.SetPixel(x, y, Color.black);
                        else _texture.SetPixel(x, y, Color.cyan);
                        break;
                    case TexturePaintType.PaintShadow:
                        _texture.SetPixel(x, y, Color.Lerp(Color.black, new Color(0, 0, 0, 0), Inverse(x, y, step)));
                        break;
                    case TexturePaintType.GradientEvaluate:
                        _texture.SetPixel(x, y, _gradient.Evaluate(Inverse(x, y, step)));
                        break;
                    case TexturePaintType.PaintCircle:
                        _texture.SetPixel(x, y, IsOutOrIn(x, y, step)
                                ? Color.Lerp(Color.blue, Color.yellow, Inverse(x, y, step))
                                : Color.white);
                        break;
                }
            }
        }
        _texture.Apply();
    }

    private static Color SetRandomColor()
    {
        float random = Random.value;
        return new Color(random, random, random, 1f);
    }

    private static Color SetGradient(float x, float y, float step)
    {
        return new Color((x + 0.5f) * step, (y + 0.5f) * step, 0f, 1f);
    }

    private float Inverse(float x, float y, float step)
    {
        float x2 = Mathf.Pow((x + 0.5f) * step - _offset.x, 2);
        float y2 = Mathf.Pow((y + 0.5f) * step - _offset.y, 2);
        float rOut2 = Mathf.Pow(_radiusOut, 2);
        float rIn2 = Mathf.Pow(_radiusIn, 2);
        float result = x2 + y2;

        return Mathf.InverseLerp(rIn2, rOut2, result);
    }

    private bool IsOutOrIn(float x, float y, float step)
    {
        float x2 = Mathf.Pow((x + 0.5f) * step - _offset.x, 2);
        float y2 = Mathf.Pow((y + 0.5f) * step - _offset.y, 2);
        float rOut2 = Mathf.Pow(_radiusOut, 2);
        float rIn2 = Mathf.Pow(_radiusIn, 2);
        float result = x2 + y2;

        return result < rOut2 && result > rIn2;
    }
}
