import pytest
from ocr import CustomOCR


class TestCustomOCR:
    # def __init__(self):
    #     self.ocr = CustomOCR()
    #     assert self.ocr is not None

    def test_call(self):
        ocr = CustomOCR()
        assert ocr is not None

        # Create a dummy image for testing
        # img = Image.open("E:\\code\\VS_junk\\WpfAppTest\\output\\202504231340568943.png")
        img_path = "E:\\code\\J2E_OCR\\images\\color1.png"
        result = ocr(img_path)
        print(result)
        # assert result == "ああぁぁ！！"
