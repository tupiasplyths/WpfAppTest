using Python.Runtime;

namespace WpfAppTest;

public class MangaOCR
{
    private dynamic? OCR;

    public MangaOCR()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Path to python dll 
        // NOTE: Anaconda python dll does not work for some reason
        Runtime.PythonDLL = "C:/Users/Phere/AppData/Local/Programs/Python/Python312/python312.dll";
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            dynamic module = Py.Import("manga_ocr");
            OCR = module.MangaOcr();
        }
    }
    public string GetTextFromOCR(string image_path)
    {
        using (Py.GIL())
        {
            Console.WriteLine("Python Version: " + PythonEngine.Version);
            string text = OCR(image_path);
            return text;
        }
    }

    public void CleanUp()
    {
        // Shutdown the Python engine when done
        PythonEngine.Shutdown();
    }
}