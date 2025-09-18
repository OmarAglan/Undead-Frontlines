using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;

namespace UndeadFrontlines.Network
{
    public class NetworkUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startServerButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private TextMeshProUGUI statusText;
        
        [Header("Network")]
        [SerializeField] private NetworkManager networkManager;
        
        private void Awake()
        {
            if (networkManager == null)
                networkManager = FindObjectOfType<NetworkManager>();
                
            ValidateComponents();
        }
        
        private void OnEnable()
        {
            // Subscribe to button clicks
            startServerButton.onClick.AddListener(StartServer);
            startClientButton.onClick.AddListener(StartClient);
            stopButton.onClick.AddListener(StopConnection);
            
            // Subscribe to FishNet events
            if (networkManager != null)
            {
                networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
                networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
            }
            
            UpdateButtonStates();
        }
        
        private void OnDisable()
        {
            // Unsubscribe from button clicks
            startServerButton.onClick.RemoveListener(StartServer);
            startClientButton.onClick.RemoveListener(StartClient);
            stopButton.onClick.RemoveListener(StopConnection);
            
            // Unsubscribe from FishNet events
            if (networkManager != null)
            {
                networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
                networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            }
        }
        
        private void StartServer()
        {
            if (networkManager.ServerManager.Started)
            {
                Debug.LogWarning("Server already started!");
                return;
            }
            
            networkManager.ServerManager.StartConnection();
            UpdateStatus("Starting server...");
        }
        
        private void StartClient()
        {
            if (networkManager.ClientManager.Started)
            {
                Debug.LogWarning("Client already connected!");
                return;
            }
            
            networkManager.ClientManager.StartConnection();
            UpdateStatus("Connecting to server...");
        }
        
        private void StopConnection()
        {
            if (networkManager.ServerManager.Started)
            {
                networkManager.ServerManager.StopConnection(true);
            }
            
            if (networkManager.ClientManager.Started)
            {
                networkManager.ClientManager.StopConnection();
            }
            
            UpdateStatus("Disconnected");
        }
        
        private void OnServerConnectionState(ServerConnectionStateArgs args)
        {
            string message = args.ConnectionState switch
            {
                LocalConnectionState.Starting => "Server starting...",
                LocalConnectionState.Started => $"Server running on port {GetPort()}",
                LocalConnectionState.Stopping => "Server stopping...",
                LocalConnectionState.Stopped => "Server stopped",
                _ => "Unknown server state"
            };
            
            UpdateStatus(message);
            UpdateButtonStates();
        }
        
        private void OnClientConnectionState(ClientConnectionStateArgs args)
        {
            string message = args.ConnectionState switch
            {
                LocalConnectionState.Starting => "Client connecting...",
                LocalConnectionState.Started => "Client connected!",
                LocalConnectionState.Stopping => "Client disconnecting...",
                LocalConnectionState.Stopped => "Client disconnected",
                _ => "Unknown client state"
            };
            
            UpdateStatus(message);
            UpdateButtonStates();
        }
        
        private void UpdateButtonStates()
        {
            bool isServerRunning = networkManager.ServerManager.Started;
            bool isClientConnected = networkManager.ClientManager.Started;
            bool isAnyConnectionActive = isServerRunning || isClientConnected;
            
            startServerButton.interactable = !isServerRunning;
            startClientButton.interactable = !isClientConnected;
            stopButton.interactable = isAnyConnectionActive;
        }
        
        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = $"Status: {message}";
                Debug.Log($"[NetworkUI] {message}");
            }
        }
        
        private int GetPort()
        {
            var tugboat = networkManager.TransportManager.Transport as FishNet.Transporting.Tugboat.Tugboat;
            return tugboat != null ? tugboat.GetPort() : 7770;
        }
        
        private void ValidateComponents()
        {
            if (networkManager == null)
                Debug.LogError("NetworkManager not found! Please assign it in the Inspector.");
                
            if (startServerButton == null || startClientButton == null || stopButton == null)
                Debug.LogWarning("Some UI buttons are not assigned in the Inspector.");
        }
    }
}