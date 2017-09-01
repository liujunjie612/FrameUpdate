using UnityEngine;
using UnityEngine.UI;

public class Log : Singleton<Log>
{
    public static bool showLog = true;
    public Text txt;
    public RectTransform rect;
    public Scrollbar scrollbar;
    public Button clearBtn;

    void Start()
    {
        scrollbar.onValueChanged.AddListener(__onScrollbarValueCahnged);
        clearBtn.onClick.AddListener(this.__onClearClick);
    }

    public void Info(object msg)
    {
        if (showLog)
        {
            Debug.Log(msg);
            setTxt(msg);
        }
    }

    public void Error(object msg)
    {
        if (showLog)
            setTxt(msg);
    }

    public void Warning(object msg)
    {
        if (showLog)
            setTxt(msg);
    }

    /// <summary>
    /// 这个方法是用来标志要注意的点
    /// </summary>
    public void Note()
    {

    }

    private void setTxt(object s)
    {
        txt.text += s;
        txt.text += "\n";
        rect.sizeDelta = new Vector2(1000, txt.preferredHeight);
    }

    private void __onScrollbarValueCahnged(float f)
    {
        scrollbar.value = 0;
    }

    private void __onClearClick()
    {
        txt.text = "";
    }
}
