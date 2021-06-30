using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace TesteSite.PageObjects.Menu
{
    public class MenuPage
    {
        public static By MenuWebTable = By.XPath("//*[text()='WebTable']");
        public static By MenuSwitchTo = By.XPath("//*[text()='SwitchTo']");
        public static By MenuWidgets = By.XPath("//*[text()='Widgets']");
        public static By MenuInteractions = By.XPath("//*[text()='Interactions ']");
        public static By MenuVideo = By.XPath("//*[text()='Video']");
        public static By MenuWYSIWYG = By.XPath("//*[text()='WYSIWYG']");
        public static By MenuMore = By.XPath("//*[text()='More']");
    }
}