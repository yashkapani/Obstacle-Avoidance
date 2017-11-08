using UnityEngine;
using System.Collections;

namespace AISandbox {
    public class ObstacleAvoidance : MonoBehaviour {

        public SimpleActor _avoiding_actor;

        private void Start() {
            _avoiding_actor.Velocity = new Vector2(10.0f, 0.0f);
        }

        private void FixedUpdate() {
            if (_avoiding_actor.transform.position.x >= 36)
                _avoiding_actor.transform.position = new Vector3(-36.0f, _avoiding_actor.transform.position.y, 0.0f);
            else if (_avoiding_actor.transform.position.x <= -36)
                _avoiding_actor.transform.position = new Vector3(36.0f, _avoiding_actor.transform.position.y, 0.0f);

            if (_avoiding_actor.transform.position.y >= 20.5)
                _avoiding_actor.transform.position = new Vector3(_avoiding_actor.transform.position.x, -20.5f, 0.0f);
            else if (_avoiding_actor.transform.position.y <= -20.5)
                _avoiding_actor.transform.position = new Vector3(_avoiding_actor.transform.position.x, 20.5f, 0.0f);
        }
    }
}