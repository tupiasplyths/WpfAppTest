from manga_ocr import MangaOcr

class OCR:
    manga_ocr = MangaOcr()

    def get_text(self, image_path):
        return manga_ocr.get_text(image_path)