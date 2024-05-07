using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CG_Base
{

// Defining a virtual method that triggers the event handler
    public class RootClass : MonoBehaviour
    {

// Module name that can be used in event handling
        public string nameModule;

// A counter that allows you to assign an individual identifier to the object
        static private int id = 0;

// Virtual method that starts the event handler
// type  - type or name of the event
// param - event parameters
// data  - data attached to the event
// ob    - the object that created the event
        public virtual void FunEvent(string type, string param, EventData data, RootClass ob)
        {

        }

// Get the unique object identifier
// Returns a unique object identifier
        public int GetId()
        {
            return id;
        }

        virtual protected void Awake()
        {
             id++;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {

        }
    }
}
