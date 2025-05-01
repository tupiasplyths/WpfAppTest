
import pytest
from PIL import Image
from custom_ocr.ocr import CustomOCR


class TestCustomOCR:
    def test_init(self):
        ocr = CustomOCR()
        assert ocr is not None

    def test_call(self):
        ocr = CustomOCR()
        # Create a dummy image for testing
        img = Image.open("E:\\code\\VS_junk\\WpfAppTest\\output\\202504231340568943.png")
        result = ocr(img)
        print(result)
        assert result == "ああぁぁ！！"
