using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public GameObject store;
    public Button close;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  Button buttonClose = close.GetComponent<Button>();
        close.onClick.AddListener(() => store.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
