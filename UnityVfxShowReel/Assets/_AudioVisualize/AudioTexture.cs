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
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();                
            }
            else
            {
                _audioSource.UnPause();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _audioSource.Pause();
        }

        if (_audioSource.isPlaying)
        {
            _audioSource.GetSpectrumData(_fftBuffer, 0, _window);
            float min = Mathf.Infinity;
            float max = -1.0f;
            
            Color col = Color.black;
            for (int x = 0; x < _fftBuffer.Length; x++)
            {
                col.r = _fftBuffer[x];
                if (col.r < min)
                    min = col.r;
                if (col.r > max)
                    max = col.r;
                _audioTexture.SetPixel(x, 0, col);
                _audioTexture.SetPixel(x, 1, Color.black);
            }
            _audioTexture.Apply();
            Debug.Log(string.Format("!spectrum range:[{0:F3},{1:F3}]", min, max));
        }
    }

    private void OnDestroy()
    {
        Destroy(_audioTexture);
    }
}
