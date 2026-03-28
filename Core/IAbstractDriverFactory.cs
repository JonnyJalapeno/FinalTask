using OpenQA.Selenium;

namespace Core
{
    public interface IAbstractDriverFactory
    {
        IWebDriver CreateDriver();
    }
}
