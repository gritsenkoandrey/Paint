using UnityEngine;

public class Texture : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [Range(2, 512), SerializeField] private int _resolution = 128;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private TextureWrapMode _textureWrapMode;

    private void OnValidate()
    {
        if (!_texture)
        {
            _texture = new Texture2D(_resolution, _resolution);
            GetComponent<Renderer>().material.mainTexture = _texture;
        }

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
                //_texture.SetPixel(x, y, new Color32(128, 128, 0, 255));
                // float randomValue = Random.value;
                // _texture.SetPixel(x, y, new Color(randomValue, randomValue, randomValue, 1f));
                
                //Gradient
                //_texture.SetPixel(x, y, new Color((x + 0.5f) * step, (y + 0.5f) * step, 0f, 1f));

                if (x % 5 == 0 || y % 5 == 0)
                    _texture.SetPixel(x, y, Color.black);
                else
                    _texture.SetPixel(x, y, Color.white);
            }
        }
        
        _texture.Apply();
    }
}
