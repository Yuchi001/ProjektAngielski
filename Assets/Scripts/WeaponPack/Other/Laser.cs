using System.Collections;
using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using UnityEngine;

namespace WeaponPack.Other
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;

        private EnemyLogic _target;

        private float _animTime = 0.3f;
        private float _maxScale = 1;
        private float _duration = 0;

        #region Setup methods

        public Laser Setup()
        {
            lineRenderer.SetPosition(0, PlayerPos);
            
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
            
            return this;
        }

        public Laser SetAnimTime(float animTime)
        {
            _animTime = animTime;
            return this;
        }

        public Laser SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }

        public Laser SetTarget(EnemyLogic target)
        {
            _target = target;
            return this;
        }

        public Laser SetMaxScale(float scale)
        {
            _maxScale = scale;
            return this;
        }

        public void Ready()
        {
            LeanTween.value(gameObject, 0, _maxScale, _animTime)
                .setEaseOutCubic()
                .setOnUpdate((value) =>
                {
                    lineRenderer.startWidth = value;
                    lineRenderer.endWidth = value;
                }).setOnComplete(() =>
                {
                    StartCoroutine(HandleLaserDamage(lineRenderer));
                });
        }

        #endregion
        
        
        private IEnumerator HandleLaserDamage(LineRenderer lineRenderer)
        {
            yield return new WaitForSeconds(_duration);
            LeanTween.value(gameObject, _maxScale, 0, _animTime)
                .setEaseOutCubic()
                .setOnUpdate((value) =>
                {
                    lineRenderer.startWidth = value;
                    lineRenderer.endWidth = value;
                }).setOnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}