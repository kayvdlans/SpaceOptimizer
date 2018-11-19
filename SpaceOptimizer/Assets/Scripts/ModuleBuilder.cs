using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
