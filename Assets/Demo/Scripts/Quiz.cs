
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Quiz
{
    public int level;
    public string letters;
    public string[] words;
}

[Serializable]
public struct QuizzesData:IEnumerable<Quiz>
{
    [SerializeField] private List<Quiz> _quizzes;

    public IEnumerator<Quiz> GetEnumerator()
    {
        return _quizzes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}