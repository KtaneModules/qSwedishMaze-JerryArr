using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;

public class qSwedishMaze : MonoBehaviour
{
    public KMSelectable[] buttons;
    public MeshRenderer[] meshNum;
    public KMBombInfo bomb;
    public KMSelectable swedish;

    //public new KMAudio audio;

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

    // TWITCH PLAYS SUPPORT
    int tpStages;
    // TWITCH PLAYS SUPPORT

    Color[] colory = { new Color(0.0f, 0.4f, 0.75f), new Color(1f, 0.85f, 0f) };

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        Init();
        //GetComponent<KMBombModule>().OnActivate += ActivateModule;
    }

    void Init()
    {
        tpStages = 0;
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
                Debug.LogFormat("[IKEA #{0}] Button number {1} is {2} and is a product: {3}.", _moduleId, i + 1, isYellow[i] == 1? "yellow" : "blue", pickedFurniture);
				
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
        buttons[0].OnInteractEnded += delegate () { OnRelease(); };
        buttons[1].OnInteractEnded += delegate () { OnRelease(); };
        buttons[2].OnInteractEnded += delegate () { OnRelease(); };
        buttons[3].OnInteractEnded += delegate () { OnRelease(); };
        buttons[4].OnInteractEnded += delegate () { OnRelease(); };
        //swedish.OnInteractEnded += delegate () { OnRelease(); swedish.AddInteractionPunch(0.3f); };
        swedish.OnInteractEnded += delegate () { OnRelease(); };

        /* buttons [0].OnInteract += delegate () { buttons[0].AddInteractionPunch(0.2f); ChangeDisplay(0, -1); return false; }; */

        buttons[0].OnInteract += delegate () { OnPress(0); buttons[0].AddInteractionPunch(0.2f); return false; };
        buttons[1].OnInteract += delegate () { OnPress(1); buttons[1].AddInteractionPunch(0.2f); return false; };
        buttons[2].OnInteract += delegate () { OnPress(2); buttons[2].AddInteractionPunch(0.2f); return false; };
        buttons[3].OnInteract += delegate () { OnPress(3); buttons[3].AddInteractionPunch(0.2f); return false; };
        buttons[4].OnInteract += delegate () { OnPress(4); buttons[4].AddInteractionPunch(0.2f); return false; };

        //swedish.OnInteract += delegate () { pressFlag(); swedish.AddInteractionPunch(0.3f); return false; };
        swedish.OnInteract += delegate () { OnPress(9); swedish.AddInteractionPunch(0.2f); return false; };

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

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Press the correct buttons with !{0} press 1 2 3 4 5 with a space in between numbers. You can substitute 'press' with 'p'. Reset your progess with !{0} reset/r/flag/f";
    private readonly bool TwitchShouldCancelCommand = false;
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var pieces = command.Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        string theError;

        //Debug.Log(pieces.Count());
        //Debug.Log(pieces.Length);

        if (pieces.Count() == 1 && pieces[0] != "flag" && pieces[0] != "f" && pieces[0] != "reset" && pieces[0] != "r")
        {
            theError = "sendtochaterror Invalid argument! To submit you need at least 'press' or 'p', then one or more numbers, separated by spaces. To reset you need 'flag', 'f', 'reset', or 'r'.";
            yield return theError;
        }
        else if (pieces.Count() == 0)
        {
            theError = "sendtochaterror No arguments! To submit you need at least 'press' or 'p', then one or more numbers, separated by spaces. To reset you need 'flag', 'f', 'reset', or 'r'.";
            yield return theError;
        }
        else if ((pieces.Count() > 1) && (pieces[0] == "press" || pieces[0] == "p"))
        {
            tpStages = pieces.Length - 1;
            //Debug.Log(pieces.Length - tpStages);
            while (tpStages > 0)
            {
                yield return new WaitForSeconds(.1f);
                if (pieces[pieces.Count() - tpStages] == "1" || pieces[pieces.Count() - tpStages] == "2" || pieces[pieces.Count() - tpStages] == "3"
                    || pieces[pieces.Count() - tpStages] == "4" || pieces[pieces.Count() - tpStages] == "5")
                {
                    var buttonPicked = Int32.Parse(pieces[pieces.Count() - tpStages]) - 1;
                    yield return null;
                    buttons[buttonPicked].OnInteract();
                    tpStages--;

                }
                else
                {
                    tpStages = 0;
                    theError = "sendtochaterror You made a boo boo! Previous stages entered, but 'press' command '" + pieces[(pieces.Length - tpStages) - 1] +
                        "' is invalid. You must 'press' a number from 1 to 5.";
                    yield return theError;
                }

            }
            yield return null;
        }
        else if (pieces[0] == "flag" || pieces[0] == "f" || pieces[0] == "reset" || pieces[0] == "r")
        {
            yield return null;
            swedish.OnInteract();
        }
        else if (pieces[0] != "press" && pieces[0] != "p")
        {
            theError = "sendtochaterror You made a boo boo! Command '" + pieces[0] + "' is invalid. You must 'press', 'p', 'flag', 'f', 'reset', or 'r'.";
            yield return theError;
        }
        else
        {
            tpStages = 0;
            theError = "sendtochaterror You didn't send any buttons to 'press'!";
            yield return theError;
        }

    }

   /* void pressFlag()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        
    }*/

    void OnPress(int pressedButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        if (pressedAllowed)
        {
            if (pressedButton == 9)
            {
                currentPath = "";
            }
            else
            {
                currentPath = currentPath + direction[pressedButton];
                if (currentPath.Length >= 2 && (currentPath.Substring(currentPath.Length - 2, 2) == "EW" || currentPath.Substring(currentPath.Length - 2, 2) == "WE" ||
                    currentPath.Substring(currentPath.Length - 2, 2) == "SN" || currentPath.Substring(currentPath.Length - 2, 2) == "NS"))
                {
                    currentPath = currentPath.Substring(0, currentPath.Length - 2);
                    //Debug.LogFormat("[IKEA #{0}] Went back. Current path = {1}", _moduleId, currentPath);
                }
                else
                {


                    if (correctPath.Substring(0, currentPath.Length) != currentPath)
                    {
                        Debug.LogFormat("[IKEA #{0}] Current path = {1}, Correct path (so far) = {2}", _moduleId, currentPath, correctPath.Substring(0, currentPath.Length));
                        Debug.LogFormat("[IKEA #{0}] Mismatch, that's a strike! (You pressed button {1}, which was {2}.) Resetting input.",
                            _moduleId, (pressedButton + 1), direction[pressedButton]);
                        tpStages = 0;
                        currentPath = "";
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                    if (currentPath == correctPath)
                    {
                        Debug.LogFormat("[IKEA #{0}] Current path = {1}, Correct path = {2}", _moduleId, currentPath, correctPath.Substring(0, currentPath.Length));
                        Debug.LogFormat("[IKEA #{0}] That's the finish line! Module solved at {1}!", _moduleId, bomb.GetFormattedTime());
                        tpStages = 0;
                        pressedAllowed = false;
                        GetComponent<KMBombModule>().HandlePass();
                    }

                }

                return;
            }
            
        }
    }

    void OnRelease()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);


    }





}
