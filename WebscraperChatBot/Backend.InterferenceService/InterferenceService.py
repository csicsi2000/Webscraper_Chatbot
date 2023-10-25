from flask import Flask, request, jsonify
from transformers import DebertaForQuestionAnswering, DebertaTokenizer
from transformers import AutoTokenizer
from transformers import pipeline

app = Flask(__name__)

model_checkpoint = "timpal0l/mdeberta-v3-base-squad2"
tokenizer = AutoTokenizer.from_pretrained(model_checkpoint)

question_answerer = pipeline("question-answering", model=model_checkpoint)

@app.route('/answer', methods=['POST'])
def answer_question():
    data = request.get_json()
    question = data['question']
    context = data['context']
    

if __name__ == '__main__':
    app.run(debug=True)