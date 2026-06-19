using Core.Runtime;
using UnityEngine;

namespace Path.Runtime
{
    public class PathGraph : TCBehaviour
    {
        #region Public API

        public Checkpoint GetNeighbourInDirection(Checkpoint from, Vector3 direction)
        {
            Vector3 dir = direction.normalized;

            foreach (Checkpoint neighbour in from.Neighbours)
            {
                Vector3 toNeighbour = (neighbour.transform.position - from.transform.position).normalized;

                if (toNeighbour == dir) return neighbour;
            }

            return null;
        }

        #endregion
    }
}