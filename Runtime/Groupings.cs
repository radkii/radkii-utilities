using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Radkii.Groupings
{
    public class TypeGrouping
    {
        private object parentObject;
        private object[] childObjects;
        public Type groupingType;

        public TypeGrouping(GameObject self, string typeName)
        {
            Component m = null;
            void FindType(Transform transform)
            {
                foreach (Transform t in transform)
                {
                    m = t.gameObject.GetComponent(typeName);
                    if (m != null) return;
                    FindType(t);
                }
            }
            FindType(self.transform);

            groupingType = m.GetType();

            Component[] children = self.GetComponentsInChildren(m.GetType());
            childObjects = children;
            Component parent = self.GetComponent(m.GetType());
            parentObject = parent;

            //print(string.Join(" - ", new List<object>(childObjects).ConvertAll(obj => (obj as Component).gameObject.name)));
        }

        public T GetParent<T>() => (T)parentObject;
        public T GetChild<T>(int index) => (T)(childObjects[index]);
        public T[] GetAllChildren<T>() => new List<object>(childObjects).ConvertAll(obj => (T)obj).ToArray();
    }

    public static class TypeGroupingExtensions 
    {
        public static TypeGrouping FindGrouping<T>(this TypeGrouping[] groupings) => Array.Find<TypeGrouping>(groupings, u => typeof(T) == u.groupingType);

        public static void HandleParentComponent<T>(this TypeGrouping grouping, Action<T> handler) => handler?.Invoke(grouping.GetParent<T>());
        public static void HandleChildComponent<T>(this TypeGrouping grouping, Action<T> handler, int index) => handler?.Invoke(grouping.GetChild<T>(index));
        public static void HandleChildrenComponents<T>(this TypeGrouping grouping, Action<T> handler)
        {
            foreach (T child in grouping.GetAllChildren<T>()) handler.Invoke(child);
        }
    }
}
