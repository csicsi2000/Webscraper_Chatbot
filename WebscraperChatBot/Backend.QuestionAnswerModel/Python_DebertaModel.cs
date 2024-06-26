﻿using Backend.QuestionAnswerModel.Data;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using log4net;
using Python.Runtime;
using System.Reflection;

namespace Backend.QuestionAnswerModel
{
    public class Python_DebertaModel : IQuestionAnswerModel
    {
        dynamic tokenizer;
        dynamic question_answerer;
        PyModule scope;

        string _folder;
        string _pythonCode;
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Path to the python Runtime.dll e.g.: python310.dll
        /// </summary>
        public Python_DebertaModel(string pythonDll) 
        {
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
            PythonEngine.Initialize();

            _folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _pythonCode = File.ReadAllText(Path.Combine(_folder, "Model_timpal01.py"));

        }

        public IAnswer AnswerFromContext(string context, string question)
        {
            using (var gil = Py.GIL())
            {
                scope = Py.CreateScope();
                try
                {
                    scope.Exec(_pythonCode);
                }
                catch (Exception ex)
                {
                    _log4.Error(ex);
                    _log4.Info("Python environment initialization failed. Hint: Check missing packages: torch, transformers");
                }
                scope.Set("question", question);
                scope.Set("context", context);
                scope.Exec("res = question_answerer(question=question, context=context)\n");
                scope.Exec("resAnswer = res[\"answer\"]");
                scope.Exec("resScore = res[\"score\"]");

                var answer = scope.Get("resAnswer")?.ToString()?.Trim() ?? "";
                var score = scope.Get("resScore")?.ToString()?.Trim() ?? "";
                _log4.Info($"Answer: {answer}, Score: {score}");
                return new ModelAnswer() {Answer = answer, Score = Convert.ToDouble(score.Replace('.', ',')) };
            }
        }
    }
}