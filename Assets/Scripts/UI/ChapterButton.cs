using UnityEngine;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log("Chapter button clicked!");
    }
}
