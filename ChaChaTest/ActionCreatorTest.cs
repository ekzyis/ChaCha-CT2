using Cryptool.Plugins.ChaCha.ViewModel.Components;
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
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +1
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +2
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +3
            actionCreator.EndSequence();

            foreach (Action a in Actions)
            {
                a.Invoke();
            }

            Assert.AreEqual(6, count);
        }

        [TestMethod]
        public void TestNestedSequential()
        {
            ActionCreator actionCreator = new ActionCreator();
            List<Action> Actions = new List<Action>();

            int count = 0;

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +1

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +2
            Actions.Add(actionCreator.Sequential(() => { count++; })); // +3
            actionCreator.EndSequence();

            Actions.Add(actionCreator.Sequential(() => { count++; })); // +2
            actionCreator.EndSequence();

            foreach (Action a in Actions)
            {
                a.Invoke();
            }

            Assert.AreEqual(8, count);
        }

        [TestMethod]
        public void TestSequentialExecutionOrder()
        {
            ActionCreator actionCreator = new ActionCreator();
            List<Action> Actions = new List<Action>();

            List<int> actual = new List<int>();

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { actual.Add(0); })); // Should add 0
            Actions.Add(actionCreator.Sequential(() => { actual.Add(1); })); // Should add 0, 1 (in this order)
            Actions.Add(actionCreator.Sequential(() => { actual.Add(2); })); // Should add 0, 1, 2 (in this order)
            actionCreator.EndSequence();

            foreach (Action a in Actions)
            {
                a.Invoke();
            }

            List<int> expected = new List<int>(new int[] { 0, 0, 1, 0, 1, 2 });

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNestedSequentialExecutionOrder()
        {
            ActionCreator actionCreator = new ActionCreator();
            List<Action> Actions = new List<Action>();

            List<int> actual = new List<int>();

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { actual.Add(0); })); // Should add 0
            Actions.Add(actionCreator.Sequential(() => { actual.Add(1); })); // Should add 0, 1 (in this order)

            actionCreator.StartSequence();
            Actions.Add(actionCreator.Sequential(() => { actual.Add(2); })); // Should add 0, 1, 2 (in this order)
            Actions.Add(actionCreator.Sequential(() => { actual.Add(3); })); // Should add 0, 1, 2, 3 (in this order)
            actionCreator.EndSequence();

            Actions.Add(actionCreator.Sequential(() => { actual.Add(4); })); // Should add 0, 1, 4 (in this order)
            Actions.Add(actionCreator.Sequential(() => { actual.Add(5); })); // Should add 0, 1, 4, 5 (in this order)

            foreach (Action a in Actions)
            {
                a.Invoke();
            }

            List<int> expected = new List<int>(new int[] { 0, 0, 1, 0, 1, 2, 0, 1, 2, 3, 0, 1, 4, 0, 1, 4, 5 });

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}