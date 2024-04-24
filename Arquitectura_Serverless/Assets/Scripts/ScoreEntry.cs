using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreEntry : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _labelUsername;

    [SerializeField]
    private TMP_Text _labelScore;

    public void SetLabels(string username, string score)
    {
        _labelUsername.text = username;
        _labelScore.text = "" + score;
    }
}
