from manga_ocr import MangaOcr

class OCR:
    manga_ocr = MangaOcr()

    def get_text(self, image_path):
        return manga_ocr.get_text(image_path)

def main():
    ocr = OCR()
    print(ocr.get_text("output/202412071304314721.png"))

if __name__ == "__main__":
    main()