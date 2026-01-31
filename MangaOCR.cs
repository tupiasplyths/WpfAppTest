using Python.Runtime;

namespace WpfAppTest;

public class MangaOCR
{
    private dynamic? OCR;
    private dynamic? customOCR;
    private static bool _isInitialized = false;
    private static readonly object _lock = new();
    private static MangaOCR? _instance;

    private MangaOCR()
    {
        // Don't initialize in constructor - do it lazily
    }

    public static MangaOCR Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MangaOCR();
                    }
                }
            }
            return _instance;
        }
    }

    public void Initialize()
    {
        lock (_lock)
        {
            if (_isInitialized)
            {
                Console.WriteLine("MangaOCR already initialized, skipping...");
                return;
            }

            try
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

                    // Import and initialize OCR models
                    // Temporarily disabled for testing - manga_ocr loads models that cause freezing
                    // dynamic manga_ocr = Py.Import("manga_ocr");
                    dynamic custom_ocr = Py.Import("custom_ocr");

                    // OCR = manga_ocr.MangaOcr();
                    customOCR = custom_ocr.CustomOCR();

                    _isInitialized = true;
                    Console.WriteLine("MangaOCR initialization complete");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing MangaOCR: {ex.Message}");
                throw;
            }
        }
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            Initialize();
        }
    }
    public string GetTextFromOCR(string image_path)
    {
        // Temporarily disabled for testing - manga_ocr loads models that cause freezing
        throw new InvalidOperationException("Manga OCR is disabled for testing. Please use Custom OCR instead.");
        /*
        EnsureInitialized();
        using (Py.GIL())
        {
            string text = OCR(image_path);
            Console.WriteLine("Got text from OCR");
            return text;
        }
        */
    }

    public string GetTextFromCustomOCR(string image_path)
    {
        EnsureInitialized();
        using (Py.GIL())
        {
            return customOCR(image_path);
        }
    }

    public static void CleanUp()
    {
        lock (_lock)
        {
            if (_isInitialized)
            {
                // Shutdown the Python engine when done
                PythonEngine.Shutdown();
                _isInitialized = false;
                Console.WriteLine("Python engine shut down");
            }
        }
    }
}