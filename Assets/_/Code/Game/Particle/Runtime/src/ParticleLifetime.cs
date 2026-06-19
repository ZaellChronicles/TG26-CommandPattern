using Core.Runtime;
using UnityEngine;

namespace Particle.Runtime
{
    public class ParticleLifetime : TCBehaviour
    {
        #region Unity API

        protected override void Awake()
        {
            base.Awake();
            _initialScale = _myTransform.localScale;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _lifeTime - _shrinkDuration)
                UpdateShrink();

            if (_elapsedTime >= _lifeTime)
                Destroy(_myGameObject);
        }

        #endregion


        #region Main API

        private void UpdateShrink()
        {
            float t = (_elapsedTime - (_lifeTime - _shrinkDuration)) / _shrinkDuration;
            _myTransform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);
        }

        #endregion


        #region Privates and Protecteds

        [SerializeField] private float _lifeTime = 2f;
        [SerializeField] private float _shrinkDuration = 0.5f;

        private Vector3 _initialScale;
        private float _elapsedTime;

        #endregion
    }
}