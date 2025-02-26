using UnityEngine;


namespace ayy
{
    public class CSTest : MonoBehaviour
    {
        public RenderTexture _rt = null;
        public ComputeShader _computeShader = null;
        public GameObject _showImage = null;
        
        private int _width = 8;
        private int _height = 8;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CreateRenderTexture();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoCompute();
            }
        }
        
        private void CreateRenderTexture()
        {
            // create render texture
            _rt = new RenderTexture(_width,_height, 0);
            _rt.enableRandomWrite = true;
            _rt.Create();
            
            // assign texture to material
            var material = _showImage.GetComponent<MeshRenderer>().material;
            material.SetTexture(Shader.PropertyToID("_MainTex"), _rt);
        }
        
        private void DoCompute()
        {
            int kernel = _computeShader.FindKernel("CSMain");
            _computeShader.SetTexture(kernel, Shader.PropertyToID("Result"), _rt);
            _computeShader.Dispatch(kernel, _width / 8, _height / 8, 1);
        }
    }
}
