using UnityEngine;

namespace AI
{
    public class SphereMovement : MonoBehaviour
    {
        public Transform[] points;  // Array of points to move between
        public float speed = 1.0f;  // Speed of movement

        private int _currentPointIndex = 0;  // Index of the current point in the array
        private Transform _targetPoint;  // The current target point

        private void Start()
        {
            if (points.Length > 0)
            {
                _targetPoint = points[_currentPointIndex];  // Set the initial target point
            }
        }

        private void Update()
        {
            if (!_targetPoint) return;

            // Move towards the target point
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint.position, speed * Time.deltaTime);

            // Check if the object has reached the target point
            if (Vector3.Distance(transform.position, _targetPoint.position) < 0.1f)
            {
                // Move to the next point in the array
                _currentPointIndex = (_currentPointIndex + 1) % points.Length;
                _targetPoint = points[_currentPointIndex];
            }
        }
    }
}
