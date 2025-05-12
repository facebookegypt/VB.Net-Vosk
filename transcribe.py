import sys
from vosk import Model, KaldiRecognizer
import wave
import json

# Check for correct number of arguments
if len(sys.argv) != 4:
    print("Usage: python transcribe.py <audio_file> <output_file> <model_path>")
    sys.exit(1)

# Get arguments
audio_file = sys.argv[1]
output_file = sys.argv[2]
model_path = sys.argv[3]

# Load the Vosk model from the provided path
model = Model(model_path)

# Example transcription code (adjust as needed)
wf = wave.open(audio_file, "rb")
rec = KaldiRecognizer(model, wf.getframerate())
while True:
    data = wf.readframes(4000)
    if len(data) == 0:
        break
    if rec.AcceptWaveform(data):
        result = json.loads(rec.Result())
        with open(output_file, "a") as f:
            f.write(result.get("text", "") + "\n")
final_result = json.loads(rec.FinalResult())
with open(output_file, "a") as f:
    f.write(final_result.get("text", "") + "\n")