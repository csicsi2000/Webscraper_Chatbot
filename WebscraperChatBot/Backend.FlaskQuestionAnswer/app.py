"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""

from flask import Flask, request, json
from transformers import AutoTokenizer, pipeline

app = Flask(__name__)
# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

@app.route('/')
def hello():
    """Renders a sample page."""
    return "Hello World!"

model_checkpoint = "timpal0l/mdeberta-v3-base-squad2"
tokenizer = AutoTokenizer.from_pretrained(model_checkpoint)

question_answerer = pipeline("question-answering", model=model_checkpoint)

@app.route('/interference', methods=['POST'])
def AnswerQuestion():
    data = request.get_json(force=True)
    question = data["question"]
    context = data["context"]
    res = question_answerer(question=question, context=context)
    return json.dumps(res)
    
    
if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', 'localhost')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    PORT = 54311
    app.run(HOST, PORT)