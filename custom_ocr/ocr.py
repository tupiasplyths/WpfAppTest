from transformers import VisionEncoderDecoderModel, TrOCRProcessor, AutoTokenizer, GenerationMixin
from PIL import Image
import jaconv
import torch
import re
from loguru import logger
from pathlib import Path

model_path = "E:/code/J2E_OCR/model_output/checkpoint-2750"
is_local = True

class CustomOCR:
    def __init__(self):
        self.processor = TrOCRProcessor.from_pretrained("kha-white/manga-ocr-base")
        self.model = VisionEncoderDecoderModel.from_pretrained(model_path, local_files_only=is_local)
        self.tokenizer = AutoTokenizer.from_pretrained("kha-white/manga-ocr-base")
        self.model.cuda()
        
    def _preprocess(self, img):
        pixel_values = self.processor(img, return_tensors="pt").pixel_values
        return pixel_values
    
    def __call__(self, img_or_path):
        if isinstance(img_or_path, str) or isinstance(img_or_path, Path):
            img = Image.open(img_or_path)
        elif isinstance(img_or_path, Image.Image):
            img = img_or_path
        else:
            raise ValueError(f"img_or_path must be a path or PIL.Image, instead got: {img_or_path}")

        img = img.convert("RGB")
        x = self._preprocess(img)
        # x = self.model.generate(x[None].to(self.model.device), max_length=512).cpu()
        x = x.to(self.model.device)
        x = self.model.generate(x, max_length=512)
        x = self.processor.decode(x[0], skip_special_tokens=True)
        x = post_process(x)
        # print(x)
        logger.info(f"OCR result: {x}")
        return x


def post_process(text):
    text = "".join(text.split())
    text = text.replace("…", "...")
    text = re.sub("[・.]{2,}", lambda x: (x.end() - x.start()) * ".", text)
    text = jaconv.h2z(text, ascii=True, digit=True)

    return text