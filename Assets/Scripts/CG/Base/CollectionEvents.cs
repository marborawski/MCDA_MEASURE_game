using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CG_Base
{

// A container that allows you to attach data to the event
    public class EventData
    {

// Possible data types attached to the event
        public enum TypeData { none, floatData, stringData, leftRightData, leftRightFloatData, leftRightVector2Data, gameObjectData, healthData, hitData, rayHitData, gameObjectController, answer, vector3, string4Data, thingData };

// The type of data attached to the event
        public TypeData typeData = TypeData.none;

    }

// Container storing string data
    public class EventStringData : EventData
    {

// Data type assignment
        public EventStringData()
        {
            typeData = TypeData.stringData;
        }

// Assigning a data type and entering string data into the container
// d - data of type string
        public EventStringData(string d)
        {
            typeData = TypeData.stringData;
            data = d;
        }

// Stored string data
        public string data;
    }

// Container storing string data
    public class Event4StringData : EventData
    {

// Data type assignment
        public Event4StringData()
        {
            typeData = TypeData.string4Data;
        }

// Assigning a data type and entering string data into the container
// d1, d2, d3, d4 - string data
        public Event4StringData(string d1, string d2, string d3, string d4)
        {
            typeData = TypeData.string4Data;
            data1 = d1;
            data2 = d2;
            data3 = d3;
            data4 = d4;
        }

// Stored string data
        public string data1, data2, data3, data4;
    }

// The container storing the data will provide the respondent's response to the survey question
    public class EventAnswerData : EventData
    {

// Data type assignment
        public EventAnswerData()
        {
            typeData = TypeData.answer;
        }

// Assigning a data type and entering data related to the respondent's response into the container
// _question   - question asked
// _questionId - question ID
// _answer     - answer provided
// _answerId   - the identifier of the response provided
        public EventAnswerData(string _question, string _questionId, string _answer, string _answerId)
        {
            typeData = TypeData.answer;
            question = _question;
            questionId = _questionId;
            answer = _answer;
            answerId = _answerId;
        }

// Question asked
        public string question;

// Question ID
        public string questionId;

// Answer provided
        public string answer;

// The identifier of the response provided
        public string answerId;
    }

// Container storing float data
    public class EventFloatData : EventData
    {

// Data type assignment
        public EventFloatData()
        {
            typeData = TypeData.floatData;
        }

// Assigning the data type and entering the float data type into the container
// d - data of type float
        public EventFloatData(float d)
        {
            typeData = TypeData.floatData;
            data = d;
        }

// Stored float data
        public float data;
    }

// A container that holds a pointer to an object of the GameObject type
    public class EventGameObjectData : EventData
    {

// Data type assignment
        public EventGameObjectData()
        {
            typeData = TypeData.gameObjectData;
        }

// Assigning a data type and inserting a pointer to a GameObject object into the container
// ob - pointer to an object of type GameObject
        public EventGameObjectData(GameObject ob)
        {
            typeData = TypeData.gameObjectData;
            obiect = ob;
        }

// Stored pointer to an object of type GameObject
        public GameObject obiect;
    }

// A container that holds a pointer to a RaycastHit object that stores information about the ray hit
    public class EventRayHitData : EventData
    {

// Data type assignment
        public EventRayHitData()
        {
            typeData = TypeData.rayHitData;
        }

// Assigning a data type and introducing a pointer to a RaycastHit object into the container
// ob - pointer to an object of type RaycastHit
        public EventRayHitData(RaycastHit hit)
        {
            typeData = TypeData.rayHitData;
            data = hit;
        }

// Stored pointer to a RaycastHit object
        public RaycastHit data;
    }

// Container storing Vector3 data
    public class EventVector3 : EventData
    {

// Data type assignment
        public EventVector3()
        {
            typeData = TypeData.vector3;
        }

// Assigning the data type and entering the Vector3 data type into the container
// v - data of type Vector3
        public EventVector3(Vector3 v)
        {
            typeData = TypeData.vector3;
            data = v;
        }

// Stored data of type Vector3
        public Vector3 data;
    }

// A delegate defines a method called by the event handling system. The method should be implemented in the object handling the event
// type  - type or name of the event
// param - event parameters
// data  - data attached to the event
// ob    - the object that created the event
    public delegate void eventFunction(string type, string param, EventData data, RootClass ob);

// The class from which all classes that can generate events must inherit
    public class CollectionEvents : RootClass
    {

// Blocking event generation
        public bool eventBlocking = false;

        private List<eventFunction> eventList = new List<eventFunction>();

        private List<eventFunction> removeList = null;

// Przypisanie metody obsługi zdarzenia do listy wywoływanych metod obsługi zdarzeń
        public eventFunction runEvent
        {
            set
            {
                if (eventList.IndexOf(value) < 0)
                    eventList.Add(value);
            }
        }

// Removing an event handler method from the list of invoked event handler methods
// ev - usuwana metoda obsługi zdarzeń
        public void RemoveEvent(eventFunction ev)
        {
            eventList.Remove(ev);
        }

// Safely removing an event handler method from the list of invoked event handler methods (when the removal must be done from within the event handler method)
// ev - usuwana metoda obsługi zdarzeń
        public void SafeRemoveEvent(eventFunction ev)
        {
            if (removeList == null)
                removeList = new List<eventFunction>();
            removeList.Add(ev);
        }

// Calling event handling methods
// type  - type or name of the event
// param - event parameters
// data  - data attached to the event
        public void RunEvent(string type, string param, EventData data)
        {
            foreach (eventFunction item in eventList)
                if (item != null && !eventBlocking)
                    item(type, param, data, this);
        }

// Calling event handling methods with the assignment of a specific object as the event source
// type  - type or name of the event
// param - event parameters
// data  - data attached to the event
// ob    - the object that created the event

        public void RunEvent(string type, string param, EventData data, RootClass ob)
        {
            foreach (eventFunction item in eventList)
                if (item != null && !eventBlocking)
                    item(type, param, data, ob);
        }

        override protected void Awake()
        {
            base.Awake();
        }

        override protected void Start()
        {
            base.Start();
        }

        override protected void Update()
        {
            base.Update();

            if (removeList != null)
            {
                foreach (eventFunction item in removeList)
                {
                    eventList.Remove(item);
                }
                removeList = null;
            }
        }

    }
}
