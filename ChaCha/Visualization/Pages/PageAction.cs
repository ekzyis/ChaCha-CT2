using System;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    public class PageAction
    {
        private List<Action> _exec = new List<Action>();
        private List<Action> _undo = new List<Action>();
        private List<string> _labels = new List<string>();
        public PageAction(Action exec, Action undo, string label = "")
        {
            _exec.Add(exec);
            _undo.Add(undo);
            _labels.Add(label);
        }
        public void exec()
        {
            foreach (Action a in _exec)
            {
                a();
            }
        }
        public void undo()
        {
            foreach (Action a in _undo)
            {
                a();
            }
        }
        public string[] Labels
        {
            get
            {
                return _labels.ToArray();
            }
        }
        public void AddLabel(string label)
        {
            _labels.Add(label);
        }
        public void AddToExec(Action toAdd)
        {
            _exec.Add(toAdd);
        }
        public void AddToUndo(Action toAdd)
        {
            _undo.Add(toAdd);
        }

        public void Add(PageAction toAdd)
        {
            _exec.Add(toAdd.exec);
            _undo.Add(toAdd.undo);
        }
    }
}
