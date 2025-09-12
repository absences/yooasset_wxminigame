// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Category
{
    [SerializeField]private string _id;
    [SerializeField]private string _name;
    [SerializeField] private Sprite _image;
    [SerializeField] private TextAsset _quizData;
    [SerializeField] private int _unlockBrain;


    private List<Quiz> _quizzes;

    public int UnlockBrain => _unlockBrain;
    public Sprite Image => _image;
    public string ID => _id;
    public string Name => _name;
    public int TotalBrain => Grid * Grid * Quizzes.Count();
    public int Grid => Mathf.RoundToInt(Mathf.Sqrt(Quizzes.First().letters.Length));
    public IEnumerable<Quiz> Quizzes
    {
        get
        {
            
            if (_quizzes != null)
            {
                return _quizzes;
            }

            _quizzes = JsonUtility.FromJson<QuizzesData>(_quizData.text).ToList();
            for (var i = 0; i < _quizzes.Count; i++)
            {
                var quiz = _quizzes[i];
                quiz.level = i + 1;
                _quizzes[i] = quiz;
            }

            return _quizzes;
        }
    }
}