using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GlobalVariable.instance.plainText.text = Decoding(GlobalVariable.instance.cipherText.text);
    }

    private string Decoding(string chipherText)
    {
        string plaintext = "";
        for(int i = 0; i < chipherText.Length; i++)
        {
            int ch = chipherText[i];
            if (ch >= 'a' && ch <= 'z')
            {
                ch = ch - 96;
                if (GlobalVariable.instance.key[ch] == 0)
                    plaintext += '?';
                else
                    plaintext += (char)(GlobalVariable.instance.key[ch] + 96);
            }
            else if (ch >= 'A' && ch <= 'Z')
            {
                ch = ch - 64;
                if (GlobalVariable.instance.key[ch] == 0)
                    plaintext += '?';
                else
                    plaintext += (char)(GlobalVariable.instance.key[ch] + 64);
            }
            else
            {
                plaintext += (char)ch;
                continue;
            }
        }
        return plaintext;
    }
}
