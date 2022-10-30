#if UNITY_EDITOR

using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;


public class MenuInputTest : InputTestFixture
{
    private bool sceneLoaded = false;

    [UnitySetUp]
    public IEnumerator MenuInputSetUp()
    {
        // Setup and load the appropriate scene
        yield return new EnterPlayMode();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("PlayerJoin_SCENE", LoadSceneMode.Single);
    }

    // Helper function to detect when the scene is done loading
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    [UnityTest]
    public IEnumerator MenuInputTestJoin()
    {
        // Wait for scene to be loaded and ready
        yield return new WaitWhile(() => sceneLoaded == false);

        // Simulate connecting two gamepads
        var gamepad1 = InputSystem.AddDevice<Gamepad>("Player 1 Gamepad");
        var gamepad2 = InputSystem.AddDevice<Gamepad>("Player 2 Gamepad");
        yield return new WaitForSeconds(1.0f);

        // Press the 'A' buttons to connect
        PressAndRelease(gamepad1.buttonSouth);
        yield return new WaitForSeconds(0.5f);
        PressAndRelease(gamepad1.dpad.down);
        yield return new WaitForSeconds(0.5f);
        PressAndRelease(gamepad1.dpad.up);
        yield return new WaitForSeconds(0.5f);

        PressAndRelease(gamepad2.buttonSouth);
        yield return new WaitForSeconds(2.0f);
        PressAndRelease(gamepad2.dpad.down);
        yield return new WaitForSeconds(0.5f);
        PressAndRelease(gamepad2.dpad.up);
        yield return new WaitForSeconds(0.5f);
    }

    [UnityTearDown]
    public new IEnumerator TearDown() 
    {
        yield return new ExitPlayMode();
        base.TearDown();
    }
}

#endif // UNITY_EDITOR
