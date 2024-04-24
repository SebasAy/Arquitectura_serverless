using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreEntryPrefab;

    private List<ScoreEntry> leaderboard;

    [SerializeField]
    private int VerticalOffset = 90;
    [SerializeField]
    private int VerticalSpace = 25;
    private RectTransform scoreEntryContainer;
    private Vector2 originalPrefabSize;
    private Vector2 originalPrefabPosition;

    void Start()
    {
        leaderboard = new List<ScoreEntry>();
        scoreEntryContainer = GetComponent<RectTransform>();

        // Obtener el tamaño y posición original del prefab
        originalPrefabSize = scoreEntryPrefab.GetComponent<RectTransform>().sizeDelta;
        originalPrefabPosition = scoreEntryPrefab.GetComponent<RectTransform>().anchoredPosition;

        FirebaseDatabase.DefaultInstance
            .GetReference("users").OrderByChild("score").LimitToLast(5) // Limitar a los 5 mejores puntajes
            .ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    { 
    Debug.Log("ValueChanged");
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            return;
        }

DataSnapshot snapshot = args.Snapshot;

int i = 0;

var _leaderboard = gameObject.GetComponentsInChildren<ScoreEntry>();
foreach (var go in _leaderboard)
{
    Destroy(go.gameObject);
}

var leaderboarDictionary = (Dictionary<string, object>)snapshot.Value;
var _leaderborard = leaderboarDictionary.Values.OrderByDescending(x => int.Parse("" + ((Dictionary<string, object>)x)["score"]));
foreach (var userDoc in _leaderborard)
{
    var userObject = (Dictionary<string, object>)userDoc;

    var go = GameObject.Instantiate(scoreEntryPrefab, transform);
    go.transform.position = new Vector2(go.transform.position.x, VerticalOffset - (VerticalSpace * i));
    go.GetComponent<ScoreEntry>().SetLabels("" + userObject["username"], "" + userObject["score"]);
    i++;
}
    }
}
