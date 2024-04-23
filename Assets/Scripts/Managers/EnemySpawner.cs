using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using EnemyPack.SO;
using PlayerPack;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private int maxEnemiesCount = 100;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnRate;
        [SerializeField] private Camera mainCamera;

        private float _timer = 0;
        private float _spawnRangeX;
        private float _spawnRangeY;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();
        
        private void Start()
        {
            var bottomLeftCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            var topRightCorner = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            var cameraWidthInUnits = Mathf.Abs(topRightCorner.x - bottomLeftCorner.x);
            var cameraHeightInUnits = Mathf.Abs(topRightCorner.y - bottomLeftCorner.y);

            _spawnRangeX = cameraWidthInUnits / 2 + 1;
            _spawnRangeY = cameraHeightInUnits / 2 + 1;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < 1 / spawnRate || PlayerManager == null) return;

            if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemiesCount) return;
            
            _timer = 0;

            // true => x
            // false => y
            var randomDimension = UtilsMethods.RandomClamp(true, false);
            
            var xDiff = randomDimension ? 
                Random.Range(-_spawnRangeX, _spawnRangeX) : 
                UtilsMethods.RandomClamp(-_spawnRangeX, _spawnRangeX);
            
            var yDiff = !randomDimension ? 
                Random.Range(-_spawnRangeY, _spawnRangeY) : 
                UtilsMethods.RandomClamp(-_spawnRangeY, _spawnRangeY);
            
            var spawnPos = PlayerManager.transform.position;
            spawnPos.x += xDiff;
            spawnPos.y += yDiff;
            
            var enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            var enemyScript = enemyObj.GetComponent<EnemyLogic>();
            
            enemyScript.Setup(GetEnemy(), PlayerManager.transform);
        }

        private SoEnemy GetEnemy()
        {
            // todo: pick enemy logic

            return _allEnemies[Random.Range(0, _allEnemies.Count)];
        }
    }
}