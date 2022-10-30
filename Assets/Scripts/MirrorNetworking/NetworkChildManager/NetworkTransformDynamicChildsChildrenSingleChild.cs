using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Holds a target transform to be synced over the network.
    /// 
    /// Should only be added to a GameObject by
    /// <see cref="NetworkTransformDynamicChildsChildren"/>.
    /// </summary>
    [ExecuteAlways]
    public class NetworkTransformDynamicChildsChildrenSingleChild :
        NetworkTransformDynamicChildBase
    {
        // The transform that should be synced.
        public override Transform targetComponent => m_targetComponent;
        private Transform m_targetComponent = null;


        /// <summary>
        /// Adds a <see cref="NetworkTransformDynamicChildsChildrenSingleChild"/> to
        /// the given GameObject as a component with the specified variables.
        /// </summary>
        /// <param name="objectToBeAddedTo">What object to spawn the script as a
        /// component of.</param>
        /// <param name="targetComp">Transform that is to have its data
        /// synced.</param>
        /// <param name="shouldSyncPos">If the position of the transform should
        /// be synced.</param>
        /// <param name="shouldSyncRot">If the rotation of the transform should
        /// be synced.</param>
        /// <param name="shouldSyncScale">If the scale of the transform should
        /// be synced.</param>
        /// <param name="shouldSyncParent">If the parent of the transform should
        /// be synced.</param>
        /// <param name="sensitivityPos">How much should we let the position change
        /// before trying to sync it.</param>
        /// <param name="sensitivityRot">How much should we let the rotation change
        /// before trying to sync it.</param>
        /// <param name="sensitivityScale">How much should we let the scale change
        /// before trying to sync it.</param>
        /// <returns>Added component.</returns>
        public static NetworkTransformDynamicChildsChildrenSingleChild
            Create(GameObject objectToBeAddedTo, Transform targetComp,
            bool shouldSyncPos, bool shouldSyncRot, bool shouldSyncScale,
            bool shouldSyncParent, float sensitivityPos, float sensitivityRot,
            float sensitivityScale)
        {
            NetworkTransformDynamicChildsChildrenSingleChild temp_singleChild = 
                objectToBeAddedTo.AddComponent
                <NetworkTransformDynamicChildsChildrenSingleChild>();

            temp_singleChild.m_syncPosition = shouldSyncPos;
            temp_singleChild.m_syncRotation = shouldSyncRot;
            temp_singleChild.m_syncScale = shouldSyncScale;
            temp_singleChild.m_syncParent = shouldSyncParent;

            temp_singleChild.m_positionSensitivity = sensitivityPos;
            temp_singleChild.m_rotationSensitivity = sensitivityRot;
            temp_singleChild.m_scaleSensitivity = sensitivityScale;

            temp_singleChild.m_targetComponent = targetComp;

            return temp_singleChild;
        }


#if UNITY_EDITOR
        private void Update()
        {
            Assert.IsFalse(!Application.isPlaying, $"{name}'s {GetType().Name} " +
                $"shouldn't exist in the Editor. It should only be instantiated " +
                $"by {nameof(NetworkTransformDynamicChildsChildren)} during " +
                $"runtime");
        }
#endif
    }
}
