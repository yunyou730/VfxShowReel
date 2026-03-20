using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public class ShellFurMono : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh = null;
        [SerializeField] private Material _material = null;
        [SerializeField, Range(1, 32)] private int _shellCount = 16;
        [SerializeField, Range(0, 3)] private float _furLength = 0.3f;

        public Material GetMaterial() => _material;
        public Mesh GetMesh() => _mesh;
        public int GetShellCount() => _shellCount;
        public float GetFurLength() => _furLength;

        // 存储每一个 GPU Instance 每个 instance 的 TRS(translation,rotation,scale) Matrix
        public static int kMaxShellCnt = 32;
        private Matrix4x4[] _matrics = null;
        public Matrix4x4[] GetMetrics() => _matrics;

        // 缓存 transform 信息, 避免每一帧都更新
        private Vector3 _pos = Vector3.zero;
        private Quaternion _rot = Quaternion.Euler(0, 0, 0);
        private Vector3 _scale = Vector3.one;

        // static singleton
        private static List<ShellFurMono> s_furMonoList = null;

        public static List<ShellFurMono> GetInstanceList()
        {
            return s_furMonoList;
        }

        private void OnEnable()
        {
            if (s_furMonoList == null)
            {
                s_furMonoList = new List<ShellFurMono>();
            }

            if (!s_furMonoList.Contains(this))
            {
                s_furMonoList.Add(this);
            }
        }

        private void OnDisable()
        {
            s_furMonoList.Remove(this);
        }

        void Awake()
        {
            _matrics = new Matrix4x4[kMaxShellCnt];
            for (int i = 0; i < kMaxShellCnt; i++)
            {
                _matrics[i] = Matrix4x4.TRS(_pos, _rot, _scale);
            }
        }

        void Update()
        {
            UpdateShellMetrics(false);
        }

        private void OnValidate()
        {
            UpdateShellMetrics(true);
        }

        private void UpdateShellMetrics(bool forceRefresh)
        {
            if (_matrics == null || _matrics.Length == 0)
            {
                return;
            }

            bool transformDirty = _pos != transform.position || _rot != transform.localRotation ||
                                  _scale != transform.localScale;
            if (forceRefresh || transformDirty)
            {
                _pos = transform.position;
                _rot = transform.localRotation;
                _scale = transform.localScale;

                Matrix4x4 furMatrix = Matrix4x4.TRS(_pos, _rot, _scale);
                for (int index = 0; index < _shellCount; index++)
                {
                    _matrics[index] = furMatrix;
                }
            }
        }
    }
}
