/*using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the HUD for skills</summary>
public class SkillHUDManager : MonoBehaviour
{
    // Constants
    private const float FADED_AMOUNT = 0.5f;

    // Name of the skill menu action map in player input
    [SerializeField] private string skillMenuActionMapName = "SkillMenu";

    // Reference to the grid HUD's total parent
    [SerializeField] private Transform gridHUDParent = null;

    // Selection row and column parents
    [SerializeField] private RectTransform colorRow = null;
    [SerializeField] private RectTransform shapeCol = null;

    // Spacing for how the states should be layed out
    [SerializeField] private float verticalSpace = 150f;
    [SerializeField] private float horizontalSpace = 150f;

    // References for the arrows
    [SerializeField] private GameObject upArrow = null;
    [SerializeField] private GameObject downArrow = null;
    [SerializeField] private GameObject leftArrow = null;
    [SerializeField] private GameObject rightArrow = null;

    // References to the images for color and shape
    private Image[] colorImages = new Image[0];
    private Image[] shapeImages = new Image[0];
    private Color[] colorImageStartColors = new Color[0];
    private Color[] shapeImagesStartColors = new Color[0];

    // Indexing information
    public int colorRowIndex { get; set; }
    public int shapeColIndex { get; set; }

    // If the HUD is currently active
    private bool isHUDActive = false;

    // Event to update the skill hud visuals
    public delegate void VisualChange();
    public static event VisualChange OnVisualChange;


    // Called when the script is enabled.
    // Subscribe to events.
    private void OnEnable()
    {
        // Open HUD when the game pauses and close it when it opens
        InputEvents.OpenSkillMenuEvent += OnOpenSkillMenu;
        InputEvents.CloseSkillMenuEvent += OnCloseSkillMenu;
        InputEvents.RevertEvent += OnRevert;
        // Navigate menu
        InputEvents.MainAxisEvent += OnHUDAxis;        

        // Testing
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        InputEvents.HackerAxisEvent += OnHackerAxis;
#endif
    }
    // Called when the script is disabled.
    // Unsubscribe from events.
    private void OnDisable()
    {
        InputEvents.OpenSkillMenuEvent -= OnOpenSkillMenu;
        InputEvents.CloseSkillMenuEvent -= OnCloseSkillMenu;
        InputEvents.RevertEvent -= OnRevert;

        InputEvents.MainAxisEvent -= OnHUDAxis;

        // Testing
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        InputEvents.HackerAxisEvent -= OnHackerAxis;
#endif
    }
    // Called 1st
    // Initialize
    private void Start()
    {
        int amountSkills = SkillController.Instance.GetSkillAmount();

        // Initialize the selection indices
        colorRowIndex = 0;
        shapeColIndex = 0;

        // Get the color and shape images
        colorImages = new Image[colorRow.childCount];
        shapeImages = new Image[shapeCol.childCount];
        colorImageStartColors = new Color[colorRow.childCount];
        shapeImagesStartColors = new Color[shapeCol.childCount];
        try
        {
            for (int i = 0; i < colorRow.childCount; ++i)
            {
                colorImages[i] = colorRow.GetChild(i).GetComponent<Image>();
                colorImageStartColors[i] = colorImages[i].color;
            }
            for (int i = 0; i < shapeCol.childCount; ++i)
            {
                shapeImages[i] = shapeCol.GetChild(i).GetComponent<Image>();
                shapeImagesStartColors[i] = shapeImages[i].color;
            }
        }
        catch
        {
            Debug.LogError("Failed to initialize images");
        }

        // Hide HUD on start
        HUDstatus(false);
    }


    /// <summary>Gets the index of the current state of the selected skill (Color or Shape).</summary>
    /// <param name="skillIndex">Index of the skill to check the state of.</param>
    public int GetStateIndexOfSkill(int skillIndex)
    {
        switch ((SkillController.SkillEnum)skillIndex)
        {
            case SkillController.SkillEnum.Shape:
                return shapeColIndex;
            case SkillController.SkillEnum.Color:
                return colorRowIndex;
            default:
                return -1;
        }
    }

    /// <summary>Called when the player wants to close the skill menu without applying. Pops the input map and closes without applying.</summary>
    public void OnRevert()
    {
        // Revert the input map and close the hud without applying
        InputController.Instance.PopInputMap(skillMenuActionMapName);
        HUDstatus(false);
    }
    /// <summary>Called when the player wants to close the skill menu. Pops the input map and closes the skill menu.</summary>
    public void OnCloseSkillMenu()
    {
        // Revert the input map and close the hud
        InputController.Instance.PopInputMap(skillMenuActionMapName);
        CloseHUD();
        hideCursor();
    }
    /// <summary>Called when the player wants to open the skill menu. Swaps the input map and opens the skill menu.</summary>
    private void OnOpenSkillMenu()
    {
        // Swap input maps and call the open event
        InputController.Instance.SwitchInputMap(skillMenuActionMapName);
        OpenHUD();
        showCursor();
    }
    private void showCursor()
    {
        Cursor.visible = true;
        Screen.lockCursor = false;
    }
    private void hideCursor()
    {
        Cursor.visible = false;
        Screen.lockCursor = true;
    }
    /// <summary>Called when the player uses the HUD navigation</summary>
    private void OnHUDAxis(Vector2 rawInput)
    {
        // Only navigate when active
        if (isHUDActive)
        {
            // Horizontal movement
            if (rawInput.x < 0) { OnColorRowLeft(); }
            else if (rawInput.x > 0) { OnColorRowRight(); }

            // Vertical movement
            if (rawInput.y < 0) { OnShapeColumnDown(); }
            else if (rawInput.y > 0) { OnShapeColumnUp(); }
        }
    }

    ///<summary>Helper method to open HUD.</summary>
    private void OpenHUD()
    {
        HUDstatus(true);
        UpdateVisuals();
    }
    /// <summary>Helper method to close HUD and execute the skill calls.</summary>
    private void CloseHUD()
    {
        HUDstatus(false);
        ExecuteHUD();
    }
    ///<summary>Activates or deactivates HUD</summary>
    private void HUDstatus(bool status)
    {
        // Show HUD
        if (status)
        {
            ActivateAllSets();
        }
        // Hide HUD
        else
        {
            DeactivateAllSets();
        }
        isHUDActive = status;
    }
    /// <summary>Turns off the parents of each of the sets</summary>
    private void DeactivateAllSets()
    {
        gridHUDParent.gameObject.SetActive(false);
    }
    /// <summary>Turns on the unlocked options in each set</summary>
    private void ActivateAllSets()
    {
        gridHUDParent.gameObject.SetActive(true);
        ActivateSingleSet(shapeCol, SkillController.Instance.GetSkill(SkillController.SkillEnum.Shape));
        ActivateSingleSet(colorRow, SkillController.Instance.GetSkill(SkillController.SkillEnum.Color));
    }
    /// <summary>Helper function for ActivateAllSets to turn on all the sets</summary>
    private void ActivateSingleSet(Transform setParent, Skill skillRef)
    {
        int count = 0;
        // Activate the states if they are unlocked
        foreach (Transform child in setParent)
        {
            child.gameObject.SetActive(skillRef.IsStateUnlocked(count));
            ++count;
        }
        // Activate the parent if the skill is unlocked
        setParent.gameObject.SetActive(skillRef.IsSkillUnlocked());
    }

    /// <summary>Uses the skills based on what was selected</summary>
    private void ExecuteHUD()
    {
        // Use the skills
        SkillController.Instance.UseSkill(SkillController.SkillEnum.Color, colorRowIndex);
        SkillController.Instance.UseSkill(SkillController.SkillEnum.Shape, shapeColIndex);

        // Form change skill
        SkillController.Instance.UseSkill(SkillController.SkillEnum.Form, 0);
    }

    /// <summary>Moves the color row right</summary>
    public void OnColorRowLeft()
    {
        if (isHUDActive)
        {
            if (colorRowIndex > 0)
            {
                --colorRowIndex;
                UpdateVisuals();
            }
        }
    }
    /// <summary>Moves the color row left</summary>
    public void OnColorRowRight()
    {
        if (isHUDActive)
        {
            int maxRow = colorRow.childCount;
            // If there is another state to the right
            if (colorRowIndex + 1 < maxRow)
            {
                // If that state is also unlocked
                Skill colorSkill = SkillController.Instance.GetSkill(SkillController.SkillEnum.Color);
                if (colorSkill.IsStateUnlocked(colorRowIndex + 1))
                {
                    ++colorRowIndex;
                    UpdateVisuals();
                }
            }
        }
    }
    /// <summary>Move one column up</summary>
    public void OnShapeColumnUp()
    {
        if (isHUDActive)
        {
            if (shapeColIndex > 0)
            {
                --shapeColIndex;
                UpdateVisuals();
            }            
        }
    }
    /// <summary>Move on column down</summary>
    public void OnShapeColumnDown()
    {
        if (isHUDActive)
        {
            int maxCol = shapeCol.childCount;
            Skill shapeSkill = SkillController.Instance.GetSkill(SkillController.SkillEnum.Shape);
            // If there is another state to the right
            if (shapeColIndex + 1 < maxCol)
            {
                // And that state is unlocked
                if (shapeSkill.IsStateUnlocked(shapeColIndex + 1))
                {
                    ++shapeColIndex;
                    UpdateVisuals();
                }
            }
        }
    }

    /// <summary>Fades out appropriate images and shows the appropriate arrows.
    /// Also calls the OnVisualChange event.</summary>
    private void UpdateVisuals()
    {
        colorRow.anchoredPosition = new Vector2(-colorRowIndex * horizontalSpace, 0);
        shapeCol.anchoredPosition = new Vector2(0, shapeColIndex * verticalSpace);

        UpdateFadeImages();
        UpdateActiveArrows();
        // Call the event for when visuals are updated
        OnVisualChange?.Invoke();
    }

    /// <summary>Fades out all the images and fades the currently selected one back in.</summary>
    private void UpdateFadeImages()
    {
        Color temp;
        // Fade out shapes
        for (int i = 0; i < shapeImages.Length; ++i)
        {
            temp = shapeImagesStartColors[i] * FADED_AMOUNT;
            temp.a = 1f;
            shapeImages[i].color = temp;
        }
        // Fade out colors
        for (int i = 0; i < colorImages.Length; ++i)
        {
            temp = colorImageStartColors[i] * FADED_AMOUNT;
            temp.a = 1f;
            colorImages[i].color = temp;
        }
        // Fade in selected
        colorImages[colorRowIndex].color = colorImageStartColors[colorRowIndex];
    }

    /// <summary>Shows arrows for the directions the player can actually traverse in the menu.</summary>
    private void UpdateActiveArrows()
    {
        Skill shapeSkill = SkillController.Instance.GetSkill(SkillController.SkillEnum.Shape);
        Skill colorSkill = SkillController.Instance.GetSkill(SkillController.SkillEnum.Color);

        upArrow.SetActive(shapeColIndex > 0);
        downArrow.SetActive(shapeColIndex + 1 < shapeSkill.GetAmountStates() && shapeSkill.IsStateUnlocked(shapeColIndex + 1));
        leftArrow.SetActive(colorRowIndex > 0);
        rightArrow.SetActive(colorRowIndex + 1 < colorSkill.GetAmountStates() && colorSkill.IsStateUnlocked(colorRowIndex + 1));
    }

    // For testing
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void OnHackerAxis(Vector2 rawInputVector)
    {
        if (rawInputVector.y < 0)
        {
            Debug.Log("Elite Hacker detected. Unlocking hidden memes.");
            SkillController.Instance.UnlockSkillState(SkillController.SkillEnum.Shape, shapeColIndex + 1);
        }

        if (rawInputVector.x > 0)
        {
            Debug.Log("Elite Hacker detected. Expanding meme library.");
            SkillController.Instance.UnlockSkillState(SkillController.SkillEnum.Color, colorRowIndex + 1);
        }

        // Update the HUD
        ActivateAllSets();
    }
#endif
}*/
