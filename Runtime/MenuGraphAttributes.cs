using System;

namespace MenuGraphTool
{
    public class MenuInputAttribute : Attribute
    { }

    public class MenuOutputAttribute : Attribute
    {

        #region Fields
        private string _displayName = default;
        public string[] _outputParamsName;
        #endregion Fields

        #region Properties
        public string DisplayName
        {
            get { return _displayName; }
            private set { _displayName = value; }
        }

        public string[] OutputParamsName
        {
            get { return _outputParamsName; }
            private set { _outputParamsName = value; }
        } 
        #endregion Properties

        #region Constructor
        public MenuOutputAttribute(string displayName = null, params string[] paramNames)
        {
            DisplayName = displayName;
            OutputParamsName = paramNames;
        } 
        #endregion Constructor

    }
}