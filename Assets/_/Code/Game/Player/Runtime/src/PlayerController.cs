using Core.Runtime;
using Path.Runtime;
using System;
using UnityEngine;

namespace Player.Runtime
{
    public class PlayerController : TCBehaviour
    {
        #region Publics

        public Checkpoint CurrentCheckpoint => _currentCheckpoint;
        public bool IsReady => _state == State.Idle;

        #endregion


        #region Events

        public event Action OnTeleportedToStart;
        public event Action OnDestinationReached;
        public event Action OnDestinationLeft;

        #endregion


        #region Unity API

        protected override void Awake()
        {
            base.Awake();

            TeleportToStart();
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Moving:
                    UpdateMoving();
                    break;
                case State.Rotating:
                    UpdateRotating();
                    break;
                case State.Pausing:
                    UpdatePausing();
                    break;
            }
        }

        #endregion


        #region Public API

        public void MoveTo(Checkpoint target)
        {
            _targetCheckpoint = target;
            _state = State.Moving;
        }

        public void RotateTo(float angle)
        {
            _targetRotation = Quaternion.Euler(0f, angle, 0f);
            _state = State.Rotating;
        }

        public void TeleportToStart()
        {
            _currentCheckpoint = _startCheckpoint;
            _myTransform.position = _startCheckpoint.transform.position;
            _myTransform.rotation = Quaternion.identity;
            _state = State.Idle;
            OnTeleportedToStart?.Invoke();
            OnDestinationLeft?.Invoke();
        }

        #endregion


        #region Main API

        private void UpdateMoving()
        {
            OnDestinationLeft?.Invoke();

            Vector3 targetPosition = _targetCheckpoint.transform.position;
            _myTransform.position = Vector3.MoveTowards(_myTransform.position, targetPosition, _moveSpeed * Time.deltaTime);

            if (_myTransform.position != targetPosition) return;

            _currentCheckpoint = _targetCheckpoint;

            if (_currentCheckpoint.IsDestination)
            {
                OnDestinationReached?.Invoke();
                _state = State.Idle;
                return;
            }

            StartPause();
        }

        private void UpdateRotating()
        {
            _myTransform.rotation = Quaternion.RotateTowards(_myTransform.rotation, _targetRotation, _rotateSpeed * Time.deltaTime);

            if (_myTransform.rotation != _targetRotation) return;

            _myTransform.rotation = _targetRotation;

            StartPause();
        }

        private void UpdatePausing()
        {
            _pauseTimer -= Time.deltaTime;

            if (_pauseTimer > 0f) return;

            _state = State.Idle;
        }

        private void StartPause()
        {
            _pauseTimer = _pauseDuration;
            _state = State.Pausing;
        }

        #endregion


        #region Privates and Protecteds

        [Header("- References -")]
        [SerializeField] private Checkpoint _startCheckpoint;

        [Header("Settings -")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 180f;
        [SerializeField] private float _pauseDuration = 0.5f;

        private Checkpoint _currentCheckpoint;
        private Checkpoint _targetCheckpoint;
        private Quaternion _targetRotation;
        private float _pauseTimer;

        private enum State { Idle, Moving, Rotating, Pausing }
        private State _state = State.Idle;

        #endregion
    }
}