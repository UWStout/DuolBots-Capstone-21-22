using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class TransformDataWithParent
    {
        private TransformData m_transformData = new TransformData();
        private TransformChildPath m_pathToParent = new TransformChildPath();

        public TransformData transformData => m_transformData;
        public TransformChildPath pathToParent => m_pathToParent;


        public TransformDataWithParent()
        {
            m_transformData = new TransformData();
            m_pathToParent = new TransformChildPath();
        }
        public TransformDataWithParent(TransformData transData,
            TransformChildPath childPath)
        {
            m_transformData = transData;
            m_pathToParent = childPath;
        }
        public TransformDataWithParent(TransformData transData,
            Transform parent, Transform descendant)
        {
            m_transformData = transData;
            m_pathToParent = new TransformChildPath(parent, descendant);
        }
        public TransformDataWithParent(TransformData transData, int[] childPath)
        {
            m_transformData = transData;
            m_pathToParent = new TransformChildPath(childPath);
        }
        public TransformDataWithParent(Vector3 position, Quaternion rotation,
            Vector3 scale, Transform parent, Transform descendant)
        {
            m_transformData = new TransformData(position, rotation, scale);
            m_pathToParent = new TransformChildPath(parent, descendant);
        }
        public TransformDataWithParent(Vector3 position, Quaternion rotation,
            Vector3 scale, int[] childPath)
        {
            m_transformData = new TransformData(position, rotation, scale);
            m_pathToParent = new TransformChildPath(childPath);
        }
    }
}
