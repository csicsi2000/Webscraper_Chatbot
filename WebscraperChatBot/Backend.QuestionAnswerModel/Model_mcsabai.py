# Only hungarian
from transformers import AutoTokenizer
from transformers import pipeline

question_answerer = pipeline(
    "question-answering",
    model="mcsabai/huBert-fine-tuned-hungarian-squadv2",
    tokenizer="mcsabai/huBert-fine-tuned-hungarian-squadv2",
    topk = 1,
    handle_impossible_answer = True
)