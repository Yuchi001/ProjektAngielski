using System;
using System.Collections;
using EnemyPack;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class KillCountUi : MonoBehaviour
    {
        [SerializeField] private float uiUpdateRate = 3;
        [SerializeField] private TextMeshProUGUI killCountField;

        private float _timer = 0;
        private EnemySpawner _enemySpawner = null;

        private IEnumerator Start()
        {
            killCountField.text = "x0";
            yield return new WaitUntil(() => FindObjectOfType<EnemySpawner>() != null);

            _enemySpawner = FindObjectOfType<EnemySpawner>();
        }

        private void Update()
        {
            if (_enemySpawner == null) return;
            
            _timer += Time.deltaTime;
            if (_timer < 1 / uiUpdateRate) return;

            _timer = 0;
            killCountField.text = $"x{_enemySpawner.DeadEnemies}";
        }
    }
}