using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

// This class will handle all communication with our backend API.
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    private const string BaseUrl = "http://localhost:4000/api"; // Your backend URL
    private string jwtToken;

    // --- Data Structures for API Responses ---
    [System.Serializable]
    public class AuthResponse
    {
        public bool success;
        public string token;
        public string refreshToken;
        public string userId;
        public string message;
    }

    private void Awake()
    {
        // Singleton pattern to ensure only one ApiManager exists.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Public Methods for Authentication ---

    public async Task<AuthResponse> Register(string email, string password, string username)
    {
        var body = new { email, password, username };
        string jsonBody = JsonConvert.SerializeObject(body);
        return await PostRequest<AuthResponse>("/auth/register", jsonBody);
    }

    public async Task<AuthResponse> Login(string email, string password)
    {
        var body = new { email, password };
        string jsonBody = JsonConvert.SerializeObject(body);

        AuthResponse response = await PostRequest<AuthResponse>("/auth/login", jsonBody);

        // If login is successful, store the token
        if (response != null && response.success)
        {
            jwtToken = response.token;
            Debug.Log("Login successful! Token stored.");
        }

        return response;
    }

    // --- Generic Request Helper ---

    private async Task<T> PostRequest<T>(string endpoint, string jsonBody) where T : class
    {
        string url = BaseUrl + endpoint;
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Add authorization header if we have a token
            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            }

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Success Response from {endpoint}: {jsonResponse}");
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            else
            {
                Debug.LogError($"Error on {endpoint}: {request.error} - {request.downloadHandler.text}");
                // You can deserialize the error response here as well if needed
                return null;
            }
        }
    }
}