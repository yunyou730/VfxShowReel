using System.Diagnostics;
using UnityEngine;
using UnityEngine.Polybrush;
using Debug = UnityEngine.Debug;


namespace ayy
{
    public class CSTest : MonoBehaviour
    {
        // Generate render texture image
        public RenderTexture _rt = null;
        public ComputeShader _computeShader = null;
        public GameObject _showImage = null;
        private int _width = 8;
        private int _height = 8;
        
        // Process image
        public Texture2D _originTexture = null;
        public ComputeShader _csImageProcess = null;
        
        
        // Simple compute
        public ComputeShader _csSimpleCompute = null;
        private ComputeBuffer _inputBuffer = null;
        private ComputeBuffer _outputBuffer = null;
        private float[] _inputData;
        private float[] _outputData;
        private const int kDataSize = 65535;
        
        // stopwatch
        Stopwatch stopwatch = new Stopwatch();
        

        void Start()
        {
            _width = 16;
            _height = 16;

            InitSimpleCompute();
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
            else if (Input.GetKeyDown(KeyCode.A))
            {
                ComputeWithCS();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ComputeWithCommon();
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
            
            int kernel = _csImageProcess.FindKernel("CSMain");
            _csImageProcess.SetTexture(kernel,Shader.PropertyToID("InputTex"), _originTexture);
            _csImageProcess.SetTexture(kernel,Shader.PropertyToID("Result"), rt);
            _csImageProcess.Dispatch(kernel,width/8 ,height/8,1);
        }


        private void InitSimpleCompute()
        {
            _inputData = new float[kDataSize];
            for (int i = 0;i < kDataSize;i++)
            {
                _inputData[i] = i;
            }
            
            _inputBuffer = new ComputeBuffer(kDataSize, sizeof(float));
            _inputBuffer.SetData(_inputData);

            
            _outputData = new float[kDataSize];
            _outputBuffer = new ComputeBuffer(kDataSize, sizeof(float)); 
        }

        private void ComputeWithCS()
        {
            Debug.Log("ComputeWithCS"); 
            stopwatch.Restart();
            
            int kernel = _csSimpleCompute.FindKernel("CSMain");
            _csSimpleCompute.SetBuffer(kernel,Shader.PropertyToID("inputBuffer"), _inputBuffer);
            _csSimpleCompute.SetBuffer(kernel,Shader.PropertyToID("outputBuffer"), _outputBuffer);
            
            int threadGroups = Mathf.CeilToInt((float)kDataSize / 64);;
            _csSimpleCompute.Dispatch(kernel,threadGroups, 1, 1);
            _outputBuffer.GetData(_outputData);

            Debug.Log("done with cs, cost time:" + stopwatch.Elapsed);
        }

        private void ComputeWithCommon()
        {
            Debug.Log("ComputeWithCommon"); 
            stopwatch.Restart();
            
            for (int i = 0;i < kDataSize;i++)
            {
                _outputData[i] = _inputData[i] * 2.0f;
            }            
            Debug.Log("done with common, cost time:" + stopwatch.Elapsed);
        }
    }
}
