using System;

namespace MenuGraphTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MenuInputAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MenuOutputAttribute : Attribute
    {
        #region Fields
        private string _flowName = default;
        // TODO : Add display names.
        //public string _displayName = default;
        #endregion Fields

        #region Properties
        public string FlowName
        {
            get { return _flowName; }
            private set { _flowName = value; }
        }

        //public string DisplayName
        //{
        //    get { return _displayName; }
        //    private set { _displayName = value; }
        //}
        #endregion Properties

        #region Constructor
        public MenuOutputAttribute(string flowName/*, string displayName = null*/)
        {
            FlowName = flowName;
            //DisplayName = displayName;
        } 
        #endregion Constructor
    }
}