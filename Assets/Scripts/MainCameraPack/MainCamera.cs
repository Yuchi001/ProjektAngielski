using System;
using System.Collections;
using Managers;
using PlayerPack;
using UnityEngine;
using UnityEngine.UI;

namespace MainCameraPack
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private CameraShaker _cameraShaker;
        [SerializeField] private Image _inOutImg;
        private Transform _player;
        
        private static MainCamera Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            PlayerHealth.OnPlayerDamaged += ShakeCamera;
            GameManager.OnStartRun += Setup;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= ShakeCamera;
            GameManager.OnStartRun -= Setup;
        }

        public void Setup()
        {
            _player = PlayerManager.Instance.transform;
        }

        private void ShakeCamera(int _, int __)
        {
            _cameraShaker.ShakeOnce(10, 10, 0.1f, 0.1f);
        }

        public static void InOutAnim(float animTime, Action halfAction, Action endAction)
        {
            Instance.StartCoroutine(Instance.CoroutineInOutAnim(animTime, halfAction, endAction));
        }

        private IEnumerator CoroutineInOutAnim(float animTime, Action halfAction, Action endAction)
        {
            var i = 0f;
            var halfTime = animTime / 2;
            while (i < halfTime)
            {
                i += Time.deltaTime;
                _inOutImg.color = Color.Lerp(Color.clear, Color.black, i / halfTime);
                yield return new WaitForEndOfFrame();
            }
            halfAction.Invoke();

            yield return new WaitForSeconds(0.1f);

            i = 0f;
            while (i < halfTime)
            {
                i += Time.deltaTime;
                _inOutImg.color = Color.Lerp(Color.black, Color.clear, i / halfTime);
                yield return new WaitForEndOfFrame();
            }
            endAction.Invoke();
        }

        private void Update()
        {
            if (_player == null) return;

            transform.position = new Vector3(_player.position.x, _player.position.y, -10);
        }
    }
}