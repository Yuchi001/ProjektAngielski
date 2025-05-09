﻿using System;
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
        [SerializeField] private Transform cameraFollow;
        [SerializeField] private Camera cameraComponent;

        private Transform _follow;
        
        private static MainCamera Instance { get; set; }
        public static bool HasInstance() => Instance != null;

        private void Awake()
        {
            if (gameObject.scene.buildIndex != (int)GameManager.EScene.MAIN)
            {
                Destroy(gameObject);
                return;
            }

            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            
            _inOutImg.color = Color.black;
            PlayerHealth.OnPlayerDamaged += ShakeCamera;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= ShakeCamera;
        }

        private void ShakeCamera(int _, int __)
        {
            _cameraShaker.ShakeOnce(10, 10, 0.1f, 0.1f);
        }

        public static void InOutAnim(float animTime, Action halfAction, Action endAction)
        {
            Instance.StartCoroutine(Instance.CoroutineInOutAnim(animTime, halfAction, endAction));
        }
        
        public static void InOutAnim(float animTime, Action halfAction, Action endAction, IEnumerator condition)
        {
            Instance.StartCoroutine(Instance.CoroutineInOutAnim(animTime, halfAction, endAction, condition));
        }
        
        public static void OutAnim(float animTime, Action endAction = null)
        {
            Instance.StartCoroutine(Instance.HalfAnim(animTime, false, endAction));
        }
        
        public static void InAnim(float animTime, Action endAction = null)
        {
            Instance.StartCoroutine(Instance.HalfAnim(animTime, true, endAction));
        }

        private IEnumerator CoroutineInOutAnim(float animTime, Action halfAction, Action endAction, IEnumerator condition = null)
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

            if (condition != null) yield return condition;
            else yield return new WaitForSeconds(0.1f);

            i = 0f;
            while (i < halfTime)
            {
                i += Time.deltaTime;
                _inOutImg.color = Color.Lerp(Color.black, Color.clear, i / halfTime);
                yield return new WaitForEndOfFrame();
            }
            endAction.Invoke();
        }

        private IEnumerator HalfAnim(float animTime, bool fadeIn, Action endAction = null)
        {
            var i = 0f;
            while (i < animTime)
            {
                i += Time.deltaTime;
                _inOutImg.color = fadeIn ? 
                    Color.Lerp(Color.black, Color.clear, i / animTime) : 
                    Color.Lerp(Color.clear, Color.black, i / animTime);
                yield return new WaitForEndOfFrame();
            }
            endAction?.Invoke();
        }

        public static void SetFollow(Transform follow)
        {
            if (Instance._follow == null) Instance.StartCoroutine(Instance.HalfAnim(0.3f, true));
            Instance._follow = follow;
        }

        public static void SetSize(float size)
        {
            Instance.cameraComponent.orthographicSize = size;
        }

        public static void SetColor(Color color)
        {
            Instance.cameraComponent.backgroundColor = color;
        }

        private void Update()
        {
            if (_follow == null) cameraFollow.position = new Vector3(0, 0, -10);
            else cameraFollow.position = new Vector3(_follow.position.x, _follow.position.y, -10);
        }

    }
}