using System.Collections.Generic;
using Core.Runtime;
using Path.Runtime;
using UnityEngine;

namespace Player.Runtime
{
    public class CommandSequencer : TCBehaviour
    {
        #region Unity API

        protected override void Awake()
        {
            base.Awake();

            _player = TryGetComponentSafe<PlayerController>();
        }

        private void OnEnable()
        {
            _player.OnTeleportedToStart += ResetSequence;
            _player.OnDestinationReached += ResetSequence;
        }

        private void OnDisable()
        {
            _player.OnTeleportedToStart -= ResetSequence;
            _player.OnDestinationReached -= ResetSequence;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (!_player.IsReady) return;
            if (_commands.Count == 0)
            {
                _isPlaying = false;
                return;
            }

            ICommand command = _commands[0];
            _commands.RemoveAt(0);
            command.Execute();
        }


        #endregion


        #region Public API

        public void AddMoveForward()
        {
            if (_isPlaying) return;
            _commands.Add(new MoveForwardCommand(_player, _pathGraph));
        }

        public void AddRotate(float angle)
        {
            if (_isPlaying) return;
            _commands.Add(new RotateCommand(_player, angle));
        }

        public void Play()
        {
            if (_isPlaying) return;
            if (_commands.Count == 0) return;
            _isPlaying = true;
        }

        #endregion


        #region Main API

        private void ResetSequence()
        {
            _commands.Clear();
            _isPlaying = false;
        }

        #endregion


        #region Privates and Protecteds

        [Header("- References -")]
        [SerializeField] private PathGraph _pathGraph;

        private PlayerController _player;
        private List<ICommand> _commands = new();
        private bool _isPlaying;

        #endregion
    }
}
