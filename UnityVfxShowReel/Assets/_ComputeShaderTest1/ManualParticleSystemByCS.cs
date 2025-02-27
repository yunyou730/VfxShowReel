using UnityEngine;

namespace ayy
{

    public class ManualParticleSystemByCS : MonoBehaviour
    {
        public int _particleCount = 1000;
        private ParticleSystem _particleSystem;
        
        void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Emit();
            }
        }

        private void Emit()
        {
            _particleSystem.Emit(_particleCount);
        }
    }
    
}
