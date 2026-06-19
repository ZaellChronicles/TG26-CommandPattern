using Path.Runtime;
using UnityEngine;

namespace Player.Runtime
{
    public class MoveForwardCommand : ICommand
    {
        #region Publics

        public MoveForwardCommand(PlayerController player, PathGraph pathGraph)
        {
            _player = player;
            _pathGraph = pathGraph;
        }

        #endregion


        #region Public API

        public void Execute()
        {
            Checkpoint next = _pathGraph.GetNeighbourInDirection(_player.CurrentCheckpoint, _player.transform.forward);

            if (next == null)
            {
                _player.TeleportToStart();
                return;
            }

            _player.MoveTo(next);
        }

        #endregion


        #region Privates and Protecteds

        private PlayerController _player;
        private PathGraph _pathGraph;

        #endregion
    }
}