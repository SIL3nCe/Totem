﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    [Serializable]
    public class ObjectInfo
    {
        public GameObject objectPrefab;

        public int spawnMinCount;
        public int spawnMaxCount;
    }

    public List<ObjectInfo> objectList;

}
