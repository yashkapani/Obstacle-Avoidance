using UnityEngine;
using System.Collections;

namespace AISandbox {
    public class SimpleActor : MonoBehaviour, IActor {
        private const float MAX_SPEED           = 40.0f;
        private const float STEERING_ACCEL      = 100.0f;
        private const float VELOCITY_LINE_SCALE = 0.5f;
        private const float STEERING_LINE_SCALE = 4.0f;
        private const float EXTENTS_VEL_SCALE   = 10.0f;

        [SerializeField]
        private bool _DrawVectors = true;
        public bool DrawVectors {
            get {
                return _DrawVectors;
            }
            set {
                _DrawVectors = value;
                _steering_line.gameObject.SetActive(_DrawVectors);
                _velocity_line.gameObject.SetActive(_DrawVectors);
                _OBB_Obj.gameObject.SetActive(_DrawVectors);
            }
        }
        public LineRenderer _steering_line;
        public LineRenderer _velocity_line;

        private Vector2 _steering = Vector2.zero;
        private Vector2 _acceleration = Vector2.zero;
        private Vector2 _velocity = Vector2.zero;

        public struct OBB {
            public Vector2 _center;
            public Vector2[] _local_axes;
            public float[] _extents;
        }

        public OBB Box { get { return _actor_OBB; } }

        public Vector2 Center { get { return transform.position; } }

        public OBB _actor_OBB;
        public GameObject _OBB_Obj;

        private void Start() {
            DrawVectors = _DrawVectors;
            _actor_OBB._local_axes = new Vector2[2];
            _actor_OBB._extents = new float[2];
        }
   
        private void UpdateOBB() {
            _actor_OBB._local_axes[0] = _velocity.normalized;
            _actor_OBB._local_axes[1] = new Vector2(-_actor_OBB._local_axes[0].y, _actor_OBB._local_axes[0].x);
            _actor_OBB._extents[0] = _velocity.magnitude / 2;
            _actor_OBB._extents[1] = 0.64f;
            _actor_OBB._center = (Vector2)transform.position + _actor_OBB._local_axes[0] * _actor_OBB._extents[0];
          
            float rotaionAmt = 0.0f;
            if(_velocity.sqrMagnitude != 0)
                rotaionAmt = Vector2.Angle(_velocity, _OBB_Obj.transform.right);

            Debug.Log(rotaionAmt);

            _OBB_Obj.transform.Rotate(Vector3.forward, rotaionAmt <= 180? rotaionAmt : 360 - rotaionAmt);
            _OBB_Obj.transform.localScale = new Vector3(_actor_OBB._extents[0] * 2 / EXTENTS_VEL_SCALE, _actor_OBB._extents[1] * 2, 1.0f);
        }

        public void SetInput( float x_axis, float y_axis ) {
            _steering = Vector2.ClampMagnitude( new Vector2(x_axis, y_axis), 1.0f );
            _acceleration = _steering * STEERING_ACCEL;
        }

        public float MaxSpeed {
            get { return MAX_SPEED; }
        }

        public Vector2 Velocity {
            set { _velocity = value; }
            get { return _velocity; }
        }

        private void FixedUpdate() {
            _velocity += _acceleration * Time.fixedDeltaTime;
            _velocity = Vector2.ClampMagnitude(_velocity, MAX_SPEED);
            Vector3 position = transform.position;
            position += (Vector3)(_velocity * Time.fixedDeltaTime);
            transform.position = position;

            UpdateOBB();
        }

        private void Update() {
            _steering_line.SetPosition( 1, _steering * STEERING_LINE_SCALE );
            _velocity_line.SetPosition( 1, _velocity * VELOCITY_LINE_SCALE );

            // The steering is reset every frame so SetInput() must be called every frame for continuous steering.
            _steering = Vector2.zero;
            _acceleration = Vector2.zero;
        }
    }
}