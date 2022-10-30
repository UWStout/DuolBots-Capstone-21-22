// Using statements at the very top of the program.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Author - AuthorName
// This is not for 'ownership' purposes, but so that other programmers know who
// to start asking if they have questions about this scripts.

/// <summary>
/// Description of the class and what it does.
/// Pre Conditions: ~~~~~ (if applicable)
/// Post Conditions: ~~~~~ (if applicable)
/// </summary>
public class StyleGuide : MonoBehaviour
{
    // 1) Constants should go at the very top of the class.
    // Constants should use all capital letters.
    public const float MY_CONSTANT = 3.14159f;
    // Since you cannot have const variables for structs, we can use readonly static
    // and treat them like constants.
    public readonly static Vector3 MY_READONLY_STATIC_VAR = new Vector3(2.0f, 1.0f, 7.0f);
    // Private constants should go below public constants
    private const bool IS_DEBUGGING = false;

    // 2) After constants, should go any static variables.
    // Prefix static variables with an s and an underscore?
    private static int s_myPrivateStaticVariable = 0;
    public static int s_myPublicStaticVariable = 0;

    // 3) After static variables, we declare member variables and properties.
    // SerializeFields should be above other variables.
    // Private member variables should being with an underscore (_).
    [SerializeField] private float m_mySerializedField = 0.0f;

    private int m_memberVariable1 = 0;
    private Rigidbody m_memberVariable2 = null;

    // Properties should be camelCase with no underscore (_).
    public string myAutoProperty { get; private set; }

    public float myControlledProperty => m_controlledMemberVariable;
    private float m_controlledMemberVariable = 0;


    // At least 2 spaces separate the member variables from the functions.
    // 4) Functions.
    // 4a) The 1st group of functions should be unity message functions (Awake, Start, Update, etc.)
    // If any function groups get rather large, you can wrap them in a region for easy collapsing
    #region UnityMessages
    private void Awake()
    {
        // In awake "Domestic Initialization" should be done.
        // Any initialization/setting of variables that doesn't rely on other scripts.
        m_memberVariable1 = 0;
        Rigidbody temp_rigidbody = GetComponent<Rigidbody>();
        m_memberVariable2 = temp_rigidbody;
    }
    private void Start()
    {
        // In start "Foreign Initialization" should be done.
        // Any initialization/setting of variables that does rely on other scripts.
        m_controlledMemberVariable = m_memberVariable2.mass;
    }
    private void Update()
    {
        // Try to use update calls as little as possible, using it only for things that actually
        // need to be active for every frame they are in the game.
        // For things that only need to be active for a few frames after some event, we should use Coroutines.
    }
    #endregion UnityMessages


    // At least 2 spaces separate the unity message functions from the public functions.
    // 4b) The 2nd group of functions should be public functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    /// <param name="param1">Description for what the parameter is used for.</param>
    /// <param name="param2">Description for what the parameter is used for.</param>
    public void MyPublicFunction(float param1, float param2)
    {

    }
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    /// <param name="param">Description for what the parameter is used for.</param>
    /// <returns>What the output of the function can be.</returns>
    public Vector2 MyOtherPublicFunction(float param)
    {
        return new Vector2(param, param);
    }


    // At least 2 spaces separate the public functions from the protected functions.
    // 4c) The 3rd group of functions should be protected functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    protected void MyProtectedFunction()
    {

    }


    // At least 2 spaces separate the protected functions from the private functions.
    // 4d) The 4th group of functions should be private functions.
    /// <summary>
    /// Description of the function.
    /// Pre Conditions: ~~~~~
    /// Post Conditions: ~~~~~
    /// </summary>
    private void MyPrivateFunction()
    {
        int[] temp_imaginaryList = { 0, 1, 2, 3, 4 };
        int temp_currentIndex = 0;
        // This is a comment explaining the while loop and what it does
        while (temp_currentIndex < temp_imaginaryList.Length)
        {
            int temp_currentVariable = temp_imaginaryList[temp_currentIndex];
            ++temp_currentIndex;
        }
    }


    #region Debugging
    // At least 2 spaces separate the private functions from the debug/test functions.
    // 4e) The 5th group of functions should be private functions used only for debugging.
    // They should all be prefixed with 'Debug.'
    private void DebugMyFunction()
    {
        CustomDebug.Log($"StyleGuide's Debug. I am debugging some unused variables to get" +
            $" rid of warnings in the console. {s_myPrivateStaticVariable}" +
            $" {m_mySerializedField} {m_memberVariable1}", IS_DEBUGGING);
    }
    #endregion Debugging
}

/// <summary>
/// Interfaces have a summary as well.
/// They also have the suffix I.
/// </summary>
public interface IMyInterface
{

}


// Naming Conventions and my reasons for the ones in here:
// Want to make all names different to easy tell scope
// and what it is (function/temp variable/member variable/property)

// ClassName - PascalCase to match C# standards.
// Interfaces - I prefix
// CONSTANTS - Standard Capitalization.
// Functions() - PascalCase to match C# standards.
// memberProperties - LowerCamelCase to match Unity's property naming conventions (ex: transform, gameObject).
//      I'm unsure if this is best thing for properties because we like to make temp variables and parameters
//      with this same naming convention in functions and such.
// m_privateMemberVariables - Underscore lowerCamelCase to tell these are private members.
// s_staticVariables - s_ and lowerCamelCase to tell the variable is static (I'm really unsure of this one).
// temp_tempVariables - temp_ and lowerCamelCase variables that are created inside functions.
