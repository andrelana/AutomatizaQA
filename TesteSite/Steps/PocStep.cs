using System;
using System.Collections.Generic;
using System.Text;
using TesteSite.PageObjects;

namespace TesteSite.Steps
{
    public class PocStep : Hooks
    {
        public static void validaAbertura(string Texto)
        {
            Driver.FindElement(PocPage.FiltroBusca).SendKeys(Texto);
        }
    }
}
