using UnityEngine;
using System.Collections;

namespace AISandbox {
    [RequireComponent(typeof (IActor))]
    public class AvoidController : MonoBehaviour {
        private IActor _actor;
        private CircleObstacle[] circleObjs;

        private Vector2 _lateral_velocity = Vector2.zero;
        private Vector2 _break_velocity = Vector2.zero;

		private void Awake() {
            _actor = GetComponent<IActor>();
        }

        private void Start() {
            circleObjs = FindObjectsOfType<CircleObstacle>();
        }

        private void FixedUpdate() {
            AvoidObstacle();

            Vector2 steering_velocity = _lateral_velocity + _break_velocity;

            // Pass all parameters to the character control script.
            _actor.SetInput( steering_velocity.x, steering_velocity.y );
        }

        void AvoidObstacle() {
            int obstaclesCount = 0;
            int obstacleToAvoid = 0;
            Vector2[] closestPoints = new Vector2[10];
            CircleObstacle[] closestObstacles = new CircleObstacle[10];

            foreach(CircleObstacle circleObj in circleObjs) {
                Vector2 closestPoint;
                if(TestCircleOBB(circleObj, _actor.Box, out closestPoint)) {
                    closestPoints[obstaclesCount] = closestPoint;
                    closestObstacles[obstaclesCount] = circleObj;
                    obstaclesCount++;
                }
            }

            //Debug.Log(obstaclesCount);

            for(int i = 0; i < obstaclesCount; i++) {
                Vector2 toClosest = closestPoints[obstacleToAvoid] - _actor.Center;
                Vector2 toClose = closestPoints[i] - _actor.Center;

                if(toClose.sqrMagnitude > toClosest.sqrMagnitude) {
                    obstacleToAvoid = i;
                }
            }

            if (obstaclesCount != 0) {
                _lateral_velocity = -Vector3.Project( (closestObstacles[obstacleToAvoid].Center - _actor.Center), _actor.Box._local_axes[1] );

                _break_velocity = -Vector3.Project((closestPoints[obstacleToAvoid] - _actor.Center), _actor.Box._local_axes[0]);
                _break_velocity = (_actor.Box._extents[0] * 2 - _break_velocity.magnitude) * _break_velocity.normalized;
            }
            else {
                _lateral_velocity = Vector2.zero;
                _break_velocity = Vector2.zero;
            }

        }

        // Given point p, return point q on (or in) OBB b, closest to p
        Vector2 ClosestPtPointOBB(Vector2 point, SimpleActor.OBB box)
        {
            Vector2 d = point - box._center;
            // Start result at center of box; make steps from there
            Vector2 closestOnBox = box._center;
            // For each OBB axis...
            for (int i = 0; i < 2; i++)
            {
                // ...project d onto that axis to get the distance
                // along the axis of d from the box center
                float dist = (Vector3.Project(d, box._local_axes[i])).magnitude;
                // If distance farther than the box extents, clamp to the box
                if (dist > box._extents[i])
                    dist = box._extents[i];
                if (dist < -box._extents[i])
                    dist = -box._extents[i];
                // Step that distance along the axis to get world coordinate
                closestOnBox += dist * box._local_axes[i];
            }

            return closestOnBox;
        }

        //Returns true if sphere s intersects OBB b, false otherwise.
        //The point p on the OBB closest to the sphere center is also returned
        bool TestCircleOBB(CircleObstacle obstacle, SimpleActor.OBB box, out Vector2 closestPoint)
        {
            // Find point p on OBB closest to sphere center
            closestPoint = ClosestPtPointOBB(obstacle.Center, box);

            // Sphere and OBB intersect if the (squared) distance from sphere
            // center to point p is less than the (squared) sphere radius
            Vector2 v = closestPoint - obstacle.Center;
            return v.sqrMagnitude <= obstacle.Radius * obstacle.Radius;
        }
    }
}