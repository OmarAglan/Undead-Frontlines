using UnityEngine;
using TMPro; // Use TextMeshPro for UI elements
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public Button loginButton;

    private void Start()
    {
        // Add listeners to the buttons to call our methods when clicked
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private async void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        Debug.Log($"Attempting to register with: {username}, {email}");
        var response = await ApiManager.Instance.Register(email, password, username);

        if (response != null && response.success)
        {
            Debug.Log("Registration Successful!");
            // You could automatically log them in here or show a "Check your email" message
        }
        else
        {
            Debug.LogError("Registration Failed!");
        }
    }

    private async void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        Debug.Log($"Attempting to login with: {email}");
        var response = await ApiManager.Instance.Login(email, password);

        if (response != null && response.success)
        {
            Debug.Log("Login Successful! Ready to proceed to character select.");
            // Here you would load the next scene, e.g., SceneManager.LoadScene("CharacterSelect");
        }
        else
        {
            Debug.LogError("Login Failed!");
        }
    }
}