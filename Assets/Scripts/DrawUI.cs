using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    public GameObject Content1;
    public GameObject guessKeyPrefab;

    public GameObject plainText, cipherText, command, feedback, error;
    public GameObject submit, clear, reset, quit;

    public GameObject commandExecutor;
    // Start is called before the first frame update
    void Start()
    {

        GlobalVariable.instance.plainText = plainText.GetComponent<TMP_InputField>();
        GlobalVariable.instance.cipherText = cipherText.GetComponent<TMP_InputField>();
        GlobalVariable.instance.command = command.GetComponent<TMP_InputField>();
        GlobalVariable.instance.feedback = feedback.GetComponent<TMP_InputField>();
        GlobalVariable.instance.error = error.GetComponent<TMP_InputField>();
        GlobalVariable.instance.submit = submit.GetComponent<Button>();
        GlobalVariable.instance.clear = clear.GetComponent<Button>();
        GlobalVariable.instance.reset = reset.GetComponent<Button>();
        GlobalVariable.instance.quit = quit.GetComponent<Button>();

        for(int i = 1; i <= 26; i++)
        {
            RectTransform content1Rec = Content1.GetComponent<RectTransform>();
            //创建Key
            char key = (char)(64 + i);
            GameObject keyObject = new("Key" + key);
            keyObject.transform.SetParent(Content1.transform);
            RectTransform keyRect = keyObject.AddComponent<RectTransform>();
            LeftUpLock(keyRect);
            keyRect.anchoredPosition = new Vector3(0, -32 * (i - 1), 0);
            keyRect.sizeDelta = new Vector2(150, 30);
            content1Rec.sizeDelta = new Vector2(content1Rec.sizeDelta.x, content1Rec.sizeDelta.y + 32);

            //charBase
            GameObject charBase = new("CharBase");
            charBase.transform.SetParent(keyObject.transform);
            Image charBaseImage = charBase.AddComponent<Image>();
            if(UnityEngine.ColorUtility.TryParseHtmlString("#DAD6A3",out Color color))
            {
                charBaseImage.color = color;
            }
            RectTransform charBaseRect = charBase.GetComponent<RectTransform>();
            LeftUpLock(charBaseRect);
            charBaseRect.anchoredPosition = new Vector3(5, -5, 0);
            charBaseRect.sizeDelta = new Vector2(20, 20);

            //keyChar
            GameObject keyChar = new("KeyChar");
            keyChar.transform.SetParent(keyObject.transform);
            TextMeshProUGUI keyCharText= keyChar.AddComponent<TextMeshProUGUI>();
            keyCharText.text += key;
            keyCharText.fontSize = 20;
            keyCharText.alignment = TextAlignmentOptions.Center;
            keyCharText.color = Color.black;
            RectTransform keyCharRect = keyChar.GetComponent<RectTransform>();
            LeftUpLock(keyCharRect);
            keyCharRect.anchoredPosition = new Vector3(5, -5, 0);
            keyCharRect.sizeDelta = new Vector2(20, 20);

            //lockBase
            GameObject lockBase = new("CharBase");
            lockBase.transform.SetParent(keyObject.transform);
            Image lockBaseImage = lockBase.AddComponent<Image>();
            lockBaseImage.color = Color.white;
            RectTransform lockBaseRect = lockBase.GetComponent<RectTransform>();
            LeftUpLock(lockBaseRect);
            lockBaseRect.anchoredPosition = new Vector3(32.5f, -7.5f, 0);
            lockBaseRect.sizeDelta = new Vector2(15, 15);

            //lock
            GameObject keyLock = new("KeyLock");
            keyLock.transform.SetParent(keyObject.transform);
            Button lockButton= keyLock.AddComponent<Button>();
            lockButton.onClick.AddListener(() => OnClickLockButton(key));
            TextMeshProUGUI lockText =lockButton.AddComponent<TextMeshProUGUI>();
            lockText.text = "×";
            lockText.fontSize = 18;
            lockText.alignment = TextAlignmentOptions.Center;
            lockText.color = Color.red;
            RectTransform lockRect = keyLock.GetComponent<RectTransform>();
            LeftUpLock(lockRect);
            lockRect.anchoredPosition = new Vector3(32.5f, -7.5f, 0);
            lockRect.sizeDelta = new Vector2(15, 15);
            GlobalVariable.instance.lockText[i]= lockText;

            //guessKey
            GameObject guessKey = Instantiate(guessKeyPrefab);
            guessKey.name = "GuessKey";
            guessKey.transform.SetParent(keyObject.transform);
            RectTransform guessKeyRect = guessKey.GetComponent<RectTransform>();
            LeftUpLock(guessKeyRect);
            guessKeyRect.anchoredPosition = new Vector3(50, -2, 0);
            guessKeyRect.sizeDelta = new Vector2(100, 26);
            GameObject guessContent = guessKey.transform.Find("Viewport/Content").gameObject;
            RectTransform contentrect = guessContent.GetComponent<RectTransform>();
            contentrect.sizeDelta = new Vector2(3, 21);
            for(int j = 1; j <= 26; j++)
            {
                //guessBase
                GameObject guessBase = new("CharBase");
                guessBase.transform.SetParent(guessContent.transform);
                Image guessBaseImage = guessBase.AddComponent<Image>();
                guessBaseImage.color = Color.green;
                RectTransform guessBaseRect = guessBase.GetComponent<RectTransform>();
                LeftUpLock(guessBaseRect);
                guessBaseRect.anchoredPosition = new Vector3(3 + (j - 1) * 18, -3, 0);
                guessBaseRect.sizeDelta = new Vector2(15, 15);
                //guess
                char guess = (char)(j + 64);
                GameObject guessButton = new("GuessButton" + j.ToString());
                guessButton.transform.SetParent(guessContent.transform);
                Button guessbutton = guessButton.AddComponent<Button>();
                guessbutton.onClick.AddListener(() => OnClickGuessButton(key,guess));
                TextMeshProUGUI guessText = guessButton.AddComponent<TextMeshProUGUI>();
                guessText.text += guess;
                guessText.fontSize = 15;
                guessText.alignment = TextAlignmentOptions.Center;
                guessText.color = Color.black;
                RectTransform guessRect = guessButton.GetComponent<RectTransform>();
                LeftUpLock(guessRect);
                guessRect.anchoredPosition = new Vector3(3 + (j - 1) * 18, -3, 0);
                guessRect.sizeDelta = new Vector2(15, 15);
                contentrect.sizeDelta = new Vector2(contentrect.sizeDelta.x + 18, contentrect.sizeDelta.y);
                GlobalVariable.instance.guessBaseImages[i, j] = guessBaseImage;
                GlobalVariable.instance.guessButtons[i, j] = guessbutton;
            }
        }

        GlobalVariable.instance.submit.onClick.AddListener(() => OnClickSubmit());
        GlobalVariable.instance.clear.onClick.AddListener(() => OnClickClear());
        GlobalVariable.instance.reset.onClick.AddListener(() => OnClickReset());
        GlobalVariable.instance.quit.onClick.AddListener(() => OnClickQuit());

        for(int i = 1; i <= 26; i++)
        {
            GlobalVariable.instance.guessKey[i] = new();
        }
    }
    private void LeftUpLock(RectTransform rect)
    {
        rect.anchorMax = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.localScale = new Vector3(1, 1, 1);
    }
    public void OnClickLockButton(char id)
    {
        ;
    }
    public void OnClickGuessButton(char id1, char id2)
    {
        List<Vector2Int> toChange = new List<Vector2Int>();
        toChange.Add(new Vector2Int(id1 - 64, id2 - 64));
        toChange = commandExecutor.GetComponent<Command>().InferChain(toChange);
        if (toChange == null)
        {
            GlobalVariable.instance.error.text = "Error:You can't change like that because there will be a logical contradiction make a letter have no corresponding ciphertext";
            return;
        }
        foreach (Vector2Int vector2Int in toChange)
        {
            commandExecutor.GetComponent<Command>().ChangeGuess(vector2Int.x, vector2Int.y);
        }
        GlobalVariable.instance.history.Push(toChange);
        commandExecutor.GetComponent<Command>().LockCheck();
    }
    public void OnClickSubmit()
    {
        string command=GlobalVariable.instance.command.text;
        commandExecutor.GetComponent<Command>().Execute(command);
    }
    public void OnClickClear()
    {
        GlobalVariable.instance.feedback.text = "";
        GlobalVariable.instance.error.text = "";
    }
    public void OnClickReset()
    {
        GlobalVariable.instance.plainText.text = "";
        GlobalVariable.instance.cipherText.text = "";
        GlobalVariable.instance.command.text = "";
        GlobalVariable.instance.feedback.text = "";
        GlobalVariable.instance.error.text = "";
        for (int i = 1; i <= 26; i++)
        {
            GlobalVariable.instance.key[i] = 0;
            GlobalVariable.instance.guessKey[i].sure = false;
            GlobalVariable.instance.guessKey[i].sureChar = 0;
            GlobalVariable.instance.lockText[i].text = "×";
            GlobalVariable.instance.lockText[i].color = Color.red;
            for (int j = 1; j <= 26; j++)
            {
                GlobalVariable.instance.guessKey[i].guess[j] = false;
                GlobalVariable.instance.guessBaseImages[i, j].color = Color.green;
            }
        }
        GlobalVariable.instance.history.Clear();
        GlobalVariable.instance.redo.Clear();
        GlobalVariable.instance.autoLevel = 0;
    }
    public void OnClickQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // 在构建的应用中退出程序
                Application.Quit();
        #endif
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
