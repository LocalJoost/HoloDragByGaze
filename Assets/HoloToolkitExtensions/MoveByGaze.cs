using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

namespace LocalJoost.HoloToolkitExtensions
{
    public class MoveByGaze : MonoBehaviour
    {
        public float MaxDistance = 2f;
        public bool IsActive = true;
        public float DistanceTrigger = 0.2f;
        public BaseRayStabilizer Stabilizer = null;
        public BaseSpatialMappingCollisionDetector CollisonDetector;

        private float _startTime;
        private float _delay = 0.5f;
        private bool _isJustEnabled;
        private Vector3 _lastMoveToLocation;
        private bool _isBusy;

        private SpatialMappingManager MappingManager
        {
            get { return SpatialMappingManager.Instance; }
        }

        void OnEnable()
        {
            _isJustEnabled = true;
        }

        void Start()
        {
            _startTime = Time.time + _delay;
            _isJustEnabled = true;
            if (CollisonDetector == null)
            {
                CollisonDetector = gameObject.AddComponent<DefaultMappingCollisionDetector>();
            }
        }

        void Update()
        {
            if (!IsActive || _isBusy || _startTime > Time.time)
                return;
            _isBusy = true;

            var newPos = GetPostionInLookingDirection();
            if ((newPos - _lastMoveToLocation).magnitude > DistanceTrigger || _isJustEnabled)
            {
                _isJustEnabled = false;
                var maxDelta = CollisonDetector.GetMaxDelta(newPos - transform.position);
                if (maxDelta != Vector3.zero)
                {
                    newPos = transform.position + maxDelta;
                    iTween.MoveTo(gameObject,
                        iTween.Hash("position", newPos, "time", 2.0f * maxDelta.magnitude,
                            "easetype", iTween.EaseType.easeInOutSine, "islocal", false,
                            "oncomplete", "MovingDone", "oncompletetarget", gameObject));
                    _lastMoveToLocation = newPos;
                }
                else
                {
                    _isBusy = false;
                }
            }
            else
            {
                _isBusy = false;
            }
        }

        private void MovingDone()
        {
            _isBusy = false;
        }


        private Vector3 GetPostionInLookingDirection()
        {
            RaycastHit hitInfo;

            var headReady = Stabilizer != null
                ? Stabilizer.StableRay
                : new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (MappingManager != null &&
                Physics.Raycast(headReady, out hitInfo, MaxDistance, MappingManager.LayerMask))
            {
                return hitInfo.point;
            }

            return CalculatePositionDeadAhead(MaxDistance);
        }

        private Vector3 CalculatePositionDeadAhead(float distance)
        {
            return Stabilizer != null
                ? Stabilizer.StableRay.origin + Stabilizer.StableRay.direction.normalized * distance
                : Camera.main.transform.position + Camera.main.transform.forward.normalized * distance;
        }
    }
}