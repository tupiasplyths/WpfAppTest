using Python.Runtime;

namespace WpfAppTest;

public static class MangaOCR
{
    public static string GetTextFromOCR(string image_path)
    {
        Runtime.PythonDLL = "W:/Conda/python311.dll";
        PythonEngine.PythonHome = "W:/Conda";
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            dynamic manga_ocr = Py.Import("manga_ocr");
            return manga_ocr.get_text(image_path);
        }
    }
}