using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;

public class qSwedishMaze : MonoBehaviour
{
    public KMSelectable[] buttons;
    public MeshRenderer[] meshNum;
    public KMBombInfo bomb;
    public new KMAudio audio;

    int furnitureButton;
    int furnitureNumber;
    int correctIndex;
    int solutionNumber;
	int[] isYellow = new int[5];
    string correctPath;
	string currentPath;
    string pickedFurniture;
    string[] pickedBrand = new string[5] { "X", "X", "X", "X", "X" };
	string[] direction = new string[5] { "X", "X", "X", "X", "X" };
	string[] usedList = new string[16];

    string[] furnitures = new string[8] { "SÄNG", "SKÅP", "STOL", "GARDINER", "SKRIVBORD", "FLÄKT", "TÄCKE", "HANDDUK" };
	string[] brandYellow = new string[16] { "FÄRG", "GODIS", "ÅTTAHÖRNING", "KÖTTBULLE", "DRAKE", "TROLLKARL", "OST", "SKO", "TVÄTTBJÖRN", "ENHÖRNING", "MÅLA", "SKJORTA", "VALP", "KÄNGA", "HÖRLURAR", "SKÅL" };
	string[] brandBlue = new string[16] { "KÖTTBULLE", "SKÅL", "OST", "TROLLKARL", "SKO", "KÄNGA", "GODIS", "VALP", "HÖRLURAR", "SKJORTA", "MÅLA", "FÄRG", "ENHÖRNING", "TVÄTTBJÖRN", "ÅTTAHÖRNING", "DRAKE" };
	string[] paths = new string[16] { "SSWWSWWNNNWN", "SSESSWWWNNNWSSSWSSS", "NWWSSSEEEENNNNNWN", "EESSESSWWWSW", "WWSSEESSWWSEESENNNNNNNW", "SWSESWSEEENNNNNN", "NNWWSSWWNNWSSWSEEEEEEEN", "WNWSWNWSSSEEEEEE", "WWSWWNNNEEENWWWNWWW", "SEENNNWWWWSSSSSES", "WWNNWNNEEENE", "EENNWWNNEENWWNWSSSSSSSE", "NNWNNEEESSSENNNENNN", "SSEENNEESSENNENWWWWWWWS", "ESENESENNNWWWWWW", "WSSEEENNNNWWWWWSW"};
	string[] directions = new string[8] { "NEWS", "SEWN", "NESW", "WNSE", "NWES", "ENSW", "WENS", "SWEN" };

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    bool pressedAllowed = false;

    Color[] colory = { new Color(0.0f, 0.4f, 0.75f), new Color(1f, 0.85f, 0f) };

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        Init();
    }

    void Init()
    {
		currentPath = "";
        solutionNumber = 0;
		furnitureButton = UnityEngine.Random.Range(0, 5);
        furnitureNumber = UnityEngine.Random.Range(0, 8);
        pickedFurniture = furnitures[furnitureNumber];        

        for (int i = 0; i < buttons.Length; i++)
        {	
			isYellow[i] = UnityEngine.Random.Range(0, 2);

            TextMesh buttonText = buttons[i].GetComponentInChildren<TextMesh>();
            meshNum[i].material.color = colory[isYellow[i]];
            buttons[i].GetComponentInChildren<TextMesh>().color = colory[1 - isYellow[i]];
            if (i == furnitureButton)

            {
                pickedBrand[i] = pickedFurniture;
                buttonText.text = pickedFurniture;
                Debug.LogFormat("[IKEA #{0}] Button number {1} is {2} and is furniture: {3}.", _moduleId, i + 1, isYellow[i] == 1? "yellow" : "blue", pickedFurniture);
				
				if (isYellow[i] == 1)
				{
					usedList = brandYellow;
				}
				else
				{
					usedList = brandBlue;
				}
            }

            else

            {
				
                pickedBrand[i] = brandYellow[UnityEngine.Random.Range(0, 16)];
                bool sameness = true;

                while (sameness && i != 0)
        
                    {
                    sameness = false;
                    pickedBrand[i] = brandYellow[UnityEngine.Random.Range(0, 16)];

                    for (int k = 0; k < i; k++)
                        {

                        if(pickedBrand[i] == pickedBrand[k])
                            {
                            sameness = true;
                            }

                        }
                    }
                buttonText.text = pickedBrand[i];
                Debug.LogFormat("[IKEA #{0}] Button number {1} is {2} and is a brand: {3}.", _moduleId, i + 1, isYellow[i] == 1 ? "yellow" : "blue", pickedBrand[i]);
            }
        }
        int curDirection = 0;
        buttons[0].OnInteractEnded += delegate () { OnRelease(0); buttons[0].AddInteractionPunch(0.2f); };
        buttons[1].OnInteractEnded += delegate () { OnRelease(1); buttons[1].AddInteractionPunch(0.2f); };
        buttons[2].OnInteractEnded += delegate () { OnRelease(2); buttons[2].AddInteractionPunch(0.2f); };
        buttons[3].OnInteractEnded += delegate () { OnRelease(3); buttons[3].AddInteractionPunch(0.2f); };
        buttons[4].OnInteractEnded += delegate () { OnRelease(4); buttons[4].AddInteractionPunch(0.2f); };
        for (int l = 0; l < 16; l++)
        {
            for (int m = 0; m < 5; m++)
            {
                if (pickedBrand[m] == usedList[l])
                {
                    direction[m] = directions[furnitureNumber].Substring(curDirection, 1);
                    Debug.LogFormat("[IKEA #{0}] Button number {1} is {2}.", _moduleId, m + 1, direction[m]);
                    if (isYellow[m] == 1)
                    {
                        solutionNumber = solutionNumber + (int)(.01 + Mathf.Pow(2, 3-curDirection));
                        //the .01 is to make sure it doesn't go like 7.9999999 and turn the solution number into 7 when it should be 8
                        //Thanks Floatbama
                    }
                    curDirection++;
                }
            }
        }
        Debug.LogFormat("[IKEA #{0}] Solution number is {1} (0-15), solution path is {2}.", _moduleId, solutionNumber, paths[solutionNumber]);
        correctPath = paths[solutionNumber];
        pressedAllowed = true;
    }

    void OnPress()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    void OnRelease(int pressedButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
        if (pressedAllowed)
        {
            currentPath = currentPath + direction[pressedButton];
            if (currentPath.Length >= 2 && (currentPath.Substring(currentPath.Length - 2, 2) == "EW" || currentPath.Substring(currentPath.Length - 2, 2) == "WE" || currentPath.Substring(currentPath.Length - 2, 2) == "SN" || currentPath.Substring(currentPath.Length - 2, 2) == "NS"))
            {
                currentPath = currentPath.Substring(0, currentPath.Length - 2);
                Debug.LogFormat("[IKEA #{0}] Went back. Current path = {1}", _moduleId, currentPath);
            }
            else
            {
                Debug.LogFormat("[IKEA #{0}] Current path = {1}, Correct path (so far) = {2}", _moduleId, currentPath, correctPath.Substring(0, currentPath.Length));

                if (correctPath.Substring(0, currentPath.Length) != currentPath)
                {
                    Debug.LogFormat("[IKEA #{0}] Mismatch, that's a strike! Resetting input.", _moduleId);
                    currentPath = "";
                    GetComponent<KMBombModule>().HandleStrike();
                }
                if (currentPath == correctPath)
                {
                    Debug.LogFormat("[IKEA #{0}] That's the finish line! Module solved at {1}!", _moduleId, bomb.GetFormattedTime());
                    pressedAllowed = false;
                    GetComponent<KMBombModule>().HandlePass();
                }

            }

            return;
        }

    }





}
