using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesteSite.Steps;

namespace TesteSite.Features
{
    [TestClass]
    public class PocFeatures : Hooks
    {
        [AtributosDeTeste]
        public void ValidaAbertura()
        {
            Report.LogTextBlueItalic("Dado: Que o site iniciou");
            Report.LogTextBlueItalic("Quando: Acessar a tela de Login");
            Report.LogTextBlueItalic("E: Clicar no botão 'Skip Sign in");
            Report.LogTextBlueItalic("Então: A página de Registro será exibida");
            PocStep.validaAbertura();
        }
    }
}
