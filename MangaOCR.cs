using Python.Runtime;

namespace WpfAppTest;

public class MangaOCR
{
    private dynamic? OCR;
    private dynamic? customOCR;

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
            Console.WriteLine("Python initialized");
            dynamic os = Py.Import("os");
            dynamic sys = Py.Import("sys");
            sys.path.append(os.getcwd());
            Console.WriteLine(sys.path);
            // dynamic module = Py.Import("manga_ocr");
            dynamic custom_ocr = Py.Import("custom_ocr");
            // OCR = module.MangaOcr();
            customOCR = custom_ocr.CustomOCR();

        }
    }
    public string GetTextFromOCR(string image_path)
    {
        using (Py.GIL())
        {
            string text = OCR(image_path);
            return text;
        }
    }

    public string GetTextFromCustomOCR(string image_path)
    {
        using (Py.GIL())
        {
            return customOCR(image_path);
        }
    }

    public void CleanUp()
    {
        // Shutdown the Python engine when done
        PythonEngine.Shutdown();
    }
}