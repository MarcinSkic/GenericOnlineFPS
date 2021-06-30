using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class APITest : MonoBehaviour
{
    public TMP_InputField responseText;
    string addkillAPI = "http://localhost:5000/api/addkill";
    string adddeathAPI = "http://localhost:5000/api/adddeath";
    // string loginAPI = "http://localhost:5000/api/login";
    string mystatsAPI = "http://localhost:5000/api/mystats";
    string statsbynameAPI = "http://localhost:5000/api/statsbyname";
    string token;
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public TMP_InputField targetNameField;

    TokenResponse tokenResponse;
    DeathData deathData;
    KillData killData;
    StatsData statsData;
    StatsData targetStatsData;

    public void LogOut()
    {
        token = "";
        responseText.text = "Logged out.";
    }
    public void AddKill()
    {
        StartCoroutine(addKill());
    }

    public void AddDeath()
    {
        StartCoroutine(addDeath());
    }

    string GetToken()
    {
        string theToken = "Bearer " + token;
        return theToken;
    }

    public IEnumerator addKill()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(addkillAPI, form))
        {
            www.SetRequestHeader("Authorization", GetToken());
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error!");
                responseText.text = "Error!";
            }
            else
            {
                if (www.downloadHandler.text == "\"wrong token!\"")
                {
                    responseText.text = "You need to log in first!";
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    Debug.Log("kills");
                    killData = JsonUtility.FromJson<KillData>(www.downloadHandler.text);
                    responseText.text = "You added 1 kill!\nYou now have " + killData.kills.ToString() + " kills.";
                }
            }
        }
    }

    public IEnumerator addDeath()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(adddeathAPI, form))
        {
            www.SetRequestHeader("Authorization", GetToken());
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error!");
                responseText.text = "Error!";
            }
            else
            {
                if(www.downloadHandler.text=="\"wrong token!\"")
                {
                    responseText.text = "You need to log in first!";
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    deathData = JsonUtility.FromJson<DeathData>(www.downloadHandler.text);
                    responseText.text = "You added 1 death!\nYou now have " + deathData.deaths.ToString() + " deaths.";
                }
            }
        }
    }

    public void DisplayMyStats()
    {
        StartCoroutine(displayMyStats());
    }

    public IEnumerator displayMyStats()
    {

        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Get(mystatsAPI))
        {
            www.SetRequestHeader("Authorization", GetToken());
            yield return www.SendWebRequest();
            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error!");
                responseText.text = "Error!";
            }    
            else
            {
                if (www.downloadHandler.text == "\"wrong token!\"")
                {
                    responseText.text = "You need to log in first!";
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    statsData = JsonUtility.FromJson<StatsData>(www.downloadHandler.text);
                    //responseText.text = statsData.ToString();
                    responseText.text = "Name: " + statsData.name + "\nKills: " + statsData.kills + "\nDeaths: " + statsData.deaths;
                }
            }
        }
    }

    public void DisplayStatsByName()
    {
        StartCoroutine(displayStatsByname());
    }

    public IEnumerator displayStatsByname()
    {

        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Get(statsbynameAPI))
        {
            www.SetRequestHeader("Authorization", GetToken());
            www.SetRequestHeader("Name", targetNameField.text);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error!");
                responseText.text = "Error!";
            }
            else
            {
                if (www.downloadHandler.text == "\"wrong target!\"")
                {
                    responseText.text = "Wrong target!";
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    targetStatsData = JsonUtility.FromJson<StatsData>(www.downloadHandler.text);
                    //responseText.text = targetStatsData.ToString();
                    responseText.text = "Name: " + targetStatsData.name + "\nKills: " + targetStatsData.kills + "\nDeaths: " + targetStatsData.deaths;
                }
            }
        }
    }

    public void LogIn()
    {
        UDPLogIn(nameField.text, passwordField.text);
    }

    public void UDPLogIn(string name, string password)
    {
        string response = SendAndGetUDP("login " + name + " " + password);
        string[] responseparts = response.Split(' ');
        if (response.Length>50 && responseparts[0]=="token:")
        {
            token = responseparts[1];
            responseText.text = "Welcome back, " + name+"!";
        }
        else
        {
            responseText.text = response;
        }
    }

    public void Register()
    {
        UDPRegister(nameField.text, passwordField.text);
    }
    public void UDPRegister(string name, string password)
    {
        string response = SendAndGetUDP("register " + name + " " + password);
        string[] responseparts = response.Split(' ');
        responseText.text = response;
    }

    private UdpClient client;

    public void Start()
    {
        try
        {
            client = new UdpClient(5600);
            client.Connect("127.0.0.1", 5500);
        }
        catch (Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }

    public void Test()
    {
        SendUDP("hello");
    }

    string SendAndGetUDP(string message)
    {
        try
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            client.Send(sendBytes, sendBytes.Length);
            print("Sending: " + message);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message from server: " + receivedString);
            return receivedString;
        }
        catch (Exception e)
        {
            print("Exception thrown " + e.Message);
            return e.Message;
        }
    }

    void SendUDP(string message)
    {
        try
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            client.Send(sendBytes, sendBytes.Length);
            print("Sending: " + message);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message from server: " + receivedString);
        }
        catch (Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }

    private class TokenResponse
    {
        public string token;
    }

    private class DeathData
    {
        public double deaths;
    }

    private class KillData
    {
        public double kills;
    }

    private class Stats
    {
        public double player;
    }
    private class StatsData
    {
        public string name;
        public int kills;
        public int deaths;
    }

}