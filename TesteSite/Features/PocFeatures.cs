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
        #region CTs
        [AtributosDeTeste]
        [TestCategory("Valida")]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void ValidaAbertura(string usuario, string senha)
        {
            Report.LogTextBlueItalic("Dado: Que o site iniciou");
            Report.LogTextBlueItalic("Quando: Acessar a tela de Login");
            Report.LogTextBlueItalic("E: Informar os dados corretamente");
            Report.LogTextBlueItalic("Então: A página inicial será exibida");
            PocStep.validaAbertura(usuario, senha);
        }
        #endregion

        #region Massa Dados
        private static IEnumerable<object[]> GetTestData()
        {
            string usuario = "alexandre@inspell.com.br";
            string senha = "123456";

            yield return new[] { usuario, senha };
        }
        #endregion
    }
}
