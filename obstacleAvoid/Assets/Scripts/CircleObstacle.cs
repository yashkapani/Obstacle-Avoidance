using UnityEngine;
using System.Collections;

namespace AISandbox {
    [RequireComponent(typeof (SpriteRenderer))]
    public class CircleObstacle : MonoBehaviour {
        private readonly Color COLLIDE_COLOR = Color.red;

        private SpriteRenderer _renderer;
        private Color _orig_color;
        private float _radius;

        private void Start() {
            _renderer = GetComponent<SpriteRenderer>();
            _orig_color = _renderer.color;
            _radius = _renderer.bounds.extents.x;
        }

        public float Radius {
            get { return _radius; }
        }

        public Vector2 Center {
            get { return (Vector2)transform.position; }
        }

        private void FixedUpdate() {
            bool collision = false;
            SimpleActor[] actors = FindObjectsOfType<SimpleActor>();
            foreach( SimpleActor actor in actors ) {
                float sqr_dist = (actor.transform.position - transform.position).sqrMagnitude;
                float actor_radius = actor.gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
                float sqr_min_dist = (_radius+actor_radius) * (_radius+actor_radius);
                if( sqr_min_dist > sqr_dist) {
                    collision = true;
                    break;
                }
            }

            _renderer.color = collision ? COLLIDE_COLOR : _orig_color;
        }
    }
}
