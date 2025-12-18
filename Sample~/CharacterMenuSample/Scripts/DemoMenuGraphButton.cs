using MenuGraphTool;
using UnityEngine;

public class DemoMenuGraphButton : MonoBehaviour
{
    [SerializeField] private MenuGraph _menuGraph;

    public void OpenMenuGraph()
    {
        if (_menuGraph == null)
        {
            return;            
        }

        Character character = new();

        MenuDirector.Instance.OpenMenuGraph(_menuGraph, character);
        MenuDirector.OnCurrentMenuGraphCloses += OnCurrentMenuGraphCloses;

        this.gameObject.SetActive(false);
    }

    private void OnCurrentMenuGraphCloses()
    {
        this.gameObject.SetActive(true);
    }
}
