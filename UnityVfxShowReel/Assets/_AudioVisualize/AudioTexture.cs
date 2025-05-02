using System;
using UnityEngine;

public class AudioTexture : MonoBehaviour
{
    public AudioSource _audioSource = null;
    public FFTWindow _window = FFTWindow.Rectangular;

    public enum EQuality
    {
        Low = 128,
        Mid = 512,
        High = 1024
    }

    public EQuality _sampleQuality = EQuality.Mid;
    
    private int _sampleCount = 512;
    private float[] _fftBuffer = null;
    
    private Material _mat = null;
    private Texture2D _audioTexture = null;
    
    void Start()
    {
        _sampleCount = (int)_sampleQuality;
        _fftBuffer = new float[_sampleCount];
        _audioTexture = new Texture2D(_sampleCount, 2);
        _audioTexture.filterMode = FilterMode.Point;
        _mat = GetComponent<MeshRenderer>().material;
        _mat.SetTexture(Shader.PropertyToID("_AudioTex"), _audioTexture);
        _mat.SetFloat(Shader.PropertyToID("_SampleCount"), _sampleCount);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _audioSource.Play();
        }

        if (_audioSource.isPlaying)
        {
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
    }

    private void OnDestroy()
    {
        Destroy(_audioTexture);
    }
}
