using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Common
{
    public static class ComponentExtenstions
    {
        public static T GetOrAdd<T>(this Component component) where T : Component
        {
            var got = component.GetComponent<T>();
            return got ? got : component.gameObject.AddComponent<T>();
        }
    }
}
