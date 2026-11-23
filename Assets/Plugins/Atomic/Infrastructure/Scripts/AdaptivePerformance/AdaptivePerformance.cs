using UnityEngine;

namespace Atomic.Infrastructure
{
    [DefaultExecutionOrder(-10000)]
    public sealed class AdaptivePerformance : MonoBehaviour
    {
        private static AdaptivePerformance _instance;

        public static AdaptivePerformance Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                GameObject go = new GameObject(nameof(AdaptivePerformance));
                _instance = go.AddComponent<AdaptivePerformance>();

                DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.HideInHierarchy;

                return _instance;
            }
        }

        [SerializeField, Range(0.01f, 1f)]
        private float _smoothing = 0.1f;

        private readonly FrameTiming[] _timings = new FrameTiming[1];

        private float _cpuTime;
        private bool _hasSample;

        public float CPUTime => _cpuTime;
        public bool HasSample => _hasSample;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private void Update()
        {
            FrameTimingManager.CaptureFrameTimings();

            uint count = FrameTimingManager.GetLatestTimings(1, _timings);
            if (count == 0)
                return;

            FrameTiming frameTiming = _timings[0];
            float cpuTime = (float) frameTiming.cpuMainThreadFrameTime;

            if (cpuTime <= 0f)
                return;

            if (!_hasSample)
            {
                _cpuTime = cpuTime;
                _hasSample = true;
                return;
            }

            _cpuTime = Mathf.Lerp(_cpuTime, cpuTime, _smoothing);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}