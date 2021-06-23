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

        [TestMethod]
        public void ValidaAbertura()
        {
            string Texto = "Teste";
            PocStep.validaAbertura(Texto);
        }
    }
}
