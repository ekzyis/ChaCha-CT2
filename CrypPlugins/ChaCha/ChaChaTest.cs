﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZXing.OneD;
using System;
using System.Text;

namespace Tests.TemplateAndPluginTests
{

    [TestClass]
    public class ChaChaTest
    { 

        public ChaChaTest()
        {
        }

        private void RunTests(TestVector[] vectors)
        {
            var pluginInstance = TestHelpers.GetPluginInstance("ChaCha");
            var scenario = new PluginTestScenario(pluginInstance, new[] { "InputData", "InputKey", "InputIV", ".Rounds", ".Version_" }, new[] { "OutputData" });

            foreach (TestVector vector in vectors)
            {
                object[] output = scenario.GetOutputs(new object[] { vector.input.HexToByteArray(), vector.key.HexToByteArray(), vector.iv.HexToByteArray(), vector.rounds, vector.version });
                string versionString = vector.version == 0 ? "IETF" : "DJB";
                Assert.AreEqual(vector.output.ToUpper(), output[0].ToHex().ToUpper(), string.Format("Unexpected value in test #{0} for ChaCha {1} version.", vector.n, versionString));
            }
        }

        [TestMethod]
        public void ChaChaTestMethodIETF()
        {
            RunTests(ietfTestvectors);
        }

        [TestMethod]
        public void ChaChaTestMethodDJB()
        {
            RunTests(djbTestvectors);
        }

        struct TestVector
        {
            public int n, rounds, version;
            public string input, key, iv, output;
        }
        // IETF Test vectors from https://tools.ietf.org/html/rfc7539
        readonly TestVector[] ietfTestvectors = new TestVector[] {
            new TestVector() {
                n=0,
                input="4c616469657320616e642047656e746c656d656e206f662074686520636c617373206f66202739393a204966204920636f756c64206f6666657220796f75206f6e6c79206f6e652074697020666f7220746865206675747572652c2073756e73637265656e20776f756c642062652069742e",
                key="000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                iv="000000000000004a00000000",
                output="6e2e359a2568f98041ba0728dd0d6981e97e7aec1d4360c20a27afccfd9fae0bf91b65c5524733ab8f593dabcd62b3571639d624e65152ab8f530c359f0861d807ca0dbf500d6a6156a38e088a22b65e52bc514d16ccf806818ce91ab77937365af90bbf74a35be6b40b8eedf2785e42874d",
                rounds=20,
                version=0,
            }
        };
        // DJB Test vectors from https://tools.ietf.org/html/draft-strombergson-chacha-test-vectors-00#page-4
        readonly TestVector[] djbTestvectors = new TestVector[] {
            // TC1: All zero key and IV.
            new TestVector()
            {
                n=0,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="00000000000000000000000000000000",
                iv="0000000000000000",
                output="e28a5fa4a67f8c5defed3e6fb7303486aa8427d31419a729572d777953491120b64ab8e72b8deb85cd6aea7cb6089a101824beeb08814a428aab1fa2c816081b8a26af448a1ba906368fd8c83831c18cec8ced811a028e675b8d2be8fce081165ceae9f1d1b7a975497749480569ceb83de6a0a587d4984f19925f5d338e430d",
                rounds=8,
                version=1,
            },
            new TestVector()
            {
                n=1,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="00000000000000000000000000000000",
                iv="0000000000000000",
                output="e1047ba9476bf8ff312c01b4345a7d8ca5792b0ad467313f1dc412b5fdce32410dea8b68bd774c36a920f092a04d3f95274fbeff97bc8491fcef37f85970b4501d43b61a8f7e19fceddef368ae6bfb11101bd9fd3e4d127de30db2db1b472e76426803a45e15b962751986ef1d9d50f598a5dcdc9fa529a28357991e784ea20f",
                rounds=12,
                version=1,
            },
            new TestVector()
            {
                n=2,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="00000000000000000000000000000000",
                iv="0000000000000000",
                output="89670952608364fd00b2f90936f031c8e756e15dba04b8493d00429259b20f46cc04f111246b6c2ce066be3bfb32d9aa0fddfbc12123d4b9e44f34dca05a103f6cd135c2878c832b5896b134f6142a9d4d8d0d8f1026d20a0a81512cbce6e9758a7143d021978022a384141a80cea3062f41f67a752e66ad3411984c787e30ad",
                rounds=20,
                version=1,
            },
            new TestVector()
            {
                n=3,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="0000000000000000000000000000000000000000000000000000000000000000",
                iv="0000000000000000",
                output="3e00ef2f895f40d67f5bb8e81f09a5a12c840ec3ce9a7f3b181be188ef711a1e984ce172b9216f419f445367456d5619314a42a3da86b001387bfdb80e0cfe42d2aefa0deaa5c151bf0adb6c01f2a5adc0fd581259f9a2aadcf20f8fd566a26b5032ec38bbc5da98ee0c6f568b872a65a08abf251deb21bb4b56e5d8821e68aa",
                rounds=8,
                version=1,
            },
            new TestVector()
            {
                n=4,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="0000000000000000000000000000000000000000000000000000000000000000",
                iv="0000000000000000",
                output="9bf49a6a0755f953811fce125f2683d50429c3bb49e074147e0089a52eae155f0564f879d27ae3c02ce82834acfa8c793a629f2ca0de6919610be82f411326be0bd58841203e74fe86fc71338ce0173dc628ebb719bdcbcc151585214cc089b442258dcda14cf111c602b8971b8cc843e91e46ca905151c02744a6b017e69316",
                rounds=12,
                version=1,
            },
            new TestVector()
            {
                n=5,
                input="0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                key="0000000000000000000000000000000000000000000000000000000000000000",
                iv="0000000000000000",
                output="76b8e0ada0f13d90405d6ae55386bd28bdd219b8a08ded1aa836efcc8b770dc7da41597c5157488d7724e03fb8d84a376a43b8f41518a11cc387b669b2ee65869f07e7be5551387a98ba977c732d080dcb0f29a048e3656912c6533e32ee7aed29b721769ce64e43d57133b074d839d531ed1f28510afb45ace10a1f4b794d6f",
                rounds=20,
                version=1,
            }
        };
    }
}
