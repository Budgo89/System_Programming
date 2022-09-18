using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{

    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonShutDownServer;
    [SerializeField] private Button buttonConnectClient;
    [SerializeField] private Button buttonDisconnectClient;
    [SerializeField] private Button buttonSendMessage;
    [SerializeField] private Button buttonNick;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField nickField;
    [SerializeField] private TextField textField;
    [SerializeField] private Server server;
    [SerializeField] private Client client;

    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        buttonNick.onClick.AddListener(() => NicknameChange());
        client.onMessageReceive += ReceiveMessage;
    }

    private void NicknameChange()
    {
        //server.NicknameChange();
        client.SendMessage(nickField.text);
        nickField.text = "";
        nickField.gameObject.SetActive(false);
        buttonNick.gameObject.SetActive(false);
    }

    private void StartServer()
    {
        server.StartServer();
    }

    private void ShutDownServer()
    {
        server.ShutDownServer();
    }

    private void Connect()
    {
        client.Connect();
    }

    private void Disconnect()
    {
        client.Disconnect();
    }

    private void SendMessage()
    {
        client.SendMessage(inputField.text);
        inputField.text = "";
    }

    public void ReceiveMessage(object message)
    {
        textField.ReceiveMessage(message);
    }

}
