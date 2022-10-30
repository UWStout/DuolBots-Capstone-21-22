using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// On crushing hand. Crushes and inflates the bot roots.
    /// 
    /// </summary>
    public class CrushOnTrigger : MonoBehaviour
    {
        [SerializeField] [Min(0.0f)] private float m_incAmount = 0.01f;

        // List of transforms being crushed/inflated
        private List<CrushedTransform> m_affectedTransforms
            = new List<CrushedTransform>();
        private bool m_isUpdating = false;


        private void OnTriggerEnter(Collider other)
        {
            // Add the transform to the list to be crushed.
            CrushedTransform temp_crushedTrans =
                new CrushedTransform(other.attachedRigidbody.transform);
            m_affectedTransforms.Add(temp_crushedTrans);

            // Start the update loop if not already started
            StartUpdateTransformSizeCoroutine();
        }
        private void OnTriggerExit(Collider other)
        {
            Transform temp_otherTrans = other.attachedRigidbody.transform;
            for (int i = 0; i < m_affectedTransforms.Count; ++i)
            {
                // Set the crushing state of this transform to inflate
                if (m_affectedTransforms[i].afflictedTrans == temp_otherTrans)
                {
                    m_affectedTransforms[i].curState = CrushedTransform.eCrushState.Inflating;
                }
            }
        }


        private void StartUpdateTransformSizeCoroutine()
        {
            // Don't start if its already started
            if (m_isUpdating) { return; }

            StartCoroutine(UpdateTransformSizeCoroutine());
        }
        private IEnumerator UpdateTransformSizeCoroutine()
        {
            m_isUpdating = true;

            while (m_affectedTransforms.Count > 0)
            {
                for (int i = 0; i < m_affectedTransforms.Count; ++i)
                {
                    CrushedTransform temp_crushedTrans = m_affectedTransforms[i];

                    if (temp_crushedTrans.afflictedTrans == null)
                    {
                        m_affectedTransforms.RemoveAt(i);
                        --i;
                        continue;
                    }

                    switch (temp_crushedTrans.curState)
                    {
                        case CrushedTransform.eCrushState.Crushing:
                            Crush(temp_crushedTrans.afflictedTrans);
                            break;
                        case CrushedTransform.eCrushState.Inflating:
                            Inflate(temp_crushedTrans.afflictedTrans);
                            break;
                        default:
                            CustomDebug.UnhandledEnum(temp_crushedTrans.curState,
                                $"{GetType().Name}'s {name}");
                            break;
                    }
                }

                yield return null;
            }

            m_isUpdating = false;
        }
        private void Crush(Transform trans)
        {
            Vector3 temp_scale = trans.localScale;
            temp_scale.y = Mathf.Max(0.0f, temp_scale.y -
                (m_incAmount * Time.deltaTime));
            trans.localScale = temp_scale;
        }
        private void Inflate(Transform trans)
        {
            Vector3 temp_scale = trans.localScale;
            temp_scale.y = Mathf.Min(1.0f, temp_scale.y +
                (m_incAmount * Time.deltaTime));
            trans.localScale = temp_scale;

            if (temp_scale.y == 1.0f)
            {
                for (int i = 0; i < m_affectedTransforms.Count; ++i)
                {
                    if (m_affectedTransforms[i].afflictedTrans == trans)
                    {
                        m_affectedTransforms.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
    }

    public class CrushedTransform
    {
        public enum eCrushState { Crushing, Inflating }


        public Transform afflictedTrans;
        public eCrushState curState;


        public CrushedTransform(Transform transform)
        {
            afflictedTrans = transform;
        }
    }
}
