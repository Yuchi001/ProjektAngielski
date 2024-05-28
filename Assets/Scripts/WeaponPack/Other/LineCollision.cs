using System.Collections.Generic;
using UnityEngine;

namespace WeaponPack.Other
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class LineCollision : MonoBehaviour
    {
        private LineRenderer lr;
        private PolygonCollider2D polygonCollider2D;
        private List<Vector2> colliderPoints = new List<Vector2>();

        public PolygonCollider2D PolygonCollider2D => polygonCollider2D;

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        }


        public void UpdateCollisions()
        {
            colliderPoints = CalculateColliderPoints();
            polygonCollider2D.SetPath(0, colliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
        }

        private List<Vector2> CalculateColliderPoints() {
            //Get All positions on the line renderer
            var positions = GetPositions();

            //Get the Width of the Line
            var width = GetWidth();

            //m = (y2 - y1) / (x2 - x1)
            var m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
            var deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
            var deltaY = (width / 2f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

            //Calculate the Offset from each point to the collision vertex
            var offsets = new Vector3[2];
            offsets[0] = new Vector3(-deltaX, deltaY);
            offsets[1] = new Vector3(deltaX, -deltaY);

            //Generate the Colliders Vertices
            var colliderPositions = new List<Vector2> {
                positions[0] + offsets[0],
                positions[1] + offsets[0],
                positions[1] + offsets[1],
                positions[0] + offsets[1]
            };

            return colliderPositions;
        }

        private Vector3[] GetPositions() {
            var positions = new Vector3[lr.positionCount];
            lr.GetPositions(positions);
            return positions;
        }

        private float GetWidth() {
            return lr.startWidth;
        }
    }
}
