using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ChaChaVisualizationTests
{
    [TestClass]
    public class ActionCreatorTest
    {
        [TestMethod]
        public void TestSequential()
        {
            ActionCreator actionCreator = new ActionCreator();
            List<Action> Actions = new List<Action>();

            int count = 0;

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { count++; }));
            Actions.Add(actionCreator.Sequential(() => { count++; }));
            Actions.Add(actionCreator.Sequential(() => { count++; }));
            actionCreator.EndSequence();

            foreach (Action a in Actions)
            {
                a.Invoke();
            }

            Assert.AreEqual(6, count);
        }
    }
}