using OpenQA.Selenium;
using System.Runtime.InteropServices;

namespace Core.Extensions
{
    public static class WebElementExtensions
    {
        //Regular .Clear() is unreliable if there's JS/React managing element text/state. 
        public static void ClearElement(this IWebElement element)
        {
            string selectAll;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                selectAll = Keys.Command + "a" + Keys.Null;
            }
            else
            {
                selectAll = Keys.Control + "a" + Keys.Null;
            }

            element.SendKeys(selectAll);
            element.SendKeys(Keys.Delete);

            if (!string.IsNullOrEmpty(element.GetAttribute("value")))
            {
                throw new InvalidOperationException($"Failed to clear element — value still present: '{element.GetAttribute("value")}'");
            }
        }
    }
}