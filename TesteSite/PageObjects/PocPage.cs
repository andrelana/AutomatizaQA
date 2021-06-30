using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace TesteSite.PageObjects
{
    public class PocPage
    {
        public static By BotaoSkipLogin = By.Id("btn2");
        public static By CabecalhoRegistro = By.XPath("//h2[text()='Register']");
    }
}
