using UnityEngine;

namespace ayy
{
    public class MicrophoneInput : MonoBehaviour
    {
        public AudioSource audioSource;
        public string microphoneName;

        void Start()
        {
            Debug.Log("MicrophoneInput Start()");
            audioSource = GetComponent<AudioSource>();
            // 检查是否有可用的麦克风
            if (Microphone.devices.Length > 0)
            {
                // 如果没有指定麦克风名称，则使用第一个可用的麦克风
                if (string.IsNullOrEmpty(microphoneName))
                {
                    microphoneName = Microphone.devices[0];
                }

                // 开始从麦克风录制音频
                audioSource.clip = Microphone.Start(microphoneName, true, 3500, AudioSettings.outputSampleRate);

                // 等待直到麦克风开始录制
                while (!(Microphone.GetPosition(microphoneName) > 0)) { }

                // 播放录制的音频
                audioSource.Play();
            }
            else
            {
                Debug.Log("没有可用的麦克风设备。");
            }
        }

        void OnDestroy()
        {
            Debug.Log("MicrophoneInput End() 1");
            // 停止录制并释放麦克风资源
            if (Microphone.IsRecording(microphoneName))
            {
                Debug.Log("MicrophoneInput End() 2");
                Microphone.End(microphoneName);
            }
        }
    }    
}

