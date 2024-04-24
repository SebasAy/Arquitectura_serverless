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
        scoreEntryContainer = GetComponent<RectTransform>(); // Aquí asignamos el contenedor de las entradas

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

        // Eliminar las entradas anteriores
        foreach (ScoreEntry entry in leaderboard)
        {
            Destroy(entry.gameObject);
        }
        leaderboard.Clear();

        int i = 0;

        var leaderboardData = snapshot.Children.Cast<DataSnapshot>()
            .OrderByDescending(x => Convert.ToInt32(x.Child("score").Value)).Take(5).ToList();

        foreach (var userDoc in leaderboardData)
        {
            var userObject = (Dictionary<string, object>)userDoc.Value;

            var go = GameObject.Instantiate(scoreEntryPrefab, scoreEntryContainer); // Usamos el contenedor de las entradas
            go.transform.localPosition = new Vector2(0f, VerticalOffset - (VerticalSpace * i)); // LocalPosition en lugar de Position
            go.GetComponent<ScoreEntry>().SetLabels("" + userObject["username"], "" + userObject["score"]);

            i++;
        }
    }
}
