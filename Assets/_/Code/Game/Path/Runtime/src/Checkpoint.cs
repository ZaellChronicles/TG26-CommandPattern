using Core.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Path.Runtime
{
    public class Checkpoint : TCBehaviour
    {
        #region Publics

        public List<Checkpoint> Neighbours => _neighbours;
        public bool IsDestination => _isDestination;

        #endregion


        #region Privates and Protecteds

        [Header("- Settings -")]
        [SerializeField] private List<Checkpoint> _neighbours = new();
        [SerializeField] private bool _isDestination;

        #endregion
    }
}