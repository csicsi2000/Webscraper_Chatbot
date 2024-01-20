// https://fgiasson.com/blog/index.php/2023/08/23/how-to-deploy-hugging-face-models-in-a-docker-container/
WORKDIR /app
FROM python:3.8-slim-buster
COPY requirements.txt .
COPY app.py
COPY download_models.py

RUN pip install --no-cache-dir --upgrade -r requirements.txt && python3 download_models.py
EXPOSE 5555
COPY . .
ENTRYPOINT [ "python3" ]
CMD ["app.py"]