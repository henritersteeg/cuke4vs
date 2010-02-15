using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace StepDefinitions
{

    public class SpecFlowClass
    {
        [Given("My Specflow phone number is (.*)")]
        public static void SpecFlowPhone(string phone)
        {
           ScenarioContext.Current.Pending();
        }
    }
}
