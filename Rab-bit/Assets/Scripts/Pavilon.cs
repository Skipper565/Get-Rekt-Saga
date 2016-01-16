using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Pavilon
    {
        // Each barrier generator child must implement these methods

        /// <summary>
        /// Saves initial position of each animated element within the barrier
        /// </summary>
        /// <param name="barrier">Barrier = one prefab section</param>
        public abstract void LoadInitialPosition(GameObject barrier);

        /// <summary>
        /// Applies saved position of each animated element within the barrier
        /// </summary>
        /// <param name="barrier">Barrier = one prefab section</param>
        public abstract void ApplyInitialPosition(GameObject barrier);

        /// <summary>
        /// Get length of the widest child of the barrier. This child is picked manually.
        /// If there is another way of easily determining the length of the barrier, it might be used instead.
        /// </summary>
        /// <returns>Length of the barrier</returns>
        public abstract float GetBarrierLength();

        /// <summary>
        /// For debug purposes.
        /// </summary>
        /// <returns>Name of the pavilon</returns>
        public abstract string GetPavilonName();

        /// <summary>
        /// Getter for all barriers in the pavilon.
        /// </summary>
        /// <returns>List of barriers (prefabs) from the pavilon.</returns>
        public abstract List<GameObject> GetBarriers();
    }
}
