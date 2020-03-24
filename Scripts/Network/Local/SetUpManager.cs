using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetUpManager : MonoBehaviour
{
    public Button btnSet, btnSure;
    public InputField serverIPInput;
    public GameObject panelSet;

    private string serverIP;
    private const string ServerKey = "ServerIP";
    // Use this for initialization
    void Start()
    {       
        if (PlayerPrefs.HasKey(ServerKey))
        {
            serverIP = PlayerPrefs.GetString(ServerKey);
            NetController.instance.ip = serverIP;
        }
        NetController.instance.Connect();

        btnSet.onClick.AddListener(OnBtnSetClick);
        
        btnSure.onClick.AddListener(OnBtnSureClick);
        panelSet.SetActive(false);
    }

    private void OnBtnSetClick()
    {
        panelSet.gameObject.SetActive(true);

        serverIPInput.text = PlayerPrefs.GetString(ServerKey, "192.168.0.132");
    }

    private void OnBtnSureClick()
    {
        if (!string.IsNullOrEmpty(serverIPInput.text))
        {
            serverIP = serverIPInput.text;
            PlayerPrefs.SetString(ServerKey, serverIP);
            NetController.instance.ip = serverIP;
            NetController.instance.DisConnect();
            StartCoroutine(ReConnect());
        }
        PlayerPrefs.Save();

        panelSet.gameObject.SetActive(false);
    }

    IEnumerator ReConnect()
    {
        yield return new WaitForSeconds(1);
        NetController.instance.Connect();
    }
}
