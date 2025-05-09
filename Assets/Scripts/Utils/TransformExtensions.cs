﻿using UnityEngine;

namespace Utils
{
    public static class TransformExtensions
    {
        public static void AdjustForPivot(this Transform transform, SpriteRenderer renderer)
        {
            var position = transform.position;
            var centerOffset = renderer.bounds.center - position;
            position -= centerOffset;
            transform.position = position;
        }

        public static void LookAt(this Transform objTransform, Vector2 lookAtPos)
        {
            var angleDeg = objTransform.GetAngleToObject(lookAtPos);
            objTransform.rotation = Quaternion.Euler(0,0,angleDeg);
        }
        
        public static float GetAngleToObject(this Transform objTransform, Vector2 lookAtPos, bool isRad = false)
        {
            var playerPos = objTransform.position;
            var x = lookAtPos.x - playerPos.x;
            var y = lookAtPos.y - playerPos.y;
            var angleRad = Mathf.Atan2(y, x);
            return isRad ? angleRad - Mathf.Deg2Rad * 90 : (180 / Mathf.PI) * angleRad - 90;
        }
        
        public static void SetPositionToMousePos(this Transform transform, Camera usedCamera)
        {
            var mousePosition = usedCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }

        public static bool InRange(this Transform current, Vector2 searched, float dist)
        {
            return current.Distance(searched) < dist;
        }
        
        public static Vector2 RandomPointInRange(this Transform transform, float range)
        {
            Vector2 center = transform.position;
            var offset = Random.insideUnitCircle * range;
            return center + offset;
        }
        
        public static Vector2 RandomPointInRange(this Transform transform, Vector2 center, float range)
        {
            var offset = Random.insideUnitCircle * range;
            return center + offset;
        }

        
        public static float Distance(this Transform current, Vector2 searched)
        {
            return (searched - (Vector2)current.transform.position).sqrMagnitude;
        }

        public static void MoveTowards(this Transform current, Vector2 target, float speed)
        {
            current.position = Vector2.MoveTowards(current.position, target, speed);
        }

        public static void MoveInDirection(this Transform current, Vector2 direction, float speed)
        {
            current.position += (Vector3)(direction * speed);
        }
    }
}