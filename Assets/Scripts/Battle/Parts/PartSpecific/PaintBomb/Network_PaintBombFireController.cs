using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Network implementation of PaintBombFireController that tells the Server to
    /// instantiate a NetworkSplatterCanvas to affect the opposing bot's screen.
    /// </summary>
    public class Network_PaintBombFireController : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Prefab to spawn as a projectile on turret fire
        private GameObject m_projectilePrefab = null;
        private GameObject m_splatterEffectCanvas = null;

        // TODO: SWAP TO TAKING GAME OBJECTS OFF OF ONE SINGLE CANVAS WHICH IS SPAWNED ON PREFAB
        // TO AFFECT THE TEAM (SINCE IT IS USING OVERLAY MODE AND WILL BE SPAWNED ON THE INSTANCE
        // OF UNITY THAT THE OPPOSING BOT IS ON. NO NEED FOR MULTIPLE CANVASES OR LIST<LIST<GAMEOBJECT>>

        // Splatter effect objects and canvases for team one
        private List<GameObject> m_screenEffectsFirstTeam;
        private List<Canvas> m_effectCanvasesFirstTeam;
        // Splatter effect objects and canvases for team two
        private List<GameObject> m_screenEffectsSecondTeam;
        private List<Canvas> m_effectCanvasesSecondTeam;

        // Relationship between Cameras and the List<GameObject>'s that affect the Canvas corresponding to that Camera
        private Dictionary<Camera, List<GameObject>> m_camEffects;

        // The list of the objects that THIS projectile currently is affecting canvases with.
        private List<GameObject> m_projectileCanvasEffects = new List<GameObject>();
        // The 4 cameras (2 for each team) for the bots
        private List<Camera> m_cameras = null;

        // The TeamIndex of the bot that fired the projectile (passed in by weapon fire controller)
        private ITeamIndex m_teamIndex = null;

        // How long the current cooldown as left
        private CooldownRemaining m_coolDownRemaining = null;
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
            Assert.IsNotNull(m_projectilePrefab, $"Projectile Prefab not " +
                $"specified for {name}'s {GetType().Name}");

            Assert.IsNotNull(m_splatterEffectCanvas, $"Projectile Prefab not" +
                $"specified for {name}'s {GetType().Name}");

            m_specifications = GetComponent<Specifications_PaintBombFireController>();
            Assert.IsNotNull(m_specifications,
                $"{typeof(Specifications_PaintBombFireController).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            m_botRootVelocityCalculator =
                GetComponentInParent<TransformVelocityCalculator>();
            Assert.IsNotNull(m_botRootVelocityCalculator,
                $"{typeof(TransformVelocityCalculator).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            m_coolDownRemaining = GetComponent<CooldownRemaining>();

            // Set up canvases to apply to their corresponding cameras
            foreach (Camera cam in FindObjectsOfType<Camera>())
            {
                m_cameras.Add(cam);
                ITeamIndex temp_camIndex = cam.GetComponent<TeamIndex>();
                // Assign the cameras
                int temp_teamOneCamCount = 0;
                int temp_teamTwoCamCount = 0;

                // Assign the cam to the appropriate worldCamera of one of the team's canvases.
                // Add entry to dictionary using cam as a key and an appropriate List<GameObject> as the value.
                if (temp_camIndex != null)
                {
                    switch (temp_camIndex.teamIndex)
                    {
                        case 1:
                            temp_teamOneCamCount++;
                            m_effectCanvasesFirstTeam[temp_teamOneCamCount - 1].worldCamera = cam;
                            //m_camEffects.Add(cam, m_screenEffectsFirstTeam[temp_teamOneCamCount - 1]);
                            break;
                        case 2:
                            temp_teamTwoCamCount++;
                            m_effectCanvasesSecondTeam[temp_teamTwoCamCount - 1].worldCamera = cam;
                            //m_camEffects.Add(cam, m_screenEffectsSecondTeam[temp_teamTwoCamCount - 1]);
                            break;
                        default:
                            CustomDebug.Log($"{this.name} found a camera, {cam.name} without a {typeof(TeamIndex)}", IS_DEBUGGING);
                            break;
                    }
                }
            }

            Assert.AreEqual(m_effectCanvasesFirstTeam.Count, m_effectCanvasesSecondTeam.Count, $"{this.name} could not find an equal" +
                $" number of effect {nameof(m_effectCanvasesFirstTeam)} for each team.");
            Assert.AreEqual(m_camEffects.Count, m_effectCanvasesFirstTeam.Count + m_effectCanvasesSecondTeam.Count, $"{this.name} " +
                $"found an uneqal number of {nameof(m_cameras)} and {nameof(m_effectCanvasesFirstTeam)}");
            Assert.IsNotNull(m_teamIndex, $"{this.name} does not have a {nameof(m_teamIndex)} assigned but is required.");
            Assert.IsFalse(m_effectCanvasesFirstTeam.Count <= 0, $"{this.name} could not activate any effect {typeof(GameObject)} because" +
                $" a {nameof(m_effectCanvasesFirstTeam)} had a size of 0 or less.");
            Assert.IsFalse(m_effectCanvasesSecondTeam.Count <= 0, $"{this.name} could not activate any effect {typeof(GameObject)} because" +
                $" a {nameof(m_effectCanvasesSecondTeam)} had a size of 0 or less.");
        }

        private void Update()
        {
            SpawnProjectile();
        }

        public void Fire(bool value)
        {
            isFiring = value;
        }

        public void AlternateFire(bool value)
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
                    temp_paintBomb.SetupTeam(1, m_screenEffectsSecondTeam);
                }
                else { Debug.LogError($"{name} spawned a {nameof(m_projectilePrefab)} that does not have an attached {typeof(PaintBombProjectile)} but requires one."); }

                // Reset cooldown
                m_curCoolDown = m_specifications.coolDown;

                if (!m_specifications.autoFire) { return; }
            }
        }
    }
}
