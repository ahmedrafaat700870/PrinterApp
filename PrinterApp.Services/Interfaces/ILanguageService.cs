namespace PrinterApp.Services.Interfaces;

public interface ILanguageService
{
    string GetText(string key);
    void SetLanguage(string language);
    string GetCurrentLanguage();
    Dictionary<string, string> GetAllTexts();
}
