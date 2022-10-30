using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Path from a parent to a descedant stored as child indices
    /// down into the hierarchy.
    /// </summary>
    [Serializable]
    public class TransformChildPath
    {
        private const bool IS_DEBUGGING = false;

        // Path of which child index it is of the current parent
        // starting from the upper most parent and working down.
        private readonly int[] m_path = new int[0];
        // If the path is valid. True unless the constructor finds
        // it to be false.
        private readonly bool m_isValid = true;

        public IReadOnlyList<int> path => m_path;
        public int[] rawPath => m_path;
        public bool isValid => m_isValid;


        public TransformChildPath()
        {
            m_path = new int[0];
        }
        public TransformChildPath(int[] path)
        {
            m_path = path;
        }
        /// <summary>
        /// Creates a path from the specified parent down to
        /// the specified descendant.
        ///
        /// Assumes that the descendant truly be a descendant of
        /// the parent in unity's hierarchy.
        /// </summary>
        public TransformChildPath(Transform parent, Transform descendant)
        {
            List<int> temp_pathList = new List<int>();

            // Work our way up from the descedant to the parent, holding
            // onto the sibling index each time.
            while (descendant != parent)
            {
                temp_pathList.Add(descendant.GetSiblingIndex());

                if (descendant.parent == null)
                {
                    Debug.LogError($"{descendant.name} did not have " +
                        $"{parent.name} as a parent, but " +
                        $"{nameof(TransformChildPath)} expected it to");
                    m_isValid = false;
                    return;
                }

                descendant = descendant.parent;
            }
            // List is backwards since it builds from bottom up
            temp_pathList.Reverse();
            m_path = temp_pathList.ToArray();
        }


        /// <summary>
        /// Traverses the given parent's child hierarchy with the current path.
        /// If the current path is empty, returns the parent.
        /// </summary>
        /// <param name="parent">Parent whose hierarchy to traverse.</param>
        /// <returns>Child who lies at the end of the specified path.</returns>
        public Transform Traverse(Transform parent)
        {
            Transform temp_curParent = parent;
            for (int i = 0; i < m_path.Length; ++i)
            {
                int temp_curSiblingIndex = m_path[i];
                CustomDebug.Log($"Attempting to traverse transform. " +
                    $"Going to {temp_curParent.name}'s {temp_curSiblingIndex}th " +
                    $"child", IS_DEBUGGING);
                Assert.IsTrue(temp_curParent.childCount > temp_curSiblingIndex,
                    $"Attempted to traverse a parent with an invalid path for " +
                    $"that parent. The path expected {temp_curParent.name} to " +
                    $"have at least {temp_curSiblingIndex + 1} children. Instead " +
                    $"it had {temp_curParent.childCount}.");
                temp_curParent = temp_curParent.GetChild(temp_curSiblingIndex);
            }
            return temp_curParent;
        }
        public bool Equals(TransformChildPath other)
        {
            if (other == null) { return false; }

            if (m_path == null && other.m_path != null) { return false; }
            if (m_path != null && other.m_path == null) { return false; }
            if (m_path == null && other.m_path == null) { return true; }

            if (m_path.Length != other.m_path.Length) { return false; }
            for (int i = 0; i < m_path.Length; ++i)
            {
                if (m_path[i] != other.m_path[i]) { return false; }
            }

            return true;
        }

        public override string ToString()
        {
            if (m_path.Length == 0) { return $"({nameof(TransformChildPath)}) " +
                    $"Empty Path"; }
            string temp_rtnString = $"({nameof(TransformChildPath)}) [";
            for (int i = 0; i < m_path.Length; ++i)
            {
                int temp_curSiblingIndex = m_path[i];
                temp_rtnString += "{" + temp_curSiblingIndex + "} -> ";
            }
            // Remove last arrow
            temp_rtnString = temp_rtnString.Substring(0, temp_rtnString.Length - 4);
            temp_rtnString += "]";

            return temp_rtnString;
        }
    }
}
