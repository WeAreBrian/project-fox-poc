using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InputDialog : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_NameInputField;
    [SerializeField]
    private TMP_InputField m_ContactInputField;
    [SerializeField]
    private GameObject m_BlurVolume;
    [SerializeField]
    private Leaderboard m_Leaderboard;

    private PlayerInputActions m_PlayerInputActions;
    private CloseOrOpenCircle m_HoleTransition;
    private bool m_DetailsAreSubmitted = false;

    private void Awake()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_PlayerInputActions = new PlayerInputActions();
        m_PlayerInputActions.UI.Enable();

        m_PlayerInputActions.UI.Submit.performed += OnSubmit;
        m_PlayerInputActions.UI.ExitLeaderBoard.performed += OnReturnToMain;
    }

    private void Start()
    {
        m_BlurVolume.SetActive(false);
        transform.localScale = Vector3.zero;
    }

    public void ActivateDialog()
    {
        m_BlurVolume.SetActive(true);
        LeanTween.scale(gameObject, Vector3.one, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic)
            .setDelay(0.2f);
        m_NameInputField.ActivateInputField();
    }

    private void DeactivateDialog()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic)
            .setDelay(0.2f)
            .setOnComplete(() => {
                m_BlurVolume.SetActive(false);
                LeanTween.delayedCall(2f, () => m_Leaderboard.MainMenuText.SetActive(true));
                m_DetailsAreSubmitted = true;
            });
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        SaveUtils.RecordPlayerName(m_NameInputField.text);
        SaveUtils.RecordPlayerContact(m_ContactInputField.text);
        SaveUtils.SaveProfile();
        m_Leaderboard.UpdateProfileName();
        DeactivateDialog();
    }

    // This is assgned to jump instead of submit because of the action map swap
    // Also makes it easier to avoid accidental return to main screen before inputting details
    private void OnReturnToMain(InputAction.CallbackContext context)
    {
        if (!m_DetailsAreSubmitted) return;
        SaveUtils.InitializeProfile();
        StartCoroutine(m_HoleTransition.ShrinkParentObject(1));

        // Switch back to normal controls preparing for a new game
        m_PlayerInputActions.Player.Enable();
        m_PlayerInputActions.UI.Disable();
    }
}
