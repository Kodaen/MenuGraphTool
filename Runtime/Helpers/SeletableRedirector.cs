using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class SelectableRedirector : Selectable
    {
        [SerializeField] private Selectable _redirectionReceiver;

        public Selectable RedirectionReceiver
        {
            get { return _redirectionReceiver; }
            set { _redirectionReceiver = value; }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (_redirectionReceiver != null)
            {
                EventSystem.current.SetSelectedGameObject(_redirectionReceiver.gameObject);
            }
        }
    }
}
