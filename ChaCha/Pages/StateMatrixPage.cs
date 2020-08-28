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
                nav.AddBold(UIStateMatrixStepDescription, desc);
            }, nav.Undo);
            PageAction constantsInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.AddBold(UITransformInput, HexConstants);
            }, nav.Undo);
            PageAction constantsChunksAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformInput);
                nav.AddBold(UITransformChunks, ConstantsChunks);
            }, nav.Undo);
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformChunks);
                nav.AddBold(UITransformLittleEndian, ConstantsLittleEndian);
            }, nav.Undo);
            PageAction addConstantsToStateAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformLittleEndian);
                nav.Replace(UIState0, ConstantsLittleEndian.Replace(" ", "").Substring(0, 8));
                nav.Replace(UIState1, ConstantsLittleEndian.Replace(" ", "").Substring(8, 8));
                nav.Replace(UIState2, ConstantsLittleEndian.Replace(" ", "").Substring(16, 8));
                nav.Replace(UIState3, ConstantsLittleEndian.Replace(" ", "").Substring(24, 8));
            }, nav.Undo);
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
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunks);
                nav.Clear(UITransformLittleEndian);
            }, nav.Undo);
            PageAction keyInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.AddBold(UITransformInput, HexInputKey);
            }, nav.Undo);
            PageAction keyChunksAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformInput);
                nav.AddBold(UITransformChunks, KeyChunks);
            }, nav.Undo);
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformChunks);
                nav.AddBold(UITransformLittleEndian, KeyLittleEndian);
            }, nav.Undo);
            PageAction addKeyToStateAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformLittleEndian);
                nav.Replace(UIState4, KeyLittleEndian.Replace(" ", "").Substring(0, 8));
                nav.Replace(UIState5, KeyLittleEndian.Replace(" ", "").Substring(8, 8));
                nav.Replace(UIState6, KeyLittleEndian.Replace(" ", "").Substring(16, 8));
                nav.Replace(UIState7, KeyLittleEndian.Replace(" ", "").Substring(24, 8));
                nav.Replace(UIState8, KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 0 : 32, 8));
                nav.Replace(UIState9, KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 8 : 40, 8));
                nav.Replace(UIState10, KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 16 : 48, 8));
                nav.Replace(UIState11, KeyLittleEndian.Replace(" ", "").Substring(InputKey.Length == 16 ? 24 : 56, 8));
            }, nav.Undo);
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
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunks);
                nav.Clear(UITransformLittleEndian);
            }, nav.Undo);
            PageAction ivInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.AddBold(UITransformInput, HexInputIV);
            }, nav.Undo);
            PageAction ivChunksAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformInput);
                nav.AddBold(UITransformChunks, IVChunks);
            }, nav.Undo);
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformChunks);
                nav.AddBold(UITransformLittleEndian, IVLittleEndian);
            }, nav.Undo);
            PageAction addIvToStateAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformLittleEndian);
                if (!versionIsDJB)
                {
                    nav.Replace(UIState13, IVLittleEndian.Replace(" ", "").Substring(0, 8));
                }
                nav.Replace(UIState14, IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 0 : 8, 8));
                nav.Replace(UIState15, IVLittleEndian.Replace(" ", "").Substring(versionIsDJB ? 8 : 16, 8));
            }, nav.Undo);
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
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunks);
                nav.Clear(UITransformLittleEndian);
            }, nav.Undo);
            PageAction counterInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.AddBold(UITransformInput, HexInitialCounter);
            }, nav.Undo);
            PageAction counterChunksAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformInput);
                nav.AddBold(UITransformChunks, InitialCounterChunks);
            }, nav.Undo);
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformChunks);
                nav.AddBold(UITransformLittleEndian, InitialCounterLittleEndian);
            }, nav.Undo);
            PageAction addCounterToStateAction = new PageAction(() =>
            {
                nav.UnboldLast(UITransformLittleEndian);
                nav.Replace(UIState12, InitialCounterLittleEndian.Replace(" ", "").Substring(0, 8));
                if (versionIsDJB)
                {
                    nav.Replace(UIState13, InitialCounterLittleEndian.Replace(" ", "").Substring(8, 8));
                }
            }, nav.Undo);
            p.AddAction(counterStepDescriptionAction);
            p.AddAction(counterInputAction);
            p.AddAction(counterChunksAction);
            p.AddAction(counterLittleEndianAction);
            p.AddAction(addCounterToStateAction);
            PageAction nextPageDesc = new PageAction(() =>
            {
                string desc = "On the next page, we will use this initialized state matrix to generate the first keystream block.";
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunks);
                nav.Clear(UITransformLittleEndian);
            }, nav.Undo);
            p.AddAction(nextPageDesc);
            #endregion
            return p;          
        }
    }
}