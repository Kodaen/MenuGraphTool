using MenuGraphTool;
using UnityEngine;

public class DemoMenuGraphButton : MonoBehaviour
{
    [SerializeField] private RuntimeMenuGraph _menuGraph;

    public void OpenMenuGraph()
    {
        if (_menuGraph == null)
        {
            return;            
        }
        MenuDirector.Instance.OpenMenuGraph(_menuGraph);
        MenuDirector.OnCurrentMenuGraphCloses += OnCurrentMenuGraphCloses;

        this.gameObject.SetActive(false);
    }

    private void OnCurrentMenuGraphCloses()
    {
        this.gameObject.SetActive(true);
    }
}
