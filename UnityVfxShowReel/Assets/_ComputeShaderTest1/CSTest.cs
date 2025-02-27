using UnityEngine;


namespace ayy
{
    public class CSTest : MonoBehaviour
    {
        public RenderTexture _rt = null;
        public ComputeShader _computeShader = null;
        public GameObject _showImage = null;
        
        public Texture2D _originTexture = null;
        public ComputeShader _csImageProcess = null;
        
        
        private int _width = 8;
        private int _height = 8;

        void Start()
        {
            _width = 16;
            _height = 16;
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CreateRenderTexture();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                DoCompute();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                DoCompute2();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ProcessImageWithComputeShader();
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
        
        // 正确 ,原因已经清楚了, 待整理
        private void DoCompute()
        {
            int kernel = _computeShader.FindKernel("CSMain");
            _computeShader.SetTexture(kernel, Shader.PropertyToID("Result"), _rt);
            _computeShader.Dispatch(kernel, _width / 8, _height / 8, 1);
        }
        
        // 错误示范. 解释错误原因需要画图, 按照 ThreadGroup 和 Thread 的对应关系就能看懂了 
        private void DoCompute2()
        {
            int kernel = _computeShader.FindKernel("CSMain");
            _computeShader.SetTexture(kernel, Shader.PropertyToID("Result"), _rt);
            _computeShader.Dispatch(kernel,  _width/8 * _height/8,1, 1);
        }

        private void ProcessImageWithComputeShader()
        {
            int width = _originTexture.width;
            int height = _originTexture.height;
            
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.enableRandomWrite = true;
            var material = _showImage.GetComponent<MeshRenderer>().material;
            material.SetTexture(Shader.PropertyToID("_MainTex"), rt);
            
            //material.SetTexture(Shader.PropertyToID("_MainTex"), _originTexture);
            int kernel = _csImageProcess.FindKernel("CSMain");
            _csImageProcess.SetTexture(kernel,Shader.PropertyToID("InputTex"), _originTexture);
            _csImageProcess.SetTexture(kernel,Shader.PropertyToID("Result"), rt);
            _csImageProcess.Dispatch(kernel,width/8 ,height/8,1);
        }
    }
}
