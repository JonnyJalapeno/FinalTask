using OpenQA.Selenium;

namespace Business
{
    public class InventoryPage :  BasePage
    {
        private readonly By burgerButton = By.CssSelector(".bm-burger-button");
        private readonly By swagLabsLogo = By.CssSelector(".app_logo");
        private readonly By shoppingCartIcon = By.CssSelector(".shopping_cart_link");
        private readonly By sortMenu = By.CssSelector(".product_sort_container");
        private readonly By inventoryList = By.CssSelector(".inventory_list");
        private readonly string urlPart = "inventory";

        public InventoryPage(IWebDriver driver) : base(driver)
        {
        }

        public override bool IsLoaded()
        {
            try
            {
                wait.Until(d => d.Url.Contains(urlPart));

                WaitForElement(burgerButton);
                WaitForElement(swagLabsLogo);
                WaitForElement(shoppingCartIcon);
                WaitForElement(sortMenu);
                WaitForElement(inventoryList);

                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}
