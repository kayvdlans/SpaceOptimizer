using System.Collections.Generic;
using UnityEngine;

namespace SpaceOptimizer.Modules
{
    public class ModuleBuilder : MonoBehaviour
    {
        [SerializeField]
        private List<BuildModule> _BuildModules = new List<BuildModule>();
        public List<BuildModule> BuildModules
        {
            get
            {
                return _BuildModules;
            }
        }
    }
}
