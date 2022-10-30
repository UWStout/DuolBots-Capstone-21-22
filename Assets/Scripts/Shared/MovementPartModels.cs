using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuolBots
{
    /// <summary>
    /// Helper for getting moving this movement parts various models
    /// to the correct positions.
    /// </summary>
    public class MovementPartModels : MonoBehaviour
    {
        // Reference to the models for this movement part
        [SerializeField] private List<Transform> m_modelTransformList = new List<Transform>();


        /// <summary>
        /// Moves the models for this movement part to the given parent transforms
        /// and resets their local transforms to match the parent's specifications.
        ///
        /// Pre Conditions - The given list of parent transforms must match up
        /// with the list of model transforms such that the model at index i
        /// should be the child of the parent at index i.
        /// Post Conditions - Each model transform is parented to its matching parent
        /// and has its local transform reset (localPos = 0, localRot = identity, localScale = 1)
        /// </summary>
        /// <param name="modelParentTransforms">List of parent transforms that match with
        /// this movement part's model list.</param>
        public void MoveModels(IReadOnlyList<Transform> modelParentTransforms)
        {
            Assert.AreEqual(m_modelTransformList.Count, modelParentTransforms.Count,
                $"{name}'s {typeof(MovementPartModels).Name} has {m_modelTransformList.Count} models. " +
                $"The given model parents list only had {modelParentTransforms.Count}.");

            // Move each individual model.
            for (int i = 0; i < m_modelTransformList.Count; ++i)
            {
                Transform temp_modelTrans = m_modelTransformList[i];
                Transform temp_parent = modelParentTransforms[i];
                MoveSingleModel(temp_modelTrans, temp_parent);
            }
        }


        /// <summary>
        /// Sets given model to the given parent's position, rotation, and scale.
        ///
        /// Pre Conditions - None
        /// Post Conditions - The given model is parented to the given parent and its local
        /// transform is reset.
        /// </summary>
        /// <param name="modelTrans">Model transform to become parent's child.</param>
        /// <param name="parent">Parent with position/rotation/scale information.</param>
        private void MoveSingleModel(Transform modelTrans, Transform parent)
        {
            modelTrans.position = parent.position;
            modelTrans.rotation = parent.rotation;
            modelTrans.localScale = parent.localScale;
        }
    }
}
