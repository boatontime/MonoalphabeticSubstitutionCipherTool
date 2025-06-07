using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Command : MonoBehaviour
{
    public GameObject errorCollector;
    public GameObject Scrollbar;
    public void Start()
    {
        // 1-gram 单字母频率 (英语字母标准频率)
        GlobalVariable.instance.nGramFreqs[1] = new NGramFreq[] {
            new NGramFreq { ngram = "E", frequency = 0.12702 },
            new NGramFreq { ngram = "T", frequency = 0.09056 },
            new NGramFreq { ngram = "A", frequency = 0.08167 },
            new NGramFreq { ngram = "O", frequency = 0.07507 },
            new NGramFreq { ngram = "I", frequency = 0.06966 },
            new NGramFreq { ngram = "N", frequency = 0.06749 },
            new NGramFreq { ngram = "S", frequency = 0.06327 },
            new NGramFreq { ngram = "H", frequency = 0.06094 },
            new NGramFreq { ngram = "R", frequency = 0.05987 },
            new NGramFreq { ngram = "D", frequency = 0.04253 },
            new NGramFreq { ngram = "L", frequency = 0.04025 },
            new NGramFreq { ngram = "C", frequency = 0.02782 },
            new NGramFreq { ngram = "U", frequency = 0.02758 },
            new NGramFreq { ngram = "M", frequency = 0.02406 },
            new NGramFreq { ngram = "W", frequency = 0.02360 },
            new NGramFreq { ngram = "F", frequency = 0.02228 },
            new NGramFreq { ngram = "G", frequency = 0.02015 },
            new NGramFreq { ngram = "Y", frequency = 0.01974 },
            new NGramFreq { ngram = "P", frequency = 0.01929 },
            new NGramFreq { ngram = "B", frequency = 0.01492 },
            new NGramFreq { ngram = "V", frequency = 0.00978 },
            new NGramFreq { ngram = "K", frequency = 0.00772 },
            new NGramFreq { ngram = "J", frequency = 0.00153 },
            new NGramFreq { ngram = "X", frequency = 0.00150 },
            new NGramFreq { ngram = "Q", frequency = 0.00095 },
            new NGramFreq { ngram = "Z", frequency = 0.00074 }
        };

        // 2-gram 双字母频率
        GlobalVariable.instance.nGramFreqs[2] = new NGramFreq[] {
            new NGramFreq { ngram = "TH", frequency = 0.0388 },
            new NGramFreq { ngram = "HE", frequency = 0.0368 },
            new NGramFreq { ngram = "IN", frequency = 0.0228 },
            new NGramFreq { ngram = "ER", frequency = 0.0218 },
            new NGramFreq { ngram = "AN", frequency = 0.0214 },
            new NGramFreq { ngram = "RE", frequency = 0.0174 },
            new NGramFreq { ngram = "ON", frequency = 0.0161 },
            new NGramFreq { ngram = "AT", frequency = 0.0155 },
            new NGramFreq { ngram = "EN", frequency = 0.0150 },
            new NGramFreq { ngram = "ND", frequency = 0.0149 },
            new NGramFreq { ngram = "TI", frequency = 0.0141 },
            new NGramFreq { ngram = "ES", frequency = 0.0139 },
            new NGramFreq { ngram = "OR", frequency = 0.0131 },
            new NGramFreq { ngram = "TE", frequency = 0.0128 },
            new NGramFreq { ngram = "OF", frequency = 0.0127 },
            new NGramFreq { ngram = "ED", frequency = 0.0126 },
            new NGramFreq { ngram = "IS", frequency = 0.0123 },
            new NGramFreq { ngram = "IT", frequency = 0.0113 },
            new NGramFreq { ngram = "AL", frequency = 0.0109 },
            new NGramFreq { ngram = "AR", frequency = 0.0107 },
            new NGramFreq { ngram = "ST", frequency = 0.0105 },
            new NGramFreq { ngram = "TO", frequency = 0.0104 },
            new NGramFreq { ngram = "NT", frequency = 0.0104 },
            new NGramFreq { ngram = "NG", frequency = 0.0095 },
            new NGramFreq { ngram = "SE", frequency = 0.0093 },
            new NGramFreq { ngram = "HA", frequency = 0.0093 },
            new NGramFreq { ngram = "AS", frequency = 0.0087 },
            new NGramFreq { ngram = "OU", frequency = 0.0087 },
            new NGramFreq { ngram = "IO", frequency = 0.0081 },
            new NGramFreq { ngram = "LE", frequency = 0.0081 },
            new NGramFreq { ngram = "VE", frequency = 0.0080 },
            new NGramFreq { ngram = "CO", frequency = 0.0079 },
            new NGramFreq { ngram = "ME", frequency = 0.0079 },
            new NGramFreq { ngram = "DE", frequency = 0.0076 },
            new NGramFreq { ngram = "HI", frequency = 0.0076 },
            new NGramFreq { ngram = "RI", frequency = 0.0073 },
            new NGramFreq { ngram = "RO", frequency = 0.0073 },
            new NGramFreq { ngram = "IC", frequency = 0.0070 },
            new NGramFreq { ngram = "NE", frequency = 0.0069 },
            new NGramFreq { ngram = "EA", frequency = 0.0069 },
            new NGramFreq { ngram = "RA", frequency = 0.0069 },
            new NGramFreq { ngram = "CE", frequency = 0.0065 },
            new NGramFreq { ngram = "LI", frequency = 0.0062 },
            new NGramFreq { ngram = "CH", frequency = 0.0060 },
            new NGramFreq { ngram = "LL", frequency = 0.0058 },
            new NGramFreq { ngram = "BE", frequency = 0.0058 },
            new NGramFreq { ngram = "MA", frequency = 0.0057 },
            new NGramFreq { ngram = "SI", frequency = 0.0055 },
            new NGramFreq { ngram = "OM", frequency = 0.0055 },
            new NGramFreq { ngram = "UR", frequency = 0.0054 }
        };

        // 3-gram 三字母频率
        GlobalVariable.instance.nGramFreqs[3] = new NGramFreq[] {
            new NGramFreq { ngram = "THE", frequency = 0.0352 },
            new NGramFreq { ngram = "AND", frequency = 0.0158 },
            new NGramFreq { ngram = "ING", frequency = 0.0112 },
            new NGramFreq { ngram = "ION", frequency = 0.0089 },
            new NGramFreq { ngram = "ENT", frequency = 0.0086 },
            new NGramFreq { ngram = "HER", frequency = 0.0078 },
            new NGramFreq { ngram = "FOR", frequency = 0.0071 },
            new NGramFreq { ngram = "THA", frequency = 0.0069 },
            new NGramFreq { ngram = "NTH", frequency = 0.0065 },
            new NGramFreq { ngram = "INT", frequency = 0.0064 },
            new NGramFreq { ngram = "ERE", frequency = 0.0061 },
            new NGramFreq { ngram = "TIO", frequency = 0.0060 },
            new NGramFreq { ngram = "TER", frequency = 0.0058 },
            new NGramFreq { ngram = "EST", frequency = 0.0057 },
            new NGramFreq { ngram = "ERS", frequency = 0.0055 },
            new NGramFreq { ngram = "ATI", frequency = 0.0054 },
            new NGramFreq { ngram = "HAT", frequency = 0.0053 },
            new NGramFreq { ngram = "ATE", frequency = 0.0050 },
            new NGramFreq { ngram = "ALL", frequency = 0.0049 },
            new NGramFreq { ngram = "ETH", frequency = 0.0047 },
            new NGramFreq { ngram = "HES", frequency = 0.0046 },
            new NGramFreq { ngram = "VER", frequency = 0.0044 },
            new NGramFreq { ngram = "HIS", frequency = 0.0043 },
            new NGramFreq { ngram = "OFT", frequency = 0.0042 },
            new NGramFreq { ngram = "ITH", frequency = 0.0041 },
            new NGramFreq { ngram = "FTH", frequency = 0.0040 },
            new NGramFreq { ngram = "STH", frequency = 0.0039 },
            new NGramFreq { ngram = "OTH", frequency = 0.0038 },
            new NGramFreq { ngram = "RES", frequency = 0.0037 },
            new NGramFreq { ngram = "ONT", frequency = 0.0036 },
            new NGramFreq { ngram = "DTH", frequency = 0.0035 },
            new NGramFreq { ngram = "ARE", frequency = 0.0034 },
            new NGramFreq { ngram = "REA", frequency = 0.0033 },
            new NGramFreq { ngram = "EAR", frequency = 0.0032 },
            new NGramFreq { ngram = "WAS", frequency = 0.0031 },
            new NGramFreq { ngram = "SIN", frequency = 0.0030 },
            new NGramFreq { ngram = "STO", frequency = 0.0029 },
            new NGramFreq { ngram = "TTH", frequency = 0.0028 },
            new NGramFreq { ngram = "STA", frequency = 0.0027 },
            new NGramFreq { ngram = "THI", frequency = 0.0026 },
            new NGramFreq { ngram = "FIN", frequency = 0.0025 },
            new NGramFreq { ngram = "CON", frequency = 0.0024 },
            new NGramFreq { ngram = "COM", frequency = 0.0023 },
            new NGramFreq { ngram = "EVE", frequency = 0.0022 },
            new NGramFreq { ngram = "PER", frequency = 0.0021 },
            new NGramFreq { ngram = "CTI", frequency = 0.0020 },
            new NGramFreq { ngram = "OUR", frequency = 0.0019 },
            new NGramFreq { ngram = "THR", frequency = 0.0018 },
            new NGramFreq { ngram = "YOU", frequency = 0.0017 },
            new NGramFreq { ngram = "ITH", frequency = 0.0016 }
        };

        // 4-gram 四字母频率
        GlobalVariable.instance.nGramFreqs[4] = new NGramFreq[] {
            new NGramFreq { ngram = "TION", frequency = 0.0098 },
            new NGramFreq { ngram = "NTHE", frequency = 0.0062 },
            new NGramFreq { ngram = "THER", frequency = 0.0059 },
            new NGramFreq { ngram = "THAT", frequency = 0.0053 },
            new NGramFreq { ngram = "OFTH", frequency = 0.0047 },
            new NGramFreq { ngram = "FTHE", frequency = 0.0045 },
            new NGramFreq { ngram = "THES", frequency = 0.0043 },
            new NGramFreq { ngram = "WITH", frequency = 0.0041 },
            new NGramFreq { ngram = "INTH", frequency = 0.0039 },
            new NGramFreq { ngram = "ATIO", frequency = 0.0038 },
            new NGramFreq { ngram = "OTHE", frequency = 0.0037 },
            new NGramFreq { ngram = "TTHE", frequency = 0.0035 },
            new NGramFreq { ngram = "DTHE", frequency = 0.0034 },
            new NGramFreq { ngram = "INGT", frequency = 0.0033 },
            new NGramFreq { ngram = "ETHE", frequency = 0.0032 },
            new NGramFreq { ngram = "PROD", frequency = 0.0031 },
            new NGramFreq { ngram = "THEI", frequency = 0.0030 },
            new NGramFreq { ngram = "CTIO", frequency = 0.0029 },
            new NGramFreq { ngram = "NDTH", frequency = 0.0028 },
            new NGramFreq { ngram = "FROM", frequency = 0.0027 },
            new NGramFreq { ngram = "MENT", frequency = 0.0026 },
            new NGramFreq { ngram = "THEM", frequency = 0.0025 },
            new NGramFreq { ngram = "RTHE", frequency = 0.0024 },
            new NGramFreq { ngram = "TING", frequency = 0.0023 },
            new NGramFreq { ngram = "THIS", frequency = 0.0022 },
            new NGramFreq { ngram = "STHE", frequency = 0.0021 },
            new NGramFreq { ngram = "THEP", frequency = 0.0020 },
            new NGramFreq { ngram = "HERE", frequency = 0.0019 },
            new NGramFreq { ngram = "THEC", frequency = 0.0018 },
            new NGramFreq { ngram = "IONS", frequency = 0.0017 },
            new NGramFreq { ngram = "EDTH", frequency = 0.0016 },
            new NGramFreq { ngram = "THEY", frequency = 0.0015 },
            new NGramFreq { ngram = "SAND", frequency = 0.0014 },
            new NGramFreq { ngram = "ATIO", frequency = 0.0013 },
            new NGramFreq { ngram = "WHIC", frequency = 0.0012 },
            new NGramFreq { ngram = "ENTH", frequency = 0.0011 },
            new NGramFreq { ngram = "ALLY", frequency = 0.0010 },
            new NGramFreq { ngram = "IGHT", frequency = 0.0009 },
            new NGramFreq { ngram = "OUGH", frequency = 0.0008 },
            new NGramFreq { ngram = "ABLE", frequency = 0.0007 },
            new NGramFreq { ngram = "OVER", frequency = 0.0006 },
            new NGramFreq { ngram = "COMP", frequency = 0.0005 },
            new NGramFreq { ngram = "UNDE", frequency = 0.0004 },
            new NGramFreq { ngram = "SOME", frequency = 0.0003 },
            new NGramFreq { ngram = "WHAT", frequency = 0.0002 },
            new NGramFreq { ngram = "WHEN", frequency = 0.0001 },
            new NGramFreq { ngram = "WERE", frequency = 0.0001 },
            new NGramFreq { ngram = "HAVE", frequency = 0.0001 },
            new NGramFreq { ngram = "BEEN", frequency = 0.0001 }
        };
    }
    public void Execute(string command)
    {
        ErrorCollect errorCollect = errorCollector.GetComponent<ErrorCollect>();
        if (string.IsNullOrWhiteSpace(command))
        {
            errorCollect.ThrowError("Type 'help' to get all available commands");
            return;
        }

        string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            errorCollect.ThrowError("Type 'help' to get all available commands");
            return;
        }

        string cmd = parts[0].ToLower();
        string[] args = parts.Skip(1).ToArray();
        string feedback;
        switch (cmd)
        {
            case "help":
                feedback = Help(args);
                break;
            case "guess":
                feedback = Guess0(args);
                break;
            case "suggest":
                feedback = Suggest0(args);
                break;
            case "undo":
                feedback = Undo0(args);
                break;
            case "redo":
                feedback = Redo(args);
                break;
            case "auto":
                feedback = Auto(args);
                break;
            default:
                errorCollect.ThrowError($"Unknown command:{cmd}.Type 'help' to get all available commands");
                return;
        }
        if (feedback.StartsWith("Error:"))
        {
            errorCollect.ThrowError(feedback.Substring(6));
            return;
        }

        GlobalVariable.instance.feedback.text += feedback + "\n";
        GlobalVariable.instance.error.text = "";
        Scrollbar.GetComponent<Scrollbar>().value = 1;
    }

    private string Help(string[] args)
    {
        if (args.Length == 0)
        {
            return @"all commands:
help <command> - show help for <command>
guess <0/1/2/3> <char> [chars] - set the relationship between the plaintext and the ciphertext
suggest - get suggestions from computer
redo - redo
undo - undo
auto - computer automatically guess the key according to suggestions
";
        }

        string subCmd = args[0].ToLower();
        return subCmd switch
        {
            "help" => "help <command> - show help for <command>",
            "guess" => "guess <0/1/2/3> <char> [chars] - set the relationship between the plaintext and the ciphertext\n" +
                      "  0 c0 c1,c2,... - c0 can only be coded as one of c1,c2...\n" +
                      "  1 c0 c1,c2,... - c0 can only not be coded as one of c1,c2...\n" +
                      "  2 c0 c1,c2,... - c0 can only be decoded as one of c1,c2...\n" +
                      "  3 c0 c1,c2,... - c0 can only not be decoded as one of c1,c2...",
            "suggest" => "suggest - get suggestions from computer\n no need for arguments",
            "undo" => "undo - undo\n no need for arguments",
            "redo" => "redo - redo\n no need for arguments",
            "auto" => "auto - computer automatically guess the key according to suggestions\n no need for arguments",
            _ => $"Error:Unknown command: {subCmd}. Type 'help' for available commands."
        };
    }

    private string Guess0(string[] args)
    {
        if (args.Length != 3)
            return "Error:Usage: guess <0/1/2/3> <char> [chars]";

        if (!int.TryParse(args[0], out int mode) || (mode < 0 || mode > 3))
            return "Error:The first argument could only be among 0~3";

        char c0 = char.ToUpper(args[1][0]);
        if (c0 < 'A' || c0 > 'Z')
            return "Error:Your second argument is not a letter";

        int c0Index = c0 - 'A' + 1;

        bool[] cIndex = new bool[27];
        for (int i = 1; i <= 26; i++)
        {
            cIndex[i] = false;
        }
        foreach (char c in args[2].ToUpper())
        {
            if (c < 'A' || c > 'Z') continue;
            cIndex[c - 'A' + 1] = true;
        }

        List<Vector2Int> toChange = new List<Vector2Int>();
        if (mode == 0 || mode == 1)
            for (int j = 1; j <= 26; j++)
            {
                if (cIndex[j])
                {
                    if (GlobalVariable.instance.guessKey[c0Index].guess[j] != (mode == 1))
                    {
                        toChange.Add(new Vector2Int(c0Index, j));
                    }
                }
                else
                {
                    if (GlobalVariable.instance.guessKey[c0Index].guess[j] == (mode == 1))
                    {
                        toChange.Add(new Vector2Int(c0Index, j));
                    }
                }
            }
        if (mode == 2 || mode == 3)
            for (int j = 1; j <= 26; j++)
            {
                if (cIndex[j])
                {
                    if (GlobalVariable.instance.guessKey[j].guess[c0Index] != (mode == 3))
                    {
                        toChange.Add(new Vector2Int(j, c0Index));
                    }
                }
                else
                {
                    if (GlobalVariable.instance.guessKey[j].guess[c0Index] == (mode == 3))
                    {
                        toChange.Add(new Vector2Int(j, c0Index));
                    }
                }
            }
        toChange = InferChain(toChange);
        if (toChange == null)
        {
            return "Error:You can't guess like that, because a ciphertext letter will be not able to get decoded";
        }
        foreach (Vector2Int vector2Int in toChange)
        {
            ChangeGuess(vector2Int.x, vector2Int.y);
        }
        GlobalVariable.instance.history.Push(toChange);
        GlobalVariable.instance.redo.Clear();
        LockCheck();
        return "Operate success";
    }

    private string Suggest0(string[] args)
    {
        if (args.Length != 0)
            return "Error:No need for arguments";
        string feedback;
        string cipherText = GlobalVariable.instance.cipherText.text;
        Guess[] guesskey = GlobalVariable.instance.guessKey;
        feedback = Suggest.GetSuggest(cipherText, guesskey, GlobalVariable.instance.nGramFreqs, 0);
        return feedback;
    }

    public void ChangeGuess(int c0, int c)
    {
        if (GlobalVariable.instance.guessKey[c0].guess[c] == false)
        {
            GlobalVariable.instance.guessKey[c0].guess[c] = true;
            GlobalVariable.instance.guessBaseImages[c0, c].color = Color.red;
        }
        else
        {
            GlobalVariable.instance.guessKey[c0].guess[c] = false;
            GlobalVariable.instance.guessBaseImages[c0, c].color = Color.green;
        }
    }

    public List<Vector2Int> InferChain(List<Vector2Int> toChange)
    {
        //备份数值
        bool[] sure=new bool[27];
        int[] sureChar=new int[27];
        bool[,] guess = new bool[27, 27];
        for(int i = 1; i <= 26; i++)
        {
            sure[i] = false;
            sureChar[i] = 0;
            for(int j = 1; j <= 26; j++)
            {
                guess[i, j] = GlobalVariable.instance.guessKey[i].guess[j];
            }
        }
        //数值更改
        for (int l = 0; l < toChange.Count; l++)
        {
            Vector2Int vector2Int = toChange[l];
            guess[vector2Int.x, vector2Int.y] = !guess[vector2Int.x, vector2Int.y];
        }
        //已知判定
        //新更改纳入
        bool flag = true;
        while (flag)
        {
            flag = false;
            for (int i = 1; i <= 26; i++)
            {
                int remain = 26;
                int remain2 = 26;
                int sureid = 0;
                for (int j = 1; j <= 26; j++)
                {
                    if (guess[i, j]) remain--;
                    else sureid = j;
                    if (guess[j, i]) remain2--;
                }
                if (remain == 0) return null;
                if (remain2 == 0) return null;
                if (remain == 1)
                {
                    sure[i] = true;
                    sureChar[i] = sureid;
                }
                if (sure[i])
                {
                    for (int j = 1; j <= 26; j++)
                    {
                        if (i != j && !guess[j, sureid])
                        {
                            flag = true;
                            toChange.Add(new Vector2Int(j, sureid));
                            guess[j, sureid] = true;
                        }
                    }
                }
            }
        }
        return toChange;
    }
    public void LockCheck()
    {
        for (int i = 1; i <= 26; i++)
        {
            GlobalVariable.instance.key[i] = 0;
        }
        for(int i = 1;i <= 26; i++)
        {
            int remain = 26;
            int sureid = 0;
            for (int j = 1; j <= 26; j++)
            {
                if (GlobalVariable.instance.guessKey[i].guess[j]) remain--;
                else sureid = j;
            }
            if (remain == 1)
            {
                GlobalVariable.instance.guessKey[i].sure = true;
                GlobalVariable.instance.guessKey[i].sureChar = sureid;
                GlobalVariable.instance.lockText[i].text = "" + (char)(64 + sureid);
                GlobalVariable.instance.lockText[i].color = Color.green;
                GlobalVariable.instance.key[sureid] = i;
            }
            else
            {
                GlobalVariable.instance.guessKey[i].sure = false;
                GlobalVariable.instance.guessKey[i].sureChar = 0;
                GlobalVariable.instance.lockText[i].text = "×";
                GlobalVariable.instance.lockText[i].color = Color.red;
            }
        }
    }

    private string Undo0(string[] args)
    {
        if (args.Length != 0)
            return "Error:No need for arguments";
        if (GlobalVariable.instance.history.Count == 0)
            return "Error:No operate history";
        List<Vector2Int> toChange = GlobalVariable.instance.history.Pop();
        foreach (Vector2Int vector2Int in toChange)
        {
            ChangeGuess(vector2Int.x, vector2Int.y);
        }
        GlobalVariable.instance.redo.Push(toChange);
        LockCheck();
        return "Undo success";
    }
    private string Redo(string[] args)
    {
        if (args.Length != 0)
            return "Error:No need for arguments";
        if (GlobalVariable.instance.redo.Count == 0)
            return "Error:No undo history";
        List<Vector2Int> toChange = GlobalVariable.instance.redo.Pop();
        foreach (Vector2Int vector2Int in toChange)
        {
            ChangeGuess(vector2Int.x, vector2Int.y);
        }
        GlobalVariable.instance.history.Push(toChange);
        LockCheck();
        return "Redo success";
    }
    private string Auto(string[] args)
    {
        if (args.Length != 0)
            return "Error:No need for arguments";
        string cipherText = GlobalVariable.instance.cipherText.text;
        Guess[] guesskey = GlobalVariable.instance.guessKey;
        string command = Suggest.GetSuggest(cipherText, guesskey, GlobalVariable.instance.nGramFreqs, 1);
        Execute(command);
        return "Auto execute：" + command;
    }
}
