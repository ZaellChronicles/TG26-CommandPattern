using System.Collections.Generic;
using Core.Runtime;
using Path.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Player.Runtime
{
    public class CommandSequencer : TCBehaviour
    {
        #region Events

        public event Action OnDestinationFinalReached;

        #endregion

        #region Unity API

        protected override void Awake()
        {
            base.Awake();

            _player = TryGetComponentSafe<PlayerController>();
        }

        private void OnEnable()
        {
            _player.OnTeleportedToStart += ResetSequence;
            _player.OnDestinationReached += OnDestinationReached;
        }

        private void OnDisable()
        {
            _player.OnTeleportedToStart -= ResetSequence;
            _player.OnDestinationReached -= OnDestinationReached;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (!_player.IsReady) return;
            if (_commands.Count == 0)
            {
                _isPlaying = false;
                SetButtonsInteractable(true);
                return;
            }

            ICommand command = _commands[0];
            _commands.RemoveAt(0);
            RefreshText();
            command.Execute();
        }


        #endregion


        #region Public API

        public void AddMoveForward()
        {
            if (_isPlaying) return;
            _commands.Add(new MoveForwardCommand(_player, _pathGraph));
            RefreshText();
        }

        public void AddRotate(float angle)
        {
            if (_isPlaying) return;
            _commands.Add(new RotateCommand(_player, angle));
            RefreshText();
        }

        public void Undo()
        {
            if (_isPlaying) return;
            if ( _commands.Count == 0) return;
            _commands.RemoveAt(_commands.Count - 1);
            RefreshText();
        }

        public void Play()
        {
            if (_isPlaying) return;
            if (_commands.Count == 0) return;
            _isPlaying = true;
            SetButtonsInteractable(false);
        }

        #endregion


        #region Main API

        private void ResetSequence()
        {
            _commands.Clear();
            _isPlaying = false;
            SetButtonsInteractable(true);
            RefreshText();
        }

        private void OnDestinationReached()
        {
            Log("Victory !");
            OnDestinationFinalReached?.Invoke();
            ResetSequence();
            RefreshText();
        }

        private void SetButtonsInteractable(bool interactable)
        {
            foreach (Button button in _buttons) button.interactable = interactable;
        }

        private void RefreshText()
        {
            if (_commandListText == null) return;

            if (_commands.Count == 0)
            {
                _commandListText.text = string.Empty;
                return;
            }

            System.Text.StringBuilder sb = new();
            for (int i = 0; i < _commands.Count; i++)
                sb.AppendLine($"{_commands[i].Label}");
                // sb.AppendLine($"{i + 1}. {_commands[i].Label}");

            _commandListText.text = sb.ToString();
        }

        #endregion


        #region Privates and Protecteds

        [Header("- References -")]
        [SerializeField] private PathGraph _pathGraph;
        [SerializeField] private TMP_Text _commandListText;
        [SerializeField] private List<Button> _buttons;

        private PlayerController _player;
        private List<ICommand> _commands = new();
        private bool _isPlaying;

        #endregion
    }
}
