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
    string moneyAPI = "http://localhost:5000/api/money";
    string loginAPI = "http://localhost:5000/api/login";
    string token;
    public TMP_InputField nameField;
    public TMP_InputField passwordField;

    TokenResponse tokenResponse;
    MoneyData moneyData;


    public void LogOut()
    {
        token = "";
        responseText.text = "Logged out.";
    }

    public void MakeMoney()
    {
        StartCoroutine(makeMoney());
    }

    string GetToken()
    {
        string theToken = "Bearer " + token;
        return theToken;
    }

    public IEnumerator makeMoney()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(moneyAPI, form))
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
                    moneyData = JsonUtility.FromJson<MoneyData>(www.downloadHandler.text);
                    responseText.text = "You made 1 money!\nYou now have " + moneyData.money.ToString() + " money.";
                }
            }
        }
    }

    public IEnumerator logIn()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", "name");
        form.AddField("password", "123");
        //List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        //form.Add(new MultipartFormDataSection("name=foo&password=bar"));
        UnityWebRequest www = UnityWebRequest.Post(loginAPI, form);
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            responseText.text = www.error;
        }
        else
        {
            tokenResponse = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
            token = tokenResponse.token;
            responseText.text = "Logged in!";
        }
    }

    public void LogIn()
    {
        UDPLogIn(nameField.text, passwordField.text);
    }

    public void UDPLogIn(string name, string password)
    {
        string response = SendAndGetUDP("login " + name + " " + password);
        Debug.Log("101: " + response);
        if(response.Length>15)
        {
            token = response;
            responseText.text = "Welcome back, " + name+"!";
        }
        else
        {
            responseText.text = response;
        }
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

    void UDPTest()
    {
        try
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes("Unity says hello");
            client.Send(sendBytes, sendBytes.Length);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message from server: " + receivedString);
        }
        catch(Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }


    private class TokenResponse
    {
        public string token;
    }

    private class MoneyData
    {
        public double money;
    }

}