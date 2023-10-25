from transformers import AutoTokenizer
from transformers import pipeline

model_checkpoint = "timpal0l/mdeberta-v3-base-squad2"
tokenizer = AutoTokenizer.from_pretrained(model_checkpoint)

question_answerer = pipeline("question-answering", model=model_checkpoint)