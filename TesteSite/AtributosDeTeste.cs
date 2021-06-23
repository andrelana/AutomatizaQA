using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TesteSite
{
    public class AtributosDeTeste : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            TestResult[] results = base.Execute(testMethod);

            foreach (TestResult result in results)
            {
                if (result.Outcome == UnitTestOutcome.Failed)
                {
                    using (StreamWriter sw = new StreamWriter(Hooks.Report, true))
                    {
                        sw.WriteLine($"<h4 style='color:red;'>{result.TestFailureException.Message}</h4>");
                        sw.WriteLine($"<h4 style='color:red;'>{result.TestFailureException.InnerException}</h4>");
                        sw.WriteLine($"<h4 style='color:red;'>{result.TestFailureException.StackTrace}</h4>");
                    }
                }
            }

            return results;
        }
    }
}
