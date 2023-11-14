# Multiple languages
from transformers import AutoTokenizer
from transformers import pipeline

model_checkpoint = "mcsabai/huBert-fine-tuned-hungarian-squadv2"
tokenizer = AutoTokenizer.from_pretrained(model_checkpoint)

question_answerer = pipeline("question-answering", model=model_checkpoint)