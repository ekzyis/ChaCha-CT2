using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cryptool.Plugins.ChaCha
{
    public partial class ChaChaPresentation : UserControl, INotifyPropertyChanged
    {
        private Page StateMatrixPage()
        {
            bool versionIsDJB = Version == ChaCha.Version.DJB;
            Page p = new Page(UIStateMatrixPage);
            #region constants
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                string desc = "The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. The first 16 bytes consist of the constants. ";
                Add(UIStateMatrixStepDescription, MakeBold(new Run(desc)));
            }, Undo);
            PageAction constantsInputAction = new PageAction(() =>
            {
                UnboldLast(UIStateMatrixStepDescription);
                Add(UITransformInput, MakeBold(new Run(HexConstants)));
            }, Undo);
            PageAction constantsChunksAction = new PageAction(() =>
            {
                UnboldLast(UITransformInput);
                Add(UITransformChunks, MakeBold(new Run(ConstantsChunks)));
            }, Undo);
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                UnboldLast(UITransformChunks);
                Add(UITransformLittleEndian, MakeBold(new Run(ConstantsLittleEndian)));
            }, Undo);
            PageAction addConstantsToStateAction = new PageAction(() =>
            {
                UnboldLast(UITransformLittleEndian);
                Add(UIState0, MakeBold(new Run(ConstantsLittleEndian.Replace(" ", "").Substring(0, 8))));
                Add(UIState1, MakeBold(new Run(ConstantsLittleEndian.Replace(" ", "").Substring(8, 8))));
                Add(UIState2, MakeBold(new Run(ConstantsLittleEndian.Replace(" ", "").Substring(16, 8))));
                Add(UIState3, MakeBold(new Run(ConstantsLittleEndian.Replace(" ", "").Substring(24, 8))));
            }, Undo);
            p.AddAction(constantsStepDescriptionAction);
            p.AddAction(constantsInputAction);
            p.AddAction(constantsChunksAction);
            p.AddAction(constantsLittleEndianAction);
            p.AddAction(addConstantsToStateAction);
            #endregion
            #region key
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                string desc = "The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ";
                Run r = MakeBold(new Run(desc));
                Add(UIStateMatrixStepDescription, r);
                Clear(UITransformInput);
                Clear(UITransformChunks);
                Clear(UITransformLittleEndian);
                UnboldLast(UIState0);
                UnboldLast(UIState1);
                UnboldLast(UIState2);
                UnboldLast(UIState3);
            }, Undo);
            PageAction keyInputAction = new PageAction(() =>
            {
                UnboldLast(UIStateMatrixStepDescription);
                Add(UITransformInput, MakeBold(new Run(HexInputKey)));
            }, Undo);
            PageAction keyChunksAction = new PageAction(() =>
            {
                UnboldLast(UITransformInput);
                Add(UITransformChunks, MakeBold(new Run(KeyChunks)));
            }, Undo);
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                UnboldLast(UITransformChunks);
                Add(UITransformLittleEndian, MakeBold(new Run(KeyLittleEndian)));
            }, Undo);
            PageAction addKeyToStateAction = new PageAction(() =>
            {
                UnboldLast(UITransformLittleEndian);
                Add(UIState4, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(0, 8))));
                Add(UIState5, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(8, 8))));
                Add(UIState6, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(16, 8))));
                Add(UIState7, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(24, 8))));
                Add(UIState8, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 0 : 32, 8))));
                Add(UIState9, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 8 : 40, 8))));
                Add(UIState10, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 16 : 48, 8))));
                Add(UIState11, MakeBold(new Run(KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 24 : 56, 8))));
            }, Undo);
            p.AddAction(keyStepDescriptionAction);
            p.AddAction(keyInputAction);
            p.AddAction(keyChunksAction);
            p.AddAction(keyLittleEndianAction);
            p.AddAction(addKeyToStateAction);
            #endregion
            #region iv
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                string desc = string.Format(
                    "The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", InputIV.Length
                ) + "First, we add the IV to the state. ";
                Run r = MakeBold(new Run(desc));
                Add(UIStateMatrixStepDescription, r);
                Clear(UITransformInput);
                Clear(UITransformChunks);
                Clear(UITransformLittleEndian);
                UnboldLast(UIState4);
                UnboldLast(UIState5);
                UnboldLast(UIState6);
                UnboldLast(UIState7);
                UnboldLast(UIState8);
                UnboldLast(UIState9);
                UnboldLast(UIState10);
                UnboldLast(UIState11);
            }, Undo);
            PageAction ivInputAction = new PageAction(() =>
            {
                UnboldLast(UIStateMatrixStepDescription);
                Add(UITransformInput, MakeBold(new Run(HexInputIV)));
            }, Undo);
            PageAction ivChunksAction = new PageAction(() =>
            {
                UnboldLast(UITransformInput);
                Add(UITransformChunks, MakeBold(new Run(IVChunks)));
            }, Undo);
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                UnboldLast(UITransformChunks);
                Add(UITransformLittleEndian, MakeBold(new Run(IVLittleEndian)));
            }, Undo);
            PageAction addIvToStateAction = new PageAction(() =>
            {
                UnboldLast(UITransformLittleEndian);
                if (!versionIsDJB)
                {
                    Add(UIState13, MakeBold(new Run(IVLittleEndian.Replace(" ", "").Substring(0, 8))));
                }
                Add(UIState14, MakeBold(new Run(IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 0 : 8, 8))));
                Add(UIState15, MakeBold(new Run(IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 8 : 16, 8))));
            }, Undo);
            p.AddAction(ivStepDescriptionAction);
            p.AddAction(ivInputAction);
            p.AddAction(ivChunksAction);
            p.AddAction(ivLittleEndianAction);
            p.AddAction(addIvToStateAction);
            #endregion
            #region counter
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                string desc = "And then the counter. Since this is our first keystream block, we set the counter to 0. ";
                Add(UIStateMatrixStepDescription, MakeBold(new Run(desc)));
                Clear(UITransformInput);
                Clear(UITransformChunks);
                Clear(UITransformLittleEndian);
                if (!versionIsDJB)
                {
                    UnboldLast(UIState13);
                }
                UnboldLast(UIState14);
                UnboldLast(UIState15);
            }, Undo);
            PageAction counterInputAction = new PageAction(() =>
            {
                UnboldLast(UIStateMatrixStepDescription);
                Add(UITransformInput, MakeBold(new Run(HexInitialCounter)));
            }, Undo);
            PageAction counterChunksAction = new PageAction(() =>
            {
                UnboldLast(UITransformInput);
                Add(UITransformChunks, MakeBold(new Run(InitialCounterChunks)));
            }, Undo);
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                UnboldLast(UITransformChunks);
                Add(UITransformLittleEndian, MakeBold(new Run(InitialCounterLittleEndian)));
            }, Undo);
            PageAction addCounterToStateAction = new PageAction(() =>
            {
                UnboldLast(UITransformLittleEndian);
                Add(UIState12, MakeBold(new Run(InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8))));
                if (versionIsDJB)
                {
                    Add(UIState13, MakeBold(new Run(InitialCounterLittleEndian.Replace(" ", "").Substring(8, 8))));
                }
            }, Undo);
            p.AddAction(counterStepDescriptionAction);
            p.AddAction(counterInputAction);
            p.AddAction(counterChunksAction);
            p.AddAction(counterLittleEndianAction);
            p.AddAction(addCounterToStateAction);
            PageAction nextPageDesc = new PageAction(() =>
            {
                string desc = "On the next page, we will use this initialized state matrix to generate the first keystream block.";
                Add(UIStateMatrixStepDescription, MakeBold(new Run(desc)));
                Clear(UITransformInput);
                Clear(UITransformChunks);
                Clear(UITransformLittleEndian);
                UnboldLast(UIState12);
                if (versionIsDJB)
                {
                    UnboldLast(UIState13);
                };
            }, Undo);
            p.AddAction(nextPageDesc);
            #endregion
            return p;          
        }
    }
}