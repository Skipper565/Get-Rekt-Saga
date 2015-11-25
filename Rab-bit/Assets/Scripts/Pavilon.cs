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
        public abstract void LoadInitialPosition(GameObject barrier);
        public abstract void ApplyInitialPosition(GameObject barrier);
        public abstract float GetBarrierLength();
        public abstract string GetPavilonName();
        public abstract List<GameObject> GetBarriers();
    }
}
