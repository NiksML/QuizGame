using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System;
using static System.Net.Mime.MediaTypeNames.Application;


public class QuestionLoader : MonoBehaviour
{
    public GameObject _startGame, _questionPanel, _resultGame, _trueOrFalse, _nextQuestion;
    public Text _questionText, _trueOrFalseText, _answer1, _answer2, _answer3, _answer4;
    public GameObject[] answ;
    public Image bg;
    private static int count_of_questions = 0;
    private static int rightAnswers = 0;

    public class Question
    {
        public string questionText { get; set; }
        public Answer[] answers { get; set; }
        public string background { get; set; }
    }

    void Start()
    {
        
    }

    public class Answer
    {
        public string text { get; set; }
        public bool correct { get; set; }
    }

    public void NewGame()
    {
		count_of_questions = 0;
        rightAnswers = 0;
        _nextQuestion.transform.GetChild(0).GetComponent<Text>().text = "Next question";
        bg.sprite = Resources.Load<Sprite>("default");
        DidButtonsInteractOrNot(true);
        _startGame.SetActive(true);
        _questionPanel.SetActive(false);
        _resultGame.SetActive(false);
    }

    public void StartGame()
    {
        Question[] result = Read();
        _startGame.SetActive(false);
        _questionPanel.SetActive(true);
        _nextQuestion.SetActive(false);
        _trueOrFalse.SetActive(false);
        _questionText.text = result[0].questionText;
        _answer1.text = result[0].answers[0].text;
        _answer2.text = result[0].answers[1].text;
        _answer3.text = result[0].answers[2].text;
        _answer4.text = result[0].answers[3].text;
        var back = result[0].background;
        bg.sprite = Resources.Load<Sprite>(back);
    }

    public void NextQuestion()
    {
        Question[] result = Read();
		count_of_questions++;
        //Debug.Log(count_of_questions);
        if (count_of_questions < result.Length)
        {
            var back = result[count_of_questions].background;
            bg.sprite = Resources.Load<Sprite>(back);
            _trueOrFalse.SetActive(false);
            _questionText.text = result[count_of_questions].questionText;
            _answer1.text = result[count_of_questions].answers[0].text;
            _answer2.text = result[count_of_questions].answers[1].text;
            _answer3.text = result[count_of_questions].answers[2].text;
            _answer4.text = result[count_of_questions].answers[3].text;
            DidButtonsInteractOrNot(true);
            _nextQuestion.SetActive(false);
        }
        else if(count_of_questions == result.Length)
        {
            _questionPanel.SetActive(false);
            _resultGame.SetActive(true);
            _resultGame.transform.GetChild(1).GetComponent<Text>().text = "Right answers: " + rightAnswers.ToString();
        }
    }

    public void ChooseAnswer(int n)
    {
        Question[] result = Read();
        _trueOrFalse.SetActive(true);
        if (result[count_of_questions].answers[n].correct == true)
        {
            _trueOrFalseText.text = "True";
            _trueOrFalseText.color = new Color(0,255,0);
            rightAnswers++;
        }
        else
        {
            _trueOrFalseText.text = "False";
            _trueOrFalseText.color = new Color(255, 0, 0);
        }
        DidButtonsInteractOrNot(false);
        _nextQuestion.SetActive(true);
        if(count_of_questions == result.Length-1)
        {
            _nextQuestion.transform.GetChild(0).GetComponent<Text>().text = "Show results";
        }
    }

    private void DidButtonsInteractOrNot(bool t)
    {
        for (int i = 0; i < answ.Length; i++)
        {
            answ[i].GetComponent<Button>().interactable = t;
        }
    }

    public Question[] Read()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "JsonData.json");
        if (File.Exists(filePath))
        {
            
            string json = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<dynamic>(json);

            Question[] questions = new Question[data.Count];

            for (int i = 0; i < data.Count; i++)
            {
                Question newQuestion = new Question();
                newQuestion.questionText = data[i].question;

                Answer[] answers = new Answer[data[i].answers.Count];
                for (int j = 0; j < data[i].answers.Count; j++)
                {
                    Answer newAnswer = new Answer();
                    newAnswer.text = data[i].answers[j].text;
                    newAnswer.correct = data[i].answers[j].correct;
                    answers[j] = newAnswer;
                }

                newQuestion.answers = answers;
                newQuestion.background = data[i].background;

                questions[i] = newQuestion;
            }
            return questions;
        }
        else
        {
            //Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
}
