using UnityEngine;

namespace ayy
{
    public class AudioSampler : MonoBehaviour
    {
        public AudioSource _audioSource = null;
        public int _sampleSize = 1024;
        public float _threshold = 0.1f;

        private float[] _spectrumLeft;
        private float[] _spectrumRight;
        private float _prevEnergy;

        private CustomParticleSystem _particleSystem = null;
        
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            int channels = _audioSource.clip.channels;
            Debug.Log("audio channels:" + channels);
            _particleSystem = FindFirstObjectByType<CustomParticleSystem>();
            _spectrumLeft = new float[_sampleSize];
            _spectrumRight = new float[_sampleSize];
            _prevEnergy = 0;
        }
        
        void Update()
        {
            _audioSource.GetSpectrumData(_spectrumLeft, 0, FFTWindow.Hamming);
            _audioSource.GetSpectrumData(_spectrumRight, 1, FFTWindow.Hamming);
            
            float currentEnergy = CalculateSpectrumEnergy(_spectrumLeft,_spectrumRight);
            if (currentEnergy - _prevEnergy > _threshold)
            {
                OnBeat();
            }
            _prevEnergy = currentEnergy;
        }
        
        float CalculateSpectrumEnergy(float[] spectrumLeft, float[] spectrumRight)
        {
            float energy = 0f;
            for (int i = 0; i < spectrumLeft.Length; i++)
            {
                energy += spectrumLeft[i] * spectrumLeft[i];
            }
            for (int i = 0; i < spectrumRight.Length; i++)
            {
                energy += spectrumRight[i] * spectrumRight[i];
            }
            return energy;
        }

        private void OnBeat()
        {
            _particleSystem.EmitParticlesManually();
        }
    }

}
