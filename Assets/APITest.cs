using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class APITest : MonoBehaviour
{
    public TMP_InputField responseText;
    string moneyAPI = "http://localhost:5000/api/money";
    string loginAPI = "http://localhost:5000/api/login";
    string token;

    TokenResponse tokenResponse;
    MoneyData moneyData;

    public void ShowMoney()
    {
        StartCoroutine(showMoney(moneyAPI));
    }

    private IEnumerator showMoney(string api)
    {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        WWW request = new WWW(api, null, headers);
        yield return request;
        moneyData = JsonUtility.FromJson<MoneyData>(request.text);
        responseText.text = "You have "+moneyData.money.ToString()+" money.";
    }

    public void LogOut()
    {
        token = "";
        responseText.text = "Logged out.";
    }

    public void MakeMoney()
    {
        StartCoroutine(makeMoney());
    }

    public void LogIn()
    {
        StartCoroutine(logIn());
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
        using (UnityWebRequest www = UnityWebRequest.Post(loginAPI, form))
        {
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