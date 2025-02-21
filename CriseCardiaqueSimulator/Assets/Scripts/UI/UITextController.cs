using TMPro;
using UnityEngine;

public class UITextController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textField;
    [SerializeField] private string m_textFormat;

    public void UpdateText(params object[] arguments)
    {
        m_textField.text = string.Format(m_textFormat, arguments);
    }
}
