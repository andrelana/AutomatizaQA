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
        public static By CampoEmail = By.Name("email");
        public static By CampoSenha = By.Name("senha");
        public static By CampoUnidades = By.Name("unidade");
        public static By CampoUnidadeCentro = By.XPath("//*[text()='Unidade Centro']");
        public static By BotaoLogar = By.XPath("//*[text()=' Logar ']");
        public static By TituloDashboard = By.XPath("//*[text()='DASHBOARD']");
    }
}
