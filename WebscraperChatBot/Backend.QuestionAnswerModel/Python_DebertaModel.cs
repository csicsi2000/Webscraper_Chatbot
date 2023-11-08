using Backend.QuestionAnswerModel.Data;
using General.Interfaces.Backend.Components;
using log4net;
using Python.Runtime;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Backend.QuestionAnswerModel
{
    public class Python_DebertaModel : IQuestionAnswerModel,IDisposable
    {
        Py.GILState _gil;
        dynamic tokenizer;
        dynamic question_answerer;
        PyModule scope;

        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Path to the python Runtime.dll e.g.: python310.dll
        /// </summary>
        public Python_DebertaModel(string pythonDll) 
        {
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
            PythonEngine.Initialize();
            _gil = Py.GIL();

            string code = File.ReadAllText("ModelInterference.py");

            scope = Py.CreateScope();
            try
            {
                scope.Exec(code);
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                _log4.Info("Python environment initialization failed. Hint: Check missing packages: torch, transformers");
            }
               
        }

        public string AnswerFromContext(string context, string question)
        {
            scope.Set("question", question);
            scope.Set("context", context);
            scope.Exec("res = question_answerer(question=question, context=context)\n");
            scope.Exec("resAnswer = res[\"answer\"]");

            return scope.Get("resAnswer")?.ToString()?.Trim() ?? "";
        }

        public void Dispose()
        {
            _gil.Dispose();
        }

        /*
         *             using (Py.GIL())
            {

            }
            // Create Tokenizer and tokenize the sentence.

            // Get the sentence tokens.
            var tokens = tokenizer.Tokenize(sentence);
            // Console.WriteLine(String.Join(", ", tokens));

            // Encode the sentence and pass in the count of the tokens in the sentence.
            var encoded = tokenizer.Encode(tokens.Count(), sentence);

            // Break out encoding to InputIds, AttentionMask and TypeIds from list of (input_id, attention_mask, type_id).
            var bertInput = new BertInput()
            {
                InputIds = encoded.Select(t => t.InputIds).ToArray(),
                AttentionMask = encoded.Select(t => t.AttentionMask).ToArray(),
                TypeIds = encoded.Select(t => t.TokenTypeIds).ToArray(),
            };

         */
    }
}