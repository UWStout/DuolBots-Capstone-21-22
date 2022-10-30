using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(Shared_VisualBombCharging))]
    public class Network_VisualBombCharging : NetworkChildBehaviour
    {
        private Shared_VisualBombCharging m_sharedVisualBomb = null;

        private bool m_curActiveState = false;


        protected override void Awake()
        {
            base.Awake();

            m_sharedVisualBomb = GetComponent<Shared_VisualBombCharging>();
            Assert.IsNotNull(m_sharedVisualBomb, $"{name}'s {GetType().Name} " +
                $"requires a {nameof(Shared_VisualBombCharging)} but none was " +
                $"found");
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            // Start the visual off as off
            SetProjectilePreviewActive(false);
        }
        private void Update()
        {
            if (!isServer) { return; }

            if (m_sharedVisualBomb.sharedController.isCharging)
            {
                SetProjectilePreviewActive(true);
                m_sharedVisualBomb.UpdatePreviewObjectScale(
                    m_sharedVisualBomb.sharedController.curCharge);
            }
            else
            {
                SetProjectilePreviewActive(false);
            }
        }


        [Server]
        private void SetProjectilePreviewActive(bool cond)
        {
            if (m_curActiveState == cond) { return; }

            messenger.SendMessageToClient(gameObject,
                nameof(SetProjectilePreviewActiveClient), cond);
            m_curActiveState = cond;
        }

        [Client]
        private void SetProjectilePreviewActiveClient(bool cond)
        {
            m_sharedVisualBomb.projectilePreviewInstance.SetActive(cond);
        }
    }
}
