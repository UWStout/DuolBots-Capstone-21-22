#define onlySyncOnChange_BANDWIDTH_SAVING
using System;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik
// Copied a lot from NetworkTransformBase

namespace DuolBots.Mirror
{
    /// <summary>
    /// WARNING. Make sure you don't copy and paste component values
    /// and if you do, regenerate the Guid and make sure it saves.
    /// Does not save by default because it hates me.
    /// 
    /// Much is copied with small tweaks from
    /// <see cref="NetworkTransformBase"/>
    /// </summary>
    public abstract class NetworkTransformDynamicChildBase : NetworkChildBehaviour
    {
#if UNITY_EDITOR
        // GUID for the component so that a single gameobject
        // can sync multiple children without the messages overlapping.
        [InspectorButton(nameof(OnRegenerateGuidButton))] [SerializeField]
        private bool regenGuid = false;
        protected void OnRegenerateGuidButton()
        {
            m_componentGuid = Guid.NewGuid().ToString();
        }
#endif
        [Tooltip("WARNING. DON'T COPY VALUES. This will not be unique if values " +
            "are copied and pasted, even as 'new.' Also regen Guid does not " +
            "save. Edit this field manually to get it to save after generation. ")]
        [SerializeField] private string m_componentGuid = Guid.NewGuid().ToString();

        [Header("Synchronization")]
        [Range(0, 1)] protected float m_sendInterval = 0.050f;
        [SerializeField] protected bool m_syncPosition = true;
        [SerializeField] protected bool m_syncRotation = true;
        [SerializeField] protected bool m_syncScale = false;
        [SerializeField] protected bool m_syncParent = false;
        private double m_lastClientSendTime = 0.0f;
        private double m_lastServerSendTime = 0.0f;

        [Header("Interpolation")]
        [SerializeField] protected bool m_interpolatePosition = true;
        [SerializeField] protected bool m_interpolateRotation = true;
        [SerializeField] protected bool m_interpolateScale = false;

        // "Experimentally I’ve found that the amount of delay that works best
        //  at 2-5% packet loss is 3X the packet send rate"
        // NOTE: we do NOT use a dyanmically changing buffer size.
        //       it would come with a lot of complications, e.g. buffer time
        //       advantages/disadvantages for different connections.
        //       Glenn Fiedler's recommendation seems solid, and should cover
        //       the vast majority of connections.
        //       (a player with 2000ms latency will have issues no matter what)
        [Header("Buffering")]
        [Tooltip("Snapshots are buffered for sendInterval * multiplier seconds. " +
            "If your expected client base is to run at non-ideal connection " +
            "quality (2-5% packet loss), 3x supposedly works best.")]
        [SerializeField] private int m_bufferTimeMultiplier = 1;

        [Tooltip("Buffer size limit to avoid ever growing list memory " +
            "consumption attacks.")]
        [SerializeField] private int m_bufferSizeLimit = 64;

        [Tooltip("Start to accelerate interpolation if buffer size is >= " +
            "threshold. Needs to be larger than bufferTimeMultiplier.")]
        [SerializeField] private int m_catchupThreshold = 4;

        [Tooltip("Once buffer is larger catchupThreshold, accelerate by " +
            "multiplier % per excess entry.")]
        [Range(0, 1)] [SerializeField] private float m_catchupMultiplier = 0.10f;

#if onlySyncOnChange_BANDWIDTH_SAVING
        [Header("Sync Only If Changed")]
        [Tooltip("When true, changes are not sent unless greater than " +
            "sensitivity values below.")]
        [SerializeField] private bool m_onlySyncOnChange = true;

        // 3 was original, but testing under really bad network conditions,
        // 2%-5% packet loss and 250-1200ms ping, 5 proved to eliminate any
        // twitching.
        [Tooltip("How much time, as a multiple of send interval, has " +
            "passed before clearing buffers.")]
        [SerializeField] private float m_bufferResetMultiplier = 5;

        [Header("Sensitivity")]
        [SerializeField] protected float m_positionSensitivity = 0.01f;
        [SerializeField] protected float m_rotationSensitivity = 0.01f;
        [SerializeField] protected float m_scaleSensitivity = 0.01f;

        protected bool m_positionChanged = false;
        protected bool m_rotationChanged = false;
        protected bool m_scaleChanged = false;

        // Used to store last sent snapshots
        protected NTSnapshot m_lastSnapshot = default;
        protected bool m_cachedSnapshotComparison = false;
        protected bool m_hasSentUnchangedPosition = false;
#endif

        // snapshots sorted by timestamp
        // in the original article, glenn fiedler drops any snapshots older than
        // the last received snapshot.
        // -> instead, we insert into a sorted buffer
        // -> the higher the buffer information density, the better
        // -> we still drop anything older than the first element in the buffer
        // => internal for testing
        //
        // IMPORTANT: of explicit 'NTSnapshot' type instead of 'Snapshot'
        //            interface because List<interface> allocates through boxing
        private SortedList<double, NTSnapshot> serverBuffer
            = new SortedList<double, NTSnapshot>();
        private SortedList<double, NTSnapshot> clientBuffer
            = new SortedList<double, NTSnapshot>();

        // absolute interpolation time, moved along with deltaTime
        // (roughly between [0, delta] where delta is snapshot B - A timestamp)
        // (can be bigger than delta when overshooting)
        private double m_serverInterpolationTime = 0.0f;
        private double m_clientInterpolationTime = 0.0f;

        // only convert the static Interpolation function to Func<T> once to
        // avoid allocations
        private Func<NTSnapshot, NTSnapshot, double, NTSnapshot>
            m_interpolateFunc = NTSnapshot.Interpolate;


        // Synchronization
        public float sendInterval => m_sendInterval;
        public bool syncPosition => m_syncPosition;
        public bool syncRotation => m_syncRotation;
        public bool syncScale => m_syncScale;
        public bool syncParent => m_syncParent;
        public double lastClientSendTime => m_lastClientSendTime;
        public double lastServerSendTime => m_lastServerSendTime;
        // Sensitivity
        public float positionSensitivity => m_positionSensitivity;
        public float rotationSensitivity => m_rotationSensitivity;
        public float scaleSensitivity => m_scaleSensitivity;
        // Interpolation
        public bool interpolatePosition => m_interpolatePosition;
        public bool interpolateRotation => m_interpolateRotation;
        public bool interpolateScale => m_interpolateScale;
        // Buffering
        public float bufferTime => m_sendInterval * m_bufferTimeMultiplier;


        public virtual Transform targetComponent => transform;


        protected virtual void OnDisable() => Reset();
        protected virtual void OnEnable() => Reset();
        public virtual void Reset()
        {
            // disabled objects aren't updated anymore.
            // so let's clear the buffers.
            serverBuffer.Clear();
            clientBuffer.Clear();

            // reset interpolation time too so we start at t=0 next time
            m_serverInterpolationTime = 0;
            m_clientInterpolationTime = 0;
        }
        private void Update()
        {
            // if server then always sync to others.
            if (isServer) UpdateServer();
            // 'else if' because host mode shouldn't send anything to server.
            // it is the server. don't overwrite anything there.
            else if (isClient) UpdateClient();
        }


        private void UpdateServer()
        {
            // broadcast to all clients each 'sendInterval'
            // (client with authority will drop the rpc)
            // NetworkTime.localTime for double precision until Unity has it too
            //
            // IMPORTANT:
            // snapshot interpolation requires constant sending.
            // DO NOT only send if position changed. for example:
            // ---
            // * client sends first position at t=0
            // * ... 10s later ...
            // * client moves again, sends second position at t=10
            // ---
            // * server gets first position at t=0
            // * server gets second position at t=10
            // * server moves from first to second within a time of 10s
            //   => would be a super slow move, instead of a wait & move.
            //
            // IMPORTANT:
            // DO NOT send nulls if not changed 'since last send' either. we
            // send unreliable and don't know which 'last send' the other end
            // received successfully.
            //
            // Checks to ensure server only sends snapshots if object is
            // on server authority(!clientAuthority) mode because on client
            // authority mode snapshots are broadcasted right after the
            // authoritative client updates server in the command
            // function(see above), OR, since host does not send anything to update
            // the server, any client authoritative movement done by the host will
            // have to be broadcasted here by checking IsClientWithAuthority.
            //
            // !!!!!!!!!!!!! Hi Wyatt here. Yeah, I'm not doing anything with
            // client authority. I just don't allow a transform to be in
            // client authority mode. This means I can't send any commands from this
            // scripts. Its not being I just don't want to. I really just can't do
            // it because this isn't actually a NetworkBehaviour.
            if (NetworkTime.localTime >= lastServerSendTime + sendInterval)
            {
                // send snapshot without timestamp.
                // receiver gets it from batch timestamp to save bandwidth.
                NTSnapshot snapshot = ConstructSnapshot();
#if onlySyncOnChange_BANDWIDTH_SAVING
                m_cachedSnapshotComparison = CompareSnapshots(snapshot);
                if (m_cachedSnapshotComparison && m_hasSentUnchangedPosition &&
                    m_onlySyncOnChange) { return; }
#endif

#if onlySyncOnChange_BANDWIDTH_SAVING
                ServerToClientSync(
                    // only sync what the user wants to sync
                    syncPosition && m_positionChanged ?
                    snapshot.position : default(Vector3?),

                    syncRotation && m_rotationChanged ?
                    snapshot.rotation : default(Quaternion?),

                    syncScale && m_scaleChanged ?
                    snapshot.scale : default(Vector3?)
                );
#else
                RpcServerToClientSync(
                    // only sync what the user wants to sync
                    syncPosition ? snapshot.position : default(Vector3?),
                    syncRotation ? snapshot.rotation : default(Quaternion?),
                    syncScale ? snapshot.scale : default(Vector3?)
                );
#endif

                m_lastServerSendTime = NetworkTime.localTime;
#if onlySyncOnChange_BANDWIDTH_SAVING
                if (m_cachedSnapshotComparison)
                {
                    m_hasSentUnchangedPosition = true;
                }
                else
                {
                    m_hasSentUnchangedPosition = false;
                    m_lastSnapshot = snapshot;
                }
#endif
            }
        }
        private void UpdateClient()
        {
            // we need to apply snapshots from the buffer

            // compute snapshot interpolation & apply if any was spit out
            // TODO we don't have Time.deltaTime double yet. float is fine.
            if (SnapshotInterpolation.Compute(
                NetworkTime.localTime, Time.deltaTime,
                ref m_clientInterpolationTime,
                bufferTime, clientBuffer,
                m_catchupThreshold, m_catchupMultiplier,
                m_interpolateFunc,
                out NTSnapshot computed))
            {
                NTSnapshot start = clientBuffer.Values[0];
                NTSnapshot goal = clientBuffer.Values[1];
                ApplySnapshot(start, goal, computed);
            }
        }


        // snapshot functions //////////////////////////////////////////////////
        // construct a snapshot of the current state
        // => internal for testing
        protected virtual NTSnapshot ConstructSnapshot()
        {
            // NetworkTime.localTime for double precision until Unity has it too
            return new NTSnapshot(
                // our local time is what the other end uses as remote time
                NetworkTime.localTime,
                // the other end fills out local time itself
                0,
                targetComponent.localPosition,
                targetComponent.localRotation,
                targetComponent.localScale
            );
        }
#if onlySyncOnChange_BANDWIDTH_SAVING
        // Returns true if position, rotation, scale, AND parent are unchanged,
        // within given sensitivity range.
        protected virtual bool CompareSnapshots(NTSnapshot currentSnapshot)
        {
            m_positionChanged = Vector3.SqrMagnitude(m_lastSnapshot.position -
                currentSnapshot.position) > positionSensitivity * positionSensitivity;
            m_rotationChanged = Quaternion.Angle(m_lastSnapshot.rotation,
                currentSnapshot.rotation) > rotationSensitivity;
            m_scaleChanged = Vector3.SqrMagnitude(m_lastSnapshot.scale -
                currentSnapshot.scale) > scaleSensitivity * scaleSensitivity;

            return (!m_positionChanged && !m_rotationChanged &&
                !m_scaleChanged);
        }
#endif
        // rpc /////////////////////////////////////////////////////////////////
        // only unreliable. see comment above of this file.
        //[ClientRpc(channel = Channels.Unreliable)]
        private void ServerToClientSync(Vector3? position, Quaternion? rotation,
            Vector3? scale)
        {
            messenger.SendMessageToClientUnreliable(gameObject,
                nameof(OnServerToClientSync), new NTDCClientSyncData(
                new UncertainTransformData(position, rotation, scale),
                m_componentGuid));
        }
        // server broadcasts sync message to all clients
        protected virtual void OnServerToClientSync(NTDCClientSyncData syncData)
        {
            // If it wasn't this component that is trying to sync.
            if (syncData.compGuid != m_componentGuid) { return; }

            UncertainTransformData temp_transData = syncData.transData;

            Vector3? position = temp_transData.position;
            Quaternion? rotation = temp_transData.rotation;
            Vector3? scale = temp_transData.scale;

            // in host mode, the server sends rpcs to all clients.
            // the host client itself will receive them too.
            // -> host server is always the source of truth
            // -> we can ignore any rpc on the host client
            // => otherwise host objects would have ever growing clientBuffers
            // (rpc goes to clients. if isServer is true too then we are host)
            if (isServer) return;

            // protect against ever growing buffer size attacks
            if (clientBuffer.Count >= m_bufferSizeLimit) return;

            // on the client, we receive rpcs for all entities.
            // not all of them have a connectionToServer.
            // but all of them go through NetworkClient.connection.
            // we can get the timestamp from there.
            double timestamp = NetworkClient.connection.remoteTimeStamp;
#if onlySyncOnChange_BANDWIDTH_SAVING
            if (m_onlySyncOnChange)
            {
                double timeIntervalCheck = m_bufferResetMultiplier * sendInterval;

                if (clientBuffer.Count > 0 && clientBuffer.Values[clientBuffer.Count - 1].remoteTimestamp + timeIntervalCheck < timestamp)
                {
                    Reset();
                }
            }
#endif
            // position, rotation, scale can have no value if same as last time.
            // saves bandwidth.
            // but we still need to feed it to snapshot interpolation. we can't
            // just have gaps in there if nothing has changed. for example, if
            //   client sends snapshot at t=0
            //   client sends nothing for 10s because not moved
            //   client sends snapshot at t=10
            // then the server would assume that it's one super slow move and
            // replay it for 10 seconds.
            if (!position.HasValue)
            {
                position = clientBuffer.Count > 0 ?
                    clientBuffer.Values[clientBuffer.Count - 1].position :
                    targetComponent.localPosition;
            }
            if (!rotation.HasValue)
            {
                rotation = clientBuffer.Count > 0 ?
                      clientBuffer.Values[clientBuffer.Count - 1].rotation :
                      targetComponent.localRotation;
            }
            if (!scale.HasValue)
            {
                scale = clientBuffer.Count > 0 ?
                    clientBuffer.Values[clientBuffer.Count - 1].scale :
                    targetComponent.localScale;
            }

            // construct snapshot with batch timestamp to save bandwidth
            NTSnapshot snapshot = new NTSnapshot(
                timestamp,
                NetworkTime.localTime,
                position.Value, rotation.Value, scale.Value
            );

            // add to buffer (or drop if older than first element)
            SnapshotInterpolation.InsertIfNewEnough(snapshot, clientBuffer);
        }
        // apply a snapshot to the Transform.
        // -> start, end, interpolated are all passed in caes they are needed
        // -> a regular game would apply the 'interpolated' snapshot
        // -> a board game might want to jump to 'goal' directly
        // (it's easier to always interpolate and then apply selectively,
        //  instead of manually interpolating x, y, z, ... depending on flags)
        // => internal for testing
        //
        // NOTE: stuck detection is unnecessary here.
        //       we always set transform.position anyway, we can't get stuck.
        protected virtual void ApplySnapshot(NTSnapshot start, NTSnapshot goal,
            NTSnapshot interpolated)
        {
            // local position/rotation for VR support
            //
            // if syncPosition/Rotation/Scale is disabled then we received nulls
            // -> current position/rotation/scale would've been added as snapshot
            // -> we still interpolated
            // -> but simply don't apply it. if the user doesn't want to sync
            //    scale, then we should not touch scale etc.
            if (syncPosition)
            {
                targetComponent.localPosition = interpolatePosition ?
                    interpolated.position : goal.position;
            }

            if (syncRotation)
            {
                targetComponent.localRotation = interpolateRotation ?
                    interpolated.rotation : goal.rotation;
            }

            if (syncScale)
            {
                targetComponent.localScale = interpolateScale ?
                    interpolated.scale : goal.scale;
            }
        }
    }

    [Serializable]
    public class NTDCClientSyncData
    {
        private UncertainTransformData m_transData = null;
        private string m_compGuid = "";

        public UncertainTransformData transData => m_transData;
        public string compGuid => m_compGuid;


        public NTDCClientSyncData(UncertainTransformData transformData,
            string componentGuid)
        {
            m_transData = transformData;
            m_compGuid = componentGuid;
        }
    }
}
