using System;
using UnityEngine;

public class AudioTexture : MonoBehaviour
{
    private AudioSource _audioSource = null;

    public FFTWindow _window = FFTWindow.Rectangular;

    private const int SAMPLE_COUNT = 512;
    private float[] _fftBuffer = new float[SAMPLE_COUNT];
    
    private Material _mat = null;
    private Texture2D _audioTexture = null;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _audioTexture = new Texture2D(SAMPLE_COUNT, 2);
        _audioTexture.filterMode = FilterMode.Point;
        _mat = GetComponent<MeshRenderer>().material;
        _mat.SetTexture(Shader.PropertyToID("_AudioTex"), _audioTexture);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _audioSource.Play();
        }
        _audioSource.GetSpectrumData(_fftBuffer, 0, _window);
        Color col = Color.black;
        for (int x = 0; x < _fftBuffer.Length; x++)
        {
            col.r = _fftBuffer[x];
            _audioTexture.SetPixel(x, 0, col);
            _audioTexture.SetPixel(x, 1, Color.black);
        }
        _audioTexture.Apply();
    }

    private void OnDestroy()
    {
        Destroy(_audioTexture);
    }
}
