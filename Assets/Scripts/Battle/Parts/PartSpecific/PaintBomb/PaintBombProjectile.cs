using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey


namespace DuolBots
{
    /// <summary>
    /// Script that handles the behavior for the PaintBomb projectile.
    /// </summary>
    public class PaintBombProjectile : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const string PART = "Part";
        private const string PART_DAMAGABLE = "PartDamagable";

        // Prefab with Canvas and splatter effect GameObjects attached (uses screen overlay and is per instance of the PaintBomb weapon)
        private Canvas m_splatterCanvasFirstTeam = null;
        private Canvas m_splatterCanvasSecondTeam = null;

        // Splatter effect objects and canvases for team one
        private List<GameObject> m_screenEffectsFirstTeam;
        // Splatter effect objects and canvases for team two
        private List<GameObject> m_screenEffectsSecondTeam;

        // The list of the objects that THIS projectile currently is affecting canvases with.
        private List<GameObject> m_projectileCanvasEffects = new List<GameObject>();
        // The 4 cameras (2 for each team) for the bots
        private List<Camera> m_cameras = null;
        public void SetCameras(List<Camera> cams) { m_cameras = cams; }

        // The TeamIndex of the bot that fired the projectile (passed in by weapon fire controller)
        private TeamIndex m_teamIndex = null;
        private void SetTeamIndex(TeamIndex index)
        { m_teamIndex = index; }


        private void Awake()
        {
            // Set all effect GameObjects inactive
            ResetEffectCanvases();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check that the collision hit a part of a bot
            if (other.CompareTag(PART) || other.CompareTag(PART_DAMAGABLE))
            {
                // Get the bot's TeamIndex and check that the projectile is not causing damage to the bot that shot it
                TeamIndex temp_index = other.GetComponent<TeamIndex>();
                Assert.IsNotNull(temp_index, $"No {nameof(temp_index)} could be found on {other.name}");

                // Debugging collision
                if (IS_DEBUGGING && temp_index != null)
                {
                    CustomDebug.Log($"{this.name} hit {other.name} with TeamIndex {temp_index.teamIndex}", IS_DEBUGGING);
                }
                else if (IS_DEBUGGING && temp_index == null)
                {
                    CustomDebug.Log($"{this.name} hit {other.name} which did not have a {typeof(TeamIndex)}", IS_DEBUGGING);
                }

                // Check that the collision hit a part with a different index that the bot that fired the projectile
                if (temp_index != null)
                {
                    // Select a random screen effect to use (can use either team screen effects list since they should be the same size).
                    int temp_randomEffect = 0;

                    // Check that the number of screen effects for each Canvas are the same
                    if (m_screenEffectsFirstTeam.Count > 0 && m_screenEffectsFirstTeam.Count == m_screenEffectsFirstTeam.Count)
                    {
                        // Get the list of objects to activate on the other bot's canvas
                        List<Camera> temp_camerasToEffect = new List<Camera>();
                        foreach (Camera cam in m_cameras)
                        {
                            TeamIndex temp_camIndex = cam.GetComponent<TeamIndex>();
                            if (temp_camIndex != null && temp_camIndex.teamIndex != temp_index.teamIndex)
                            {
                                // Get the list of objects for the cam, set the GameObject at temp_randomEffect to active,
                                switch (temp_camIndex.teamIndex)
                                {
                                    case 0:
                                        m_screenEffectsFirstTeam[temp_randomEffect].SetActive(true);
                                        m_projectileCanvasEffects.Add(m_screenEffectsFirstTeam[temp_randomEffect]);
                                        break;
                                    case 1:
                                        m_screenEffectsSecondTeam[temp_randomEffect].SetActive(true);
                                        m_projectileCanvasEffects.Add(m_screenEffectsSecondTeam[temp_randomEffect]);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else if(m_screenEffectsFirstTeam.Count > 0 && m_screenEffectsSecondTeam.Count > 0)
                    {
                        Debug.LogError($"{name} did not apply a splatter effect to any Canvases because {m_splatterCanvasFirstTeam.name}" +
                            $"has {m_screenEffectsFirstTeam.Count} effects and {m_splatterCanvasSecondTeam.name} has " +
                            $"{m_screenEffectsSecondTeam.Count} effects. These MUST be equal.");
                    }
                    else
                    {
                        Debug.LogError($"Team 1 effect size: {m_screenEffectsFirstTeam.Count}, Team 2 effect size {m_screenEffectsSecondTeam.Count}");
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ResetEffectCanvases();
        }


        private void ResetEffectCanvases()
        {
            // Deactivates the effects that this projectile has activated 
            foreach (GameObject go in m_projectileCanvasEffects)
            { go.SetActive(false); }
        }

        public void SetupTeam(int index, List<GameObject> screenEffects)
        {
            if(index != 1 && index != 0)
            {
                index %=2;
            }
            switch (index)
            {
                case 0:
                    m_screenEffectsFirstTeam = screenEffects;
                    break;
                case 1:
                    m_screenEffectsSecondTeam = screenEffects;
                    break;
                default:
                    // Should not get here
                    Debug.LogError($"{name} called SetupTeam with an invalid index. Valid indices are 0 and 1, actual was {index}");
                    break;
            }
        }
    
    }
}
