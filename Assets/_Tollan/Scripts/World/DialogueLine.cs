using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)] public string text;

    public DialogueLine() { }
    public DialogueLine(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}
