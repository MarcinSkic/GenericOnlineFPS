using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerServerData
{
    public enum Teams { Red, Blue, Yellow, Green };
    public Teams team;
    public string nick;
}
public class SpawnController : MonoBehaviour
{
    public void Start()
    {
        PlayerServerData[] players = ServerMethod();
    }
    
    public PlayerServerData[] ServerMethod()
    {
        PlayerServerData[] players = new PlayerServerData[5];
        return players;
    }
}
