using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class NetworkBomberVisualPreview : NetworkChildBehaviour
    {
        private const bool IS_DEBUGGING = false;


        [SerializeField] [Required] [ShowAssetPreview]
        private GameObject m_projectilePreviewPrefab = null;

        public Shared_VisualBombCharging visualBombCharging { set; get; }

        private bool m_curVisualActiveState = false;


        public override void OnStartServer()
        {
            base.OnStartServer();

            Transform temp_projSpawnTrans = visualBombCharging.sharedController.
                specifications.projectileSpawnPos;
            GameObject temp_previewObj = Instantiate(m_projectilePreviewPrefab,
                temp_projSpawnTrans.position, temp_projSpawnTrans.rotation,
                temp_projSpawnTrans);

            manager.Spawn(temp_previewObj);
        }
        private void Update()
        {
            if (!isServer) { return; }

            if (visualBombCharging.sharedController.isCharging)
            {
                SetProjectilePreviewActiveServer(true);
                visualBombCharging.UpdatePreviewObjectScale(
                    visualBombCharging.sharedController.curCharge);
            }
            else
            {
                SetProjectilePreviewActiveServer(false);
            }
        }

        [Server]
        private void SetProjectilePreviewActiveServer(bool cond)
        {
            CustomDebug.Log($"{nameof(SetProjectilePreviewActiveServer)}",
                IS_DEBUGGING);

            // Don't send a message if it is already in that state.
            if (m_curVisualActiveState == cond) { return; }

            messenger.SendMessageToClient(gameObject,
                nameof(SetProjectilePreviewActiveClient), cond);

            m_curVisualActiveState = cond;
        }

        [Client]
        private void SetProjectilePreviewActiveClient(bool cond)
        {
            CustomDebug.Log($"{nameof(SetProjectilePreviewActiveClient)}",
                IS_DEBUGGING);

            visualBombCharging.SetPreviewObjectActive(cond);
        }
    }
}
