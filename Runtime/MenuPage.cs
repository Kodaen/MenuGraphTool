using System;
using System.Collections.Generic;
using UnityEngine;

namespace MenuGraphTool
{
    [ExecuteInEditMode]
    public class MenuPage : MonoBehaviour
    {
        [SerializeField]
        public List<KeyValuePair<Action, List<Type>>> actions = new();


        private void Awake()
        {
            actions.Clear();

            List<Type> types = new List<Type>();
            types.Add(typeof(int));
            types.Add(typeof(float));
            types.Add(typeof(double));

            KeyValuePair<Action, List<Type>> kvp = new KeyValuePair<Action, List<Type>>(new Action(test), types);

            actions.Add(kvp);
        }

        private void test()
        {

        }
    }

    public class Character
    {
        string name;
    }
}