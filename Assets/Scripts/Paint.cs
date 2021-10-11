using System.Diagnostics.CodeAnalysis;
using Enum;
using Managers;
using UniRx;
using UnityEngine;

[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
public sealed class Paint : MonoBehaviour
{
    private MInput _input;
    private MGame _game;
    
    [SerializeField] private PaintingType _paintingType;

    [SerializeField] private Texture2D _texture;
    [Range(2, 512), SerializeField] private int _resolution = 128;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private TextureWrapMode _textureWrapMode;
    [SerializeField] private Material _material;

    [SerializeField] private Camera _camera;
    [SerializeField] private Collider _collider;
    [SerializeField] private Color _color;

    private readonly ReactiveCommand<Vector3> OnDraw = new ReactiveCommand<Vector3>();

    private int _brushSize = 10;
    private int _oldRayX;
    private int _oldRayY;
    
    private void Awake()
    {
        _input = MInput.Instance;
        _game = MGame.Instance;
        
        _input.OnInputStart
            .Subscribe(point => OnDraw.Execute(point))
            .AddTo(this);

        _input.OnInputHold
            .Subscribe(point => OnDraw.Execute(point))
            .AddTo(this);

        _input.OnInputEnd
            .Subscribe(point => { })
            .AddTo(this);

        OnDraw
            .Subscribe(point =>
            {
                Ray ray = _camera.ScreenPointToRay(point);

                if (_collider.Raycast(ray, out RaycastHit hit, 100f))
                {
                    int rayX = (int)(hit.textureCoord.x * _resolution);
                    int rayY = (int)(hit.textureCoord.y * _resolution);

                    if (_oldRayX != rayX || _oldRayY != rayY)
                    {
                        switch (_paintingType)
                        {
                            case PaintingType.Quad:
                                DrawQuad(rayX, rayY);
                                break;
                            case PaintingType.Circle:
                                DrawCircle(rayX, rayY);
                                break;
                        }

                        _oldRayX = rayX;
                        _oldRayY = rayY;
                        
                        _texture.Apply();
                    }
                }
            })
            .AddTo(this);

        _game.OnBrushColor
            .Subscribe(color => _color = new Color(color.r, color.g, color.b, _color.a))
            .AddTo(this);

        _game.OnBrushAlpha
            .Subscribe(value => _color = new Color(_color.r, _color.g, _color.b, Remap(value, 0f, 255f, 0f, 1f)))
            .AddTo(this);

        Observable
            .EveryUpdate()
            .Subscribe(_ =>
            {
                _brushSize += (int)Input.mouseScrollDelta.y;

                if (_brushSize < 1) _brushSize = 1;
                if (_game.OnBrushSize.Value != _brushSize) _game.OnBrushSize.Value = _brushSize;
            })
            .AddTo(this);
    }

    private void Start()
    {
        _input.IsEnable.Value = true;
    }

    private void OnValidate()
    {
        if (!_texture)
        {
            _texture = new Texture2D(_resolution, _resolution);
        }

        if (_texture.width != _resolution)
        {
            _texture.Resize(_resolution, _resolution);
        }

        _texture.filterMode = _filterMode;
        _texture.wrapMode = _textureWrapMode;
        _material.mainTexture = _texture;
        
        _texture.Apply();
    }

    private void DrawQuad(int rayX, int rayY)
    {
        for (int y = 0; y < _brushSize; y++)
        {
            for (int x = 0; x < _brushSize; x++)
            {
                _texture.SetPixel(rayX + x - _brushSize / 2, rayY + y - _brushSize / 2, _color);
            }
        }
    }
    
    private void DrawCircle(int rayX, int rayY)
    {
        for (int y = 0; y < _brushSize; y++)
        {
            for (int x = 0; x < _brushSize; x++)
            {
                float x2 = Mathf.Pow(x - _brushSize / 2f, 2);
                float y2 = Mathf.Pow(y - _brushSize / 2f, 2);
                float r2 = Mathf.Pow(_brushSize / 2f - 0.5f, 2);

                if (x2 + y2 < r2)
                {
                    int pixelX = rayX + x - _brushSize / 2;
                    int pixelY = rayY + y - _brushSize / 2;

                    if (pixelX >= 0 && pixelX < _resolution && pixelY >= 0 && pixelY < _resolution)
                    {
                        Color oldColor = _texture.GetPixel(pixelX, pixelY);
                        Color lerpColor = Color.Lerp(oldColor, _color, _color.a);
                    
                        _texture.SetPixel(pixelX, pixelY, lerpColor);
                    }
                }
            }
        }
    }
    
    private static float Remap(float val, float from1, float to1, float from2, float to2)
    {
        return (val - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}