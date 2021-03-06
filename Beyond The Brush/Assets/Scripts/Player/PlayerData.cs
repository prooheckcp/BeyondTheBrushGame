﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerData : MonoBehaviour
{
    //Variables||

        //Instance
        static public PlayerData instance;

        private void Awake()
        {
            instance = this;
        }

    //Database data
    static private string _id = null;
        static private string _username = "Offline Player";
        static private int _level = 1;
        static private int _exp = 0;
        static private int _resources = 0;
        static private int _gold = 0;
        static private string _email;

        //local data
        static private int _healthPoints = 100;
        static private int _maxHealthPoints = 100;

        //Cooldowns
        static public float slashCooldownDefault = 0.5f;
        static public float xslashCooldownDefault = 5f;
        static public float shieldCooldownDefault = 6f;

        private class cooldowsClass{
            public float _slashCooldown = 0;
            public float _xslashCooldown = 0;
            public float _shieldCooldown = 0;
        }

        static private cooldowsClass cooldowns = new cooldowsClass();

    //_________||

    //Get the database data
    public static string id {get{return _id;} set { _id = value; } }
    public static string username { get { return _username; } set { _username = value; } }
    public static int level { get { return _level; } set { _level = value; } }
    public static int exp { get { return _exp; } set { _exp = value; } }
    public static int resources { get { return _resources; } set { _resources = value; } }
    public static int gold { get { return _gold; } set { _gold = value; } }

    //Get the local data
    public static int healthPoints { get { return _healthPoints; } set { _healthPoints = value; } }
    public static int maxHealthPoints { get { return _maxHealthPoints; } set { _maxHealthPoints = value; } }

    //Cooldowns
    public static float slashCooldown { get { return cooldowns._slashCooldown; } set { cooldowns._slashCooldown = value; } }
    public static float xslashCooldown { get { return cooldowns._xslashCooldown; } set { cooldowns._xslashCooldown = value; } }
    public static float shieldCooldown { get { return cooldowns._shieldCooldown; } set { cooldowns._shieldCooldown = value; } }



    public static int getNeededExp()
    {
        int result = level * 10;

        return result;
    }

    public static void resetCooldowns()
    {
        //Slash
        if (cooldowns._slashCooldown > 0)
            cooldowns._slashCooldown -= Time.deltaTime;
        else
            cooldowns._slashCooldown = 0;

        //XSlash
        if (cooldowns._xslashCooldown > 0)
            cooldowns._xslashCooldown -= Time.deltaTime;
        else
            cooldowns._xslashCooldown = 0;

        //Shield
        if (cooldowns._shieldCooldown > 0)
            cooldowns._shieldCooldown -= Time.deltaTime;
        else
            cooldowns._shieldCooldown = 0;
    }

    static public accountInfoResponse.nestedData savePlayerData()
    {
        accountInfoResponse.nestedData data = new accountInfoResponse.nestedData();

        data.stats = new accountInfoResponse.nestedData.nested2Data();

        //Set the data to be sent
        data._id = _id;
        data.stats.level = _level;
        data.stats.exp = _exp;
        data.stats.gold = _gold;
        data.stats.ressources = _resources;

        return data;
    }

        public static IEnumerator savePlayerDataRequest(accountInfoResponse.nestedData data, Action doLast)
        {
            Debug.Log("Trying to save player data");

            //Set the cookie
            WebServices.CookieString = null;

            var request = WebServices.Post("save/fullSave", JsonUtility.ToJson(data));

            //Make the code wait until the server responds
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Network Error");
            }
            else
            {
                //Get the header for proccessing
                WebServices.CookieString = request.GetResponseHeader("set-cookie");

                //The server response
                string result = request.downloadHandler.text;

                doLast();

                Debug.Log("Completed");
        }
        }



    static public void SetPlayerData(accountInfoResponse json)
    {
        if (json.body._id != null){_id = json.body._id;}
        if (json.body.name != null){ _username = json.body.name;}
        if (json.body.email != null) { _email = json.body.email; }

        if (json.body.stats.level < 1)
        {
            _level = 1;
        }
        else
        {
            _level = json.body.stats.level;
        }

        _exp = json.body.stats.exp;
        _resources = json.body.stats.ressources;
        _gold = json.body.stats.gold;

        //Calculate the max health
        _maxHealthPoints = 100;

        //Reset the player hp
        _healthPoints = _maxHealthPoints;
    }

    static public void ResetPlayerData()
    {
        _id = null;
        _username = "Offline Player";
        _level = 1;
        _exp = 0;
        _resources = 0;
        _gold = 0;
        _email = "";
        _healthPoints = 100;
        _maxHealthPoints = 100;
    }

}
