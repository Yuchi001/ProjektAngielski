using System.Collections.Generic;
using System.Linq;
using EnemyPack;
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

        public static float GetAngleToObject(Transform objTransform, Vector2 lookAtPos, bool isRad = false)
        {
            var playerPos = objTransform.position;
            var x = lookAtPos.x - playerPos.x;
            var y = lookAtPos.y - playerPos.y;
            var angleRad = Mathf.Atan2(y, x);
            return isRad ? angleRad - Mathf.Deg2Rad * 90 : (180 / Mathf.PI) * angleRad - 90;
        }

        public static T RandomClamp<T>(T val1, T val2)
        {
            var random = Random.Range(0, 2);
            return random == 0 ? val1 : val2;
        }
        
        public static EnemyLogic FindTarget(Vector2 position, List<int> usedTargets = null)
        {
            usedTargets ??= new List<int>();
            
            var enemies = Object.FindObjectsOfType<EnemyLogic>().ToList();
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

            return pickedEnemy;
        }
        
        public static string StringJoin(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }
        
        public static EnemyLogic FindTarget(List<string> usedTargets = null)
        {
            usedTargets = usedTargets ?? new List<string>();
            
            var enemies = Object.FindObjectsOfType<EnemyLogic>().ToList();
            enemies = enemies.Where(e => !usedTargets.Contains(e.name)).ToList();
            if (enemies.Count == 0) return null;

            var randomIndex = Random.Range(0, enemies.Count);
            
            return enemies[randomIndex];
        }
    }
}