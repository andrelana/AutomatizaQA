using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesteSite.PageObjects;

namespace TesteSite.Steps
{
    public class PocStep : Hooks
    {
        public static void validaAbertura()
        {
            Driver.FindElement(PocPage.BotaoSkipLogin).Click();
            Assert.IsTrue(Driver.FindElement(PocPage.CabecalhoRegistro).Displayed, "Valida página correta após skip de login");
        }
    }
}
