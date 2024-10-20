using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class OnFieldClick : MonoBehaviour
{
    public GameObject[] Field;
    [SerializeField] Sprite x;
    [SerializeField] Sprite o;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject XorOMenu;
    private int numberOfClicks;
    private Color trans = new (1, 1, 1, 1);
    private bool EndGame;
    public StateOfGame GameState;
    public Sprite Player;
    public Sprite Computer;
    private bool firstMove=true;

    public enum StateOfGame
    {
        SinglePlayer,
        MultiPlayer
    }

    public enum PlayerValues
    {
        X,
        O
    }

    private void SetStateOfGame(StateOfGame stateOfGame)
    {
        GameState = stateOfGame;
    }

    public void SetSinglePlayer()
    {
        SetStateOfGame(StateOfGame.SinglePlayer);
        CloseMainMenu();
        SetActiveXorOMenu(true);
    }

    public void SetMultiPlayer()
    {
        SetStateOfGame(StateOfGame.MultiPlayer);
        CloseMainMenu();
    }

    private void SetActiveXorOMenu(bool value)
    {
        XorOMenu.SetActive(value);
    }

    private void CloseMainMenu()
    {
        MainMenu.SetActive(false);
    }

    public void SetXValueForComputer()
    {
        Computer = x;
        Player = o;
        SetActiveXorOMenu(false);
        MakingComputersMove();
    }
    public void SetOValueForComputer()
    {
        Computer = o;
        Player = x;
        SetActiveXorOMenu(false);
    }

    private void Start()
    {
        numberOfClicks = 0;
    }

    private bool HasTheSameValue(int firstField, int secondField, int thirdField)
    {
        if (AreAllFieldsFilled(firstField,secondField,thirdField) && AreAllImagesTheSame(firstField,secondField,thirdField))
            return true;
        else
            return false;
    }

    private bool AreAllImagesTheSame(int firstField, int secondField, int thirdField)
    {
        if (Field[firstField].GetComponent<Image>().sprite == Field[secondField].GetComponent<Image>().sprite
            && Field[thirdField].GetComponent<Image>().sprite == Field[firstField].GetComponent<Image>().sprite)
            return true;
        else
            return false;
    }

    private bool AreAllFieldsFilled(int firstField, int secondField, int thirdField)
    {
        if (Field[firstField].GetComponent<Field>().IsClicked && Field[secondField].GetComponent<Field>().IsClicked 
            && Field[thirdField].GetComponent<Field>().IsClicked)
            return true;
        else
            return false;
    }

    private void CheckIfSomeoneHasWonOrItsTie()
    {
        var winningCombinations = new (int[] indices, int x1, int y1, int x2, int y2, int z2)[]
        {
        (new int[] {0, 1, 2}, 0, 0, 90, 0, 290),
        (new int[] {0, 4, 8}, 0, 0, 41, 0, 0),
        (new int[] {0, 3, 6}, 0, 0, 0, -300, 0),
        (new int[] {3, 4, 5}, 0, 0, 90, 0, 0),
        (new int[] {6, 7, 8}, 0, 0, 90, 0, -310),
        (new int[] {6, 4, 2}, 0, 0, -41, 0, 0),
        (new int[] {1, 4, 7}, 0, 0, 0, 0, 0),
        (new int[] {2, 5, 8}, 0, 0, 0, 300, 0)
        };

        foreach (var (indices, x1, y1, x2, y2, z2) in winningCombinations)
        {
            if (HasTheSameValue(indices[0], indices[1], indices[2]))
            {
                StartCoroutine(EndGameAndSetLine(x1, y1, x2, y2, z2, 0));
                break;
            }
        }
        foreach(var f in Field)
        {
            if (!f.GetComponent<Field>().IsClicked)
                return;
        }
        EndGame = true;
    }
    private bool CheckWinningCondition(Func<int, int, int, bool> conditionCheck)
    {
        int[][] winningCombinations = new int[][]
        {
        new int[] { 0, 1, 2 },
        new int[] { 0, 4, 8 },
        new int[] { 0, 3, 6 },
        new int[] { 3, 4, 5 },
        new int[] { 6, 7, 8 },
        new int[] { 6, 4, 2 },
        new int[] { 1, 4, 7 },
        new int[] { 2, 5, 8 }
        };

        foreach (var combination in winningCombinations)
        {
            if (conditionCheck(combination[0], combination[1], combination[2]))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckIfPlayerIsNearToWin()
    {
        return CheckWinningCondition(CheckIfThereIsOneMoveFromWinning);
    }

    private IEnumerator EndGameAndSetLine(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        EnableFieldsToPress();
        EndGame = true;
        yield return new WaitForSeconds(0.7f);
        FinalLineController.Instance.SetFinalLine(x1, y1, z1, x2, y2, z2);
    }

    private void EnableFieldsToPress()
    {
        foreach(var field in Field)
        {
            field.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnClickedField(GameObject field)
    {
        var obj = field.GetComponent<Field>();
        if (obj.IsClicked)
            return;
        StartCoroutine(ApperingXorO(obj,0f));
        CheckIfSomeoneHasWonOrItsTie();
        if (GameState == StateOfGame.SinglePlayer & !EndGame)
        {
            MakingComputersMove();
        }
    }
    private IEnumerator ApperingXorO(Field f, float timeOfWaiting)
    {
        if (numberOfClicks % 2 == 0)
        {
            f.Image.sprite = x;
        }
        else
        {
            f.Image.sprite = o;
        }
        numberOfClicks++;
        f.IsClicked = true;
        yield return new WaitForSeconds(timeOfWaiting);
        f.Image.color = trans;
    }

    private void MakingComputersMove()
    {
        if (EndGame)
            return;
        int randomNumber;
        GameObject chosenField;
        if (firstMove)
        {
            do
            {
                randomNumber = Random.Range(0, 9);
                chosenField = Field[randomNumber];
            } while (Field[randomNumber].GetComponent<Field>().IsClicked);
            StartCoroutine(ApperingXorO(chosenField.GetComponent<Field>(),0.3f));
            firstMove = false;
        }
        else
        {
            if (!MakeComputerMoveForWin())
            {
                if (!CheckIfPlayerIsNearToWin())
                    if (!MakeComputerMove())
                        FindFirstEmptySpace();
            }
            
        }
        CheckIfSomeoneHasWonOrItsTie();
    }

    private void FindFirstEmptySpace()
    {
        GameObject chosenField = null;
        foreach(var f in Field)
        {
            if (!f.GetComponent<Field>().IsClicked)
            {
                chosenField = f;
            }
        }
        StartCoroutine(ApperingXorO(chosenField.GetComponent<Field>(), 0.3f));
    }

    private bool MakeComputerMove()
    {
        return CheckWinningCondition(DoMoveForComputer);
    }
    private bool MakeComputerMoveForWin()
    {
        return CheckWinningCondition(DoMoveForComputersWin);
    }

    private bool CheckIfThereIsOneMoveFromWinning(int first,int second, int third)
    {
        if (((Field[first].GetComponent<Field>().Image.sprite == Player && Field[second].GetComponent<Field>().Image.sprite == Player)
            || (Field[first].GetComponent<Field>().Image.sprite == Player && Field[third].GetComponent<Field>().Image.sprite == Player)
            || (Field[second].GetComponent<Field>().Image.sprite == Player && Field[third].GetComponent<Field>().Image.sprite == Player))
            && (!Field[first].GetComponent<Field>().IsClicked || !Field[second].GetComponent<Field>().IsClicked
            || !Field[third].GetComponent<Field>().IsClicked))
        {
            SelectComputersNextMove(first, second, third);
            return true;
        }
        else
            return false;
    }

    private bool DoMoveForComputer(int first, int second, int third)
    {
        if ((Field[first].GetComponent<Field>().Image.sprite == Computer && !Field[second].GetComponent<Field>().IsClicked &&
            !Field[third].GetComponent<Field>().IsClicked)
            || (Field[second].GetComponent<Field>().Image.sprite == Computer && !Field[first].GetComponent<Field>().IsClicked &&
            !Field[third].GetComponent<Field>().IsClicked)
            || (Field[third].GetComponent<Field>().Image.sprite == Computer && !Field[first].GetComponent<Field>().IsClicked &&
            !Field[second].GetComponent<Field>().IsClicked))
        {
            SelectComputersNextMove(first, second, third);
            return true;
        }
        else
            return false;
    }
    private bool DoMoveForComputersWin(int first, int second, int third)
    {
        if ((Field[first].GetComponent<Field>().Image.sprite == Computer && (!Field[second].GetComponent<Field>().IsClicked ||
            !Field[third].GetComponent<Field>().IsClicked) && (Field[second].GetComponent<Field>().Image.sprite == Computer ||
            Field[third].GetComponent<Field>().Image.sprite == Computer))
            || (Field[second].GetComponent<Field>().Image.sprite == Computer && (!Field[first].GetComponent<Field>().IsClicked ||
            !Field[third].GetComponent<Field>().IsClicked) && (Field[first].GetComponent<Field>().Image.sprite == Computer ||
            Field[third].GetComponent<Field>().Image.sprite == Computer))
            || (Field[third].GetComponent<Field>().Image.sprite == Computer && (!Field[second].GetComponent<Field>().IsClicked ||
            !Field[first].GetComponent<Field>().IsClicked) && (Field[second].GetComponent<Field>().Image.sprite == Computer ||
            Field[first].GetComponent<Field>().Image.sprite == Computer)))
        {
            SelectComputersNextMove(first, second, third);
            return true;
        }
        else
            return false;
    }

    private void SelectComputersNextMove(int first, int second, int third)
    {
        List<GameObject> list = new()
            {
                Field[first],
                Field[second],
                Field[third]
            };
        foreach (GameObject obj in list)
        {
            if (!obj.GetComponent<Field>().IsClicked)
            {
                StartCoroutine(ApperingXorO(obj.GetComponent<Field>(), 0.3f));
                list.Clear();
                return;
            }
        }
        return;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("PlayerVSComputer");
        print("da");
    }

}
