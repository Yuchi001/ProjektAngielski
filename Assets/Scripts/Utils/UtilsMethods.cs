using System.Collections.Generic;
using System.Linq;
using AccessorPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using PoolPack;
using UnityEngine;

namespace Utils
{
    public static class UtilsMethods
    {
        public static Vector2 GetMousePosition()
        {
            var mouseRawPos = Input.mousePosition;
            return Camera.main.ScreenToWorldPoint(mouseRawPos);
        }

        public static void LookAtMouse(Transform objTransform)
        {
            var angleDeg = GetAngleToMouse(objTransform);
            objTransform.rotation = Quaternion.Euler(0,0,angleDeg);
        }

        public static void LookAtObj(Transform objTransform, Vector2 lookAtPos)
        {
            var angleDeg = GetAngleToObject(objTransform, lookAtPos);
            objTransform.rotation = Quaternion.Euler(0,0,angleDeg);
        }

        public static float GetAngleToMouse(Transform objTransform, bool isRad = false)
        {
            var mousePos = GetMousePosition();
            var playerPos = objTransform.position;
            var x = mousePos.x - playerPos.x;
            var y = mousePos.y - playerPos.y;
            var angleRad = Mathf.Atan2(y, x);
            return isRad ? angleRad - Mathf.Deg2Rad * 90 : (180 / Mathf.PI) * angleRad - 90;
        }

        public static void SetPositionToMousePos(this Transform transform, Camera usedCamera)
        {
            var mousePosition = usedCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }

        public static T BinaryRandom<T>(T first, T second)
        {
            return Random.Range(0, 2) == 0 ? first : second;
        }

        public static bool InRange(this Transform current, Vector2 searched, float dist)
        {
            return Vector2.Distance(current.position, searched) < dist;
        }
        
        public static float Distance(this Transform current, Vector2 searched)
        {
            return Vector2.Distance(current.position, searched);
        }

        public static void MoveTowards(this Transform current, Vector2 target, float speed)
        {
            current.position = Vector2.MoveTowards(current.position, target, speed * Time.deltaTime);
        }

        public static float GetAngleToObject(Transform objTransform, Vector2 lookAtPos, bool isRad = false)
        {
            var playerPos = objTransform.position;
            var x = lookAtPos.x - playerPos.x;
            var y = lookAtPos.y - playerPos.y;
            var angleRad = Mathf.Atan2(y, x);
            return isRad ? angleRad - Mathf.Deg2Rad * 90 : (180 / Mathf.PI) * angleRad - 90;
        }

        public static EnemyLogic FindNearestTarget(Vector2 position, List<int> usedTargets = null)
        {
            usedTargets ??= new List<int>();
            
            var enemies = MainSceneAccessor.EnemySpawner.GetActiveEnemies();
            enemies = enemies.Where(e => !usedTargets.Contains(e.GetInstanceID())).ToList();
            if (enemies.Count == 0) return null;

            var (pickedEnemy, smallestDistance) = (enemies[0], Vector2.Distance(position, enemies[0].transform.position));
            foreach (var enemy in enemies)
            {
                var distance = Vector2.Distance(position, enemy.transform.position);
                if (distance > smallestDistance) continue;

                pickedEnemy = enemy;
                smallestDistance = distance;
            }

            return pickedEnemy as EnemyLogic;
        }
        
        public static EnemyLogic FindFurthestTarget(Vector2 position, List<int> usedTargets = null)
        {
            usedTargets ??= new List<int>();
            
            var enemies = MainSceneAccessor.EnemySpawner.GetActiveEnemies();
            enemies = enemies.Where(e => !usedTargets.Contains(e.GetInstanceID())).ToList();
            if (enemies.Count == 0) return null;

            var (pickedEnemy, farthestDistance) = (enemies[0], Vector2.Distance(position, enemies[0].transform.position));
            foreach (var enemy in enemies)
            {
                var distance = Vector2.Distance(position, enemy.transform.position);
                if (distance < farthestDistance) continue;

                pickedEnemy = enemy;
                farthestDistance = distance;
            }

            return pickedEnemy as EnemyLogic;
        }
        
        public static EnemyLogic FindTargetInBiggestGroup(Vector2 position, float? xRange = null, float? yRange = null)
        {
            var enemies = MainSceneAccessor.EnemySpawner.GetActiveEnemies();
            if (enemies.Count == 0) return null;

            var left = new List<PoolObject>();
            var right = new List<PoolObject>();
            foreach (var enemy in enemies)
            {
                var pos = enemy.transform.position;
                if(!InRange(pos)) continue;
                var isLeft = pos.x < position.x;
                if (isLeft) left.Add(enemy);
                else right.Add(enemy);
            }

            var biggestGroup = left.Count > right.Count ? left : right;
            
            var (pickedEnemy, smallestDistance) = (enemies[0], Vector2.Distance(position, enemies[0].transform.position));
            foreach (var enemy in biggestGroup)
            {
                var distance = Vector2.Distance(position, enemy.transform.position);
                if (distance > smallestDistance) continue;

                pickedEnemy = enemy;
                smallestDistance = distance;
            }

            return pickedEnemy as EnemyLogic;

            bool InRange(Vector2 enemyPos)
            {
                var inRangeX = !xRange.HasValue ||
                               (enemyPos.x < position.x + xRange && enemyPos.x > position.x - xRange);
                var inRangeY = !yRange.HasValue ||
                               (enemyPos.y < position.y + yRange && enemyPos.y > position.y - yRange);
                return inRangeX && inRangeY;
            }
        }

        public static EnemyLogic FindRandomTarget(List<int> usedTargets = null)
        {
            usedTargets ??= new List<int>();
            
            var enemies = MainSceneAccessor.EnemySpawner.GetActiveEnemies();
            enemies = enemies.Where(e => !usedTargets.Contains(e.GetInstanceID())).ToList();
            if (enemies.Count == 0) return null;

            var randomIndex = Random.Range(0, enemies.Count);
            
            return enemies[randomIndex] as EnemyLogic;
        }
    }
}