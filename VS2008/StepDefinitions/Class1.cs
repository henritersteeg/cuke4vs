using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cuke4Nuke.Framework;

namespace StepDefinitions
{
    public class Class1
    {
        [NotNull]
        public void Test() 
        {
        }

        [Given("^nothing$")]
        public void Nothing() {}

        [When("^this$")]
        public void This() {}

        [Then("^that$")]
        public void That() {}

        [Cuke4Nuke.Framework.Given("^nothing too$")]
        public void NothingToo() {}

        [Then("^\\d\"")]
        public void Escapes() {}

        [Given("^My phone number is (\\d+) and my name is '(.*)'$")]
        public void IAm(int phoneNumber, string name) { }
    }
}
