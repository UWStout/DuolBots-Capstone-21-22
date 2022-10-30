using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// This allows for the server to call ANY function on any descendent of this
    /// GameObject as if they were a ClientRpc. This is necessary to do because
    /// we have NetworkChild prefabs (parts) that need to relay data over the
    /// network. Like setting a preview active. However those objects have
    /// no way to call ClientRpcs since they themselves are not NetworkBehaviours.
    /// The other alternative to this system is to put a new script on the bot root
    /// prefab for each separate part prefab anytime we need to call a ClientRpc.
    /// </summary>
    public class NetworkMessenger : NetworkBehaviour
    {
        /// <summary>
        /// Requests that a function be called on all clients.
        ///
        /// Pre Conditions - Assumes that the specified GameObject is a descendent
        /// of this GameObject. Assumes that there is at least one function with the
        /// specified name in a component attached to the specified GameObject.
        /// Post Conditions - Tells all the clients to call this function on the
        /// specified GameObject.
        /// </summary>
        /// <param name="receiverObj">GameObject that the function should be
        /// called on.</param>
        /// <param name="funcName">Name of the function that should be
        /// called.</param>
        [Server]
        public void SendMessageToClient(GameObject receiverObj, string funcName)
        {
            SendMessageToClient(receiverObj.transform, funcName);
        }
        [Server]
        public void SendMessageToClient(GameObject receiverObj, string funcName,
            object param)
        {
            SendMessageToClient(receiverObj.transform, funcName, param);
        }
        [Server]
        public void SendMessageToClientUnreliable(GameObject receiverObj,
            string funcName, object param)
        {
            SendMessageToClientUnreliable(receiverObj.transform, funcName, param);
        }
        /// <summary>
        /// Requests that a function be called on all clients.
        ///
        /// Pre Conditions - Assumes that the specified Transform is a descendent
        /// of this GameObject. Assumes that there is at least one function with the
        /// specified name in a component attached to the specified Transform's
        /// GameObject.
        /// Post Conditions - Tells all the clients to call this function on the
        /// specified Transform's GameObject.
        /// </summary>
        /// <param name="receiverTrans">Transform whose GameObject that the function
        /// should be called on.</param>
        /// <param name="funcName">Name of the function that should be
        /// called.</param>
        [Server]
        public void SendMessageToClient(Transform receiverTrans, string funcName)
        {
            // Convert the transform to a TransformChildPath so that it may be
            // sent over the network. This assumes the transform is a descendent of
            // this GameObject.
            TransformChildPath temp_pathToReceiver = new TransformChildPath(
                transform, receiverTrans);

            // Have the clients call the function.
            RelayMessageClientRpc(temp_pathToReceiver, funcName);
        }
        [Server]
        public void SendMessageToClient(Transform receiverTrans, string funcName,
            object param)
        {
            // Convert the transform to a TransformChildPath so that it may be
            // sent over the network. This assumes the transform is a descendent of
            // this GameObject.
            TransformChildPath temp_pathToReceiver = new TransformChildPath(
                transform, receiverTrans);

            // Have the clients call the function.
            RelayMessageWithParamClientRpc(temp_pathToReceiver, funcName,
                param.ToByteArray());
        }
        [Server]
        public void SendMessageToClientUnreliable(Transform receiverTrans,
            string funcName, object param)
        {
            // Convert the transform to a TransformChildPath so that it may be
            // sent over the network. This assumes the transform is a descendent of
            // this GameObject.
            TransformChildPath temp_pathToReceiver = new TransformChildPath(
                transform, receiverTrans);

            // Have the clients call the function.
            RelayMessageWithParamClientRpcUnreliable(temp_pathToReceiver, funcName,
                param.ToByteArray());
        }


        /// <summary>
        /// Has the GameObject at the end of the path call the function with the
        /// given name using SendMessage.
        ///
        /// Pre Conditions - Assumes that the specified TransformChildPath is valid.
        /// Assumes that there is at least one function with the specified name in a
        /// component attached to the GameObject at the end of the path.
        /// Post Conditions - Tells all the clients to call this function on the
        /// GameObject at the end of the specified path.
        /// </summary>
        /// <param name="path">Path to the GameObject that the function should be
        /// called on.</param>
        /// <param name="funcName">Name of the function that should be
        /// called.</param>
        [ClientRpc]
        private void RelayMessageClientRpc(TransformChildPath path, string funcName)
        {
            Transform temp_receiverTrans = path.Traverse(transform);
            temp_receiverTrans.gameObject.SendMessage(funcName,
                SendMessageOptions.RequireReceiver);
        }
        [ClientRpc]
        private void RelayMessageWithParamClientRpc(TransformChildPath path,
            string funcName, byte[] paramData)
        {
            Transform temp_receiverTrans = path.Traverse(transform);
            temp_receiverTrans.gameObject.SendMessage(funcName,
                paramData.ToObject(), SendMessageOptions.RequireReceiver);
        }
        [ClientRpc(channel = Channels.Unreliable)]
        private void RelayMessageWithParamClientRpcUnreliable(
            TransformChildPath path, string funcName, byte[] paramData)
        {
            Transform temp_receiverTrans = path.Traverse(transform);
            temp_receiverTrans.gameObject.SendMessage(funcName,
                paramData.ToObject(), SendMessageOptions.RequireReceiver);
        }
    }
}
