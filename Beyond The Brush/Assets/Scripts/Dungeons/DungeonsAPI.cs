﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dungeon
{
    //Variables||

        public string DungeonName;
        public int baseReward;

        GameObject startingRoom;
        public List<room> rooms = new List<room>();
    //_________||


    public Dungeon(string name)
    {
        DungeonName = name;
    }

    [System.Serializable]
    public class room
    {
        public string roomName = "room";

        [System.Serializable]
        public class sides
        {
            //Room sides||
                public bool top = false;
                public bool right = false;
                public bool left = false;
                public bool bottom = false;
            //__________||
        }

        public GameObject roomPrefab;
        public sides roomSides = new sides();

    }

}

public class DungeonsAPI : MonoBehaviour
{
    public List<Dungeon> dungeons = new List<Dungeon>();


}
