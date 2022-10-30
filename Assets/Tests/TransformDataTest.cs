using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// Original Authors - Wyatt Senalik

namespace DuolBots.Tests
{
    public class TransformDataTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void DefaultConstructorTest()
        {
            TransformData temp_transData = new TransformData();
            // Use the Assert class to test conditions
            // Default pos is 0.
            ExtraAsserts.AreClose(Vector3.zero, temp_transData.position);
            // Default rot is identity.
            ExtraAsserts.AreClose(Quaternion.identity, temp_transData.rotation);
            // Default scale is 1.
            ExtraAsserts.AreClose(Vector3.one, temp_transData.scale);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void FullConstructorTest()
        {
            Vector3 temp_pos = new Vector3(10, 723.2f, -238.5f);
            Quaternion temp_rot = new Quaternion(-10.2f, 7, 990.2f, 3);
            Vector3 temp_scale = new Vector3(9, 18, -27.27f);

            TransformData temp_transData = new TransformData(temp_pos, temp_rot,
                temp_scale);
            // Use the Assert class to test conditions
            ExtraAsserts.AreClose(temp_pos, temp_transData.position);
            ExtraAsserts.AreClose(temp_rot, temp_transData.rotation);
            ExtraAsserts.AreClose(temp_scale, temp_transData.scale);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void CreateGlobalTransformDataTest()
        {
            Vector3 temp_pos = new Vector3(-10, 78, 7);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 90).
            Quaternion temp_rot = Quaternion.Euler(24.21f, 87.07f, 16.34f);
            Vector3 temp_scale = new Vector3(99, 23, 10);

            Transform temp_transform = new GameObject().transform;
            temp_transform.position = temp_pos;
            temp_transform.rotation = temp_rot;
            temp_transform.localScale = temp_scale;
            TransformData temp_transData = TransformData.CreateGlobalTransformData(
                temp_transform);
            // Use the Assert class to test conditions
            ExtraAsserts.AreClose(temp_pos, temp_transData.position);
            ExtraAsserts.AreClose(temp_rot, temp_transData.rotation);
            ExtraAsserts.AreClose(temp_scale, temp_transData.scale);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void CreateGlobalTransformDataWithParentTest()
        {
            Vector3 temp_parentPos = new Vector3(-10, 7, 7);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 45).
            // This subset is half of 90 to ensure that the global rotation
            // does not exceed 90.
            Quaternion temp_parentRot = Quaternion.Euler(35.62f, 29.24f, 33.73f);
            Vector3 temp_parentScale = new Vector3(99, 23, 10.2f);

            Transform temp_parent = new GameObject().transform;
            temp_parent.position = temp_parentPos;
            temp_parent.rotation = temp_parentRot;
            temp_parent.localScale = temp_parentScale;

            Vector3 temp_pos = new Vector3(3, -10, 1);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 45).
            // This subset is half of 90 to ensure that the global rotation
            // does not exceed 90.
            Quaternion temp_rot = Quaternion.Euler(10.98f, 1.55f, 17.22f);
            Vector3 temp_scale = new Vector3(2, 3, 4);

            Transform temp_transform = new GameObject().transform;
            temp_transform.SetParent(temp_parent);
            temp_transform.position = temp_pos;
            temp_transform.rotation = temp_rot;
            temp_transform.localScale = temp_scale;

            TransformData temp_transData = TransformData.CreateGlobalTransformData(
                temp_transform);
            // Use the Assert class to test conditions
            ExtraAsserts.AreClose(temp_pos, temp_transData.position);
            ExtraAsserts.AreClose(temp_rot, temp_transData.rotation);
            // Scale should be local scale not world/lossy scale
            ExtraAsserts.AreClose(temp_scale, temp_transData.scale);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void CreateLocalTransformDataTest()
        {
            Vector3 temp_pos = new Vector3(-213, 92, 1.5f);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 90).
            Quaternion temp_rot = Quaternion.Euler(45.69f, 26.14f, 17.19f);
            Vector3 temp_scale = new Vector3(7, 2, 9.2f);

            Transform temp_transform = new GameObject().transform;
            temp_transform.position = temp_pos;
            temp_transform.rotation = temp_rot;
            temp_transform.localScale = temp_scale;
            TransformData temp_transData = TransformData.CreateLocalTransformData(
                temp_transform);
            // Use the Assert class to test conditions
            ExtraAsserts.AreClose(temp_pos, temp_transData.position);
            ExtraAsserts.AreClose(temp_rot, temp_transData.rotation);
            ExtraAsserts.AreClose(temp_scale, temp_transData.scale);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void CreateLocalTransformDataWithParentTest()
        {
            Vector3 temp_parentPos = new Vector3(4, 8, 16);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 45).
            // This subset is half of 90 to ensure that the global rotation
            // does not exceed 90.
            Quaternion temp_parentRot = Quaternion.Euler(14.85f, 23.95f, 11.34f);
            Vector3 temp_parentScale = new Vector3(50, -16, -25);

            Transform temp_parent = new GameObject().transform;
            temp_parent.position = temp_parentPos;
            temp_parent.rotation = temp_parentRot;
            temp_parent.localScale = temp_parentScale;

            Vector3 temp_pos = new Vector3(-63, -81, 59);
            // Unity transforms mess with quaternions to clamp them
            // within a certain range. To ensure no clamping occurs, the
            // numbers are being kept within a subset of that range (0, 45).
            // This subset is half of 90 to ensure that the global rotation
            // does not exceed 90.
            Quaternion temp_rot = Quaternion.Euler(4.79f, 22.67f, 20.10f);
            Vector3 temp_scale = new Vector3(1, 78, 75);

            Transform temp_transform = new GameObject().transform;
            temp_transform.SetParent(temp_parent);
            temp_transform.localPosition = temp_pos;
            temp_transform.localRotation = temp_rot;
            temp_transform.localScale = temp_scale;

            TransformData temp_transData = TransformData.CreateLocalTransformData(
                temp_transform);
            // Use the Assert class to test conditions
            ExtraAsserts.AreClose(temp_pos, temp_transData.position);
            ExtraAsserts.AreClose(temp_rot, temp_transData.rotation);
            ExtraAsserts.AreClose(temp_scale, temp_transData.scale);
        }
    }
}
