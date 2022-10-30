using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Local weapon fire controller for the PaintBomb (uses specifications to give projectile information on what GameObjects to affect in the scene).
    /// </summary>
    [RequireComponent(typeof(TransformVelocityCalculator))]
    public class Local_PaintBombFireController : MonoBehaviour, IWeaponFireController
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        // Tags used to find objects for each team and each player T=team, P = player (only 2 teams so 0 and 1)
        private const string CANVAS_TAG = "BotCanvas";

        private CooldownRemaining m_coolDownRemaining = null;

        // Prefab to spawn as a projectile on turret fire
        private GameObject m_projectilePrefab = null;

        // Canvases for each team
        private Canvas m_team0Canvas = null;
        private Canvas m_team1Canvas = null;

        // Splatter effect objects and canvases for team one
        private List<GameObject> m_screenEffectsFirstTeam;

        // Splatter effect objects and canvases for team two
        private List<GameObject> m_screenEffectsSecondTeam;

        // Relationship between Cameras and the List<GameObject>'s that affect the Canvas corresponding to that Camera
        private Dictionary<Camera, List<GameObject>> m_camEffects;

        // The 4 cameras (2 for each team) for the bots
        private List<Camera> m_cameras = null;

        // The TeamIndex of the bot that fired the projectile (passed in by weapon fire controller)
        private ITeamIndex m_teamIndex = null;
        

        // How long the current cooldown as left
        private float m_curCoolDown = 0.0f;

        private bool m_isFiring = false;
        public bool isFiring
        {
            set => m_isFiring = value;
            get => m_isFiring;
        }

        // Specifications for variables
        private Specifications_PaintBombFireController
            m_specifications = null;

        // Componenet that calcuates parent velocity
        private TransformVelocityCalculator m_botRootVelocityCalculator = null;


        // Domestic initialization
        private void Awake()
        {
            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            Assert.IsNotNull(m_coolDownRemaining, $"{name} does not have a {typeof(CooldownRemaining)} but requires one.");
            Assert.IsNotNull(m_projectilePrefab, $"Projectile Prefab not " +
                $"specified for {name}'s {GetType().Name}");

            m_botRootVelocityCalculator =
                GetComponentInParent<TransformVelocityCalculator>();
            Assert.IsNotNull(m_botRootVelocityCalculator,
                $"{typeof(TransformVelocityCalculator).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            m_specifications = GetComponent<Specifications_PaintBombFireController>();
            Assert.IsNotNull(m_specifications,
                $"{typeof(Specifications_PaintBombFireController).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            // Get all of the cameras in the scene 
            m_cameras.AddRange(FindObjectsOfType<Camera>());

            SetupTeamEffectCanvases();
        }

        private void Update()
        {
            SpawnProjectile();
        }

        public void Fire(bool value, eInputType type)
        {
            m_coolDownRemaining.inputType = type;
            isFiring = value;
        }

        public void AlternateFire(bool value, eInputType type)
        { /*This controller does not utilize alternate firing.*/ }

        private void SpawnProjectile()
        {
            if (m_curCoolDown >= 0.0f)
            {
                m_curCoolDown -= Time.deltaTime;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);
            }
            else if (isFiring && m_curCoolDown <= 0.0f)
            {
                // Instantiate projectile
                GameObject temp_spawnedObject =
                Instantiate(m_projectilePrefab,
                m_specifications.projectileSpawnPos.position,
                m_specifications.projectileSpawnPos.rotation);

                // Check if the projectile should inherit its parent's velocity
                if (m_specifications.inheritsParentsVel)
                {
                    if (!temp_spawnedObject.TryGetComponent(out
                        Rigidbody temp_rigidBody))
                    {
                        Debug.LogError($"Spawned projectile, " +
                            $"{temp_rigidBody.name} that is trying to inherit " +
                            $"velocity does not have a rigidbody to inherit " +
                            $"from.");
                        return;
                    }
                    temp_rigidBody.velocity +=
                        m_botRootVelocityCalculator.velocitySinceLastFrame;
                }


                // Setup PaintBombProjectile's screen effects, canvases, and cams
                PaintBombProjectile temp_paintBomb = temp_spawnedObject.GetComponent<PaintBombProjectile>();
                if (temp_paintBomb != null)
                {
                    temp_paintBomb.SetupTeam(0, m_screenEffectsFirstTeam);
                    temp_paintBomb.SetCameras(m_cameras);
                }
                else
                {
                    Debug.LogError($"{name} spawned a {nameof(m_projectilePrefab)} that does not have an " +
                    $"attached {typeof(PaintBombProjectile)} but requires one.");
                }

                // Reset cooldown
                m_curCoolDown = m_specifications.coolDown;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);

                if (!m_specifications.autoFire) { return; }
            }
        }

        private void SetupTeamEffectCanvases()
        {
            List<GameObject> temp_splatterEffects = new List<GameObject>();
            List<Canvas> temp_canvases = new List<Canvas>();

            // Set each team's canvas and List<GameObject> for splatter effects based off of the Canvas on each bot
            // using the TeamIndex attached to it.
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(CANVAS_TAG))
            {
                Canvas temp_canvas = go.GetComponent<Canvas>();
                ITeamIndex temp_teamIndex = go.GetComponent<TeamIndex>();

                if (temp_canvas != null)
                {
                    if (temp_teamIndex != null)
                    {
                        switch (temp_teamIndex.teamIndex)
                        {
                            case 0:
                                m_team0Canvas = temp_canvas;
                                m_screenEffectsFirstTeam.AddRange(temp_canvas.GetComponentsInChildren<GameObject>());

                                Assert.IsNotNull(m_screenEffectsFirstTeam, $"{name} could not find splatter effect" +
                                    $"{typeof(GameObject)}'s on {nameof(temp_canvas)}");
                                break;
                            case 1:
                                m_team1Canvas = temp_canvas;
                                m_screenEffectsSecondTeam.AddRange(temp_canvas.GetComponentsInChildren<GameObject>());

                                Assert.IsNotNull(m_screenEffectsSecondTeam, $"{name} could not find splatter effect" +
                                    $"{typeof(GameObject)}'s on {nameof(temp_canvas)}");
                                break;
                            default:
                                CustomDebug.Log($"{this.name} found a {typeof(Canvas)} with a team index of {temp_teamIndex.teamIndex}", IS_DEBUGGING);
                                break;
                        }
                    }
                }
            }
        }
    }
}

