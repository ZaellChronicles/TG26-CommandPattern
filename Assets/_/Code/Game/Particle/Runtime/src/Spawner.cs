using Core.Runtime;
using Player.Runtime;
using UnityEngine;

namespace Particle.Runtime
{
    public class Spawner : TCBehaviour
    {
        #region Unity API

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(_particle != null, "[Spawner] _particle n'a pas de GameObject !", this);
            Debug.Assert(_commandSequencer != null, "[Spawner] CommandSequencer n'est pas référencé !", this);
            Debug.Assert(_playerController != null, "[Spawner] PlayerController n'est pas référencé !", this);
        }

        private void OnEnable()
        {
            _commandSequencer.OnDestinationFinalReached += VictoryHandle;
            _playerController.OnDestinationLeft += LeftDestinationHandle;
        }

        private void OnDisable()
        {
            _commandSequencer.OnDestinationFinalReached -= VictoryHandle;
            _playerController.OnDestinationLeft -= LeftDestinationHandle;
        }

        private void Update()
        {
            HipHipHipHoura();
        }

        #endregion


        #region Main API

        private void HipHipHipHoura()
        {
            if (!_isActive) return;

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime <= _timeBetweenParticles) return;

            _elapsedTime = 0f;

            GameObject particle = Instantiate(
                _particle,
                transform.position + Random.insideUnitSphere * _unitCircleMultipliers,
                Quaternion.identity,
                transform
            );
        }

        private void VictoryHandle() => _isActive = true;

        private void LeftDestinationHandle() => _isActive = false;

        #endregion


        #region Privates and Protecteds

        [Header("- References -")]
        [SerializeField] private CommandSequencer _commandSequencer;
        [SerializeField] private PlayerController _playerController;

        [Header("- Settings -")]
        [SerializeField] private GameObject _particle;
        [SerializeField, Range(0.01f, 0.5f)] private float _timeBetweenParticles;
        [SerializeField, Range(0f, 2.5f)] private float _unitCircleMultipliers;

        private bool _isActive = false;
        private float _elapsedTime;

        #endregion
    }
}