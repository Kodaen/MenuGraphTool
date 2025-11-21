using System;
using System.Collections.Generic;
using UnityEngine;

namespace MenuGraphTool
{
    internal class RuntimeMenuGraph : ScriptableObject
    {
        public string EntryNodeID;
        public List<RuntimeMenuNode> AllNodes = new();
    }

    [Serializable]
    internal class RuntimeMenuNode
    {
        public string NodeID;
        public string NextNodeID;
    }

}
