using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;
    public InputField NicknameInput;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ValueText, PlayersText, ClickUpgradeText, AutoUpgradeText,
        ValuePerClickText, ValuePerSecondText;
    public Button ClickUpgradeBtn, AutoUpgradeBtn;

    float nextTime;

    void Start()
    {
        //on tutorial
        //Screen.SetResolution(540, 960, false);
        //for PC
        Screen.SetResolution(1920,1080,false);

    }

    public void Connect()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnLeftRoom() //for leftRoom state, show other panel
    {
        ShowPanel(DisconnectPanel);
    }
     
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 5 }, null);

    }
    public override void OnJoinedRoom()
    {
        ShowPanel(RoomPanel);
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    void ShowPanel(GameObject CurPanel)
    {
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);
        CurPanel.SetActive(true);
    }

    PlayerScript FindPlayer()
    {
        foreach(GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (Player.GetPhotonView().IsMine) return Player.GetComponent<PlayerScript>();
        }
        return null;
    }

    public void Click()
    {
        PlayerScript Player = FindPlayer();
        Player.value += Player.valuePerClick;
    }

    public void ClickUpgrade()
    {
        PlayerScript Player = FindPlayer();

        if(Player.value >= Player.clickUpgradeCost)
        {
            Player.value -= Player.clickUpgradeCost;
            Player.valuePerClick += Player.clickUpgradeAdd;
            Player.clickUpgradeCost += Player.clickUpgradeAdd * 10;
            Player.clickUpgradeAdd += 2;

            ClickUpgradeText.text = Player.clickUpgradeAdd + " / click" + "\n" + "Cost : "
                + Player.clickUpgradeCost;

            ValuePerClickText.text = Player.valuePerClick.ToString();
        }
    }

    public void AutoUpgrade()
    {
        PlayerScript Player = FindPlayer();

        if (Player.value >= Player.autoUpgradeCost)
        {
            Player.value -= Player.autoUpgradeCost;
            Player.valuePerSecond += Player.autoUpgradeAdd;
            Player.autoUpgradeCost += 500;
            Player.autoUpgradeAdd += 2;

            AutoUpgradeText.text = Player.autoUpgradeAdd + " / sec" + "\n" + "Cost : "
                + Player.autoUpgradeCost;

            ValuePerSecondText.text = Player.valuePerSecond.ToString();
        }
    }

    void ShowPlayers()
    {
        string playersText = "";

        foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playersText += Player.GetPhotonView().Owner.NickName + " / " +
                Player.GetComponent<PlayerScript>().value.ToString() + "\n";
        }
        PlayersText.text = playersText;
    }

    void EnableUpgrade()
    {
        PlayerScript Player = FindPlayer();

        ClickUpgradeBtn.interactable = Player.value >= Player.clickUpgradeCost;
        AutoUpgradeBtn.interactable = Player.value >= Player.autoUpgradeCost;
    }

    void ValuePerSecond()
    {
        PlayerScript Player = FindPlayer();
        Player.value += Player.valuePerSecond;
    }
    void Update()
    {
        if (!PhotonNetwork.InRoom) return;

        ShowPlayers();
        EnableUpgrade();

        if(Time.time > nextTime)
        {
            nextTime = Time.time + 1;
            ValuePerSecond();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			LeaveRoom();
		}

    }

/*
    public void QuitApplication()
    {
        Application.CancelQuit();
#if !UNITY_EDITOR
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
*/ //only if there is an application
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect(); //without disconnect -> auto entrance
        /*
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);*/ //Reload scene
    }

}
