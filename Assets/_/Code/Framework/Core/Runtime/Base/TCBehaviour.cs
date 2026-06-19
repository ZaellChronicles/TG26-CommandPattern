using UnityEngine;

namespace Core.Runtime
{
    public abstract class TCBehaviour : MonoBehaviour
    {
        #region Unity API

        protected virtual void Awake()
        {
            _myTransform = transform;
            _myGameObject = gameObject;
        }

        #endregion


        #region Public API

        protected T TryGetComponentSafe<T>() where T : Component
        {
            TryGetComponent(out T component);
            Debug.Assert(component != null, $"[<b><color=#FF6B6B>{GetType().Name}</color></b>] Missing component {typeof(T).Name}", this);
            return component;
        }

        #endregion


        #region Main API

        protected void Log(string message) =>
            Debug.Log($"[<b><color=#57C7FF>{GetType().Name}</color></b>] {message}", this);

        protected void LogWarning(string message) =>
            Debug.LogWarning($"[<b><color=#FFD657>{GetType().Name}</color></b>] {message}", this);

        protected void LogError(string message) =>
            Debug.LogError($"[<b><color=#FF6B6B>{GetType().Name}</color></b>] {message}", this);

        #endregion


        #region Privates and Protecteds

        protected Transform _myTransform;
        protected GameObject _myGameObject;

        #endregion
    }
}