using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TesteSite.PageObjects;

namespace TesteSite.Steps
{
    public class PocStep : Hooks
    {
        public static void validaAbertura(string email, string senha)
        {
            Driver.FindElement(PocPage.CampoEmail).SendkeysCharByChar(email);
            Thread.Sleep(1000);
            Driver.FindElement(PocPage.CampoSenha).SendKeys(senha);
            Thread.Sleep(1000);
            Driver.FindElement(PocPage.CampoUnidades).Click();
            Thread.Sleep(1000);
            Driver.FindElement(PocPage.CampoUnidadeCentro).Click();
            Thread.Sleep(1000);
            Driver.FindElement(PocPage.BotaoLogar).Click();
            Assert.IsTrue(Driver.FindElement(PocPage.TituloDashboard).Displayed, "Valida se o login foi feito corretamente e se exibiu o dashboard");
        }
    }
}
