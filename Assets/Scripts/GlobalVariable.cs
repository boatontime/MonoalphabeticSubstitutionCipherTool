using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariable
{
    public static readonly GlobalVariable instance = new();

    public Guess[] guessKey = new Guess[27];
    public int[] key = new int[27];

    public TextMeshProUGUI[] lockText = new TextMeshProUGUI[27];
    public Button[,] guessButtons = new Button[27, 27];
    public Image[,] guessBaseImages = new Image[27, 27];
    public TMP_InputField plainText, cipherText, command, feedback, error;
    public Button submit, clear, reset, quit;

    public Stack<List<Vector2Int>> history = new();
    public Stack<List<Vector2Int>> redo = new();
    public int autoLevel = 0;

    public NGramFreq[][] nGramFreqs = new NGramFreq[5][];
}
