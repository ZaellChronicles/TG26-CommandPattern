using UnityEngine;

namespace Player.Runtime
{
    public class RotateCommand : ICommand
    {
        #region Publics

        public string Label => _angle > 0 ? "Rotate Right" : "Rotate Left";

        public RotateCommand(PlayerController player, float angle)
        {
            _player = player;
            _angle = angle;
        }

        #endregion


        #region Public API

        public void Execute()
        {
            float targetAngle = _player.transform.eulerAngles.y + _angle;
            _player.RotateTo(targetAngle);
        }

        #endregion


        #region Privates and Protected

        private PlayerController _player;
        private float _angle;

        #endregion
    }
}