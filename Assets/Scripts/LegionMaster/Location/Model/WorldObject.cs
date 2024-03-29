﻿using UnityEngine;

namespace LegionMaster.Location.Model
{
    public class WorldObject : MonoBehaviour, IWorldObject
    {
        [SerializeField]
        private string _objectId;
        
        [SerializeField]
        private ObjectType _objectType;
        public void Reset()
        {
            ObjectId = gameObject.name;
        }
        public string ObjectId
        {
            get { return _objectId; }
            set { _objectId = value; }
        }
        public GameObject GameObject
        {
            get { return gameObject; }
        }
        public ObjectType ObjectType
        {
            get { return _objectType; }
        }
    }
}