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
            bool keyIs32Byte = InputKey.Length == 32;
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
                nav.Replace(UITransformInput, HexConstants);
            }, nav.Undo);
            PageAction constantsChunksAction = new PageAction(() =>
            {
                nav.Replace(UITransformChunk0, ConstantsChunks[0]);
                nav.Replace(UITransformChunk1, ConstantsChunks[1]);
                nav.Replace(UITransformChunk2, ConstantsChunks[2]);
                nav.Replace(UITransformChunk3, ConstantsChunks[3]);
            }, nav.Undo);
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                nav.Replace(UITransformLittleEndian0, ConstantsLittleEndian[0]);
                nav.Replace(UITransformLittleEndian1, ConstantsLittleEndian[1]);
                nav.Replace(UITransformLittleEndian2, ConstantsLittleEndian[2]);
                nav.Replace(UITransformLittleEndian3, ConstantsLittleEndian[3]);
            }, nav.Undo);
            PageAction[] copyConstantsToStateActions = CopyActions(
                new Border[] { UITransformLittleEndian0Cell, UITransformLittleEndian1Cell, UITransformLittleEndian2Cell, UITransformLittleEndian3Cell },
                new Border[] { UIState0Cell, UIState1Cell, UIState2Cell, UIState3Cell });
            p.AddAction(constantsStepDescriptionAction);
            p.AddAction(constantsInputAction);
            p.AddAction(constantsChunksAction);
            p.AddAction(constantsLittleEndianAction);
            p.AddAction(copyConstantsToStateActions);
            #endregion
            #region key
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                string desc = "The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ";
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunk0, UITransformChunk1, UITransformChunk2, UITransformChunk3);
                nav.Clear(UITransformLittleEndian0, UITransformLittleEndian1, UITransformLittleEndian2, UITransformLittleEndian3);
            }, nav.Undo);
            PageAction keyInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.Replace(UITransformInput, HexInputKey);
            }, nav.Undo);
            PageAction keyChunksAction = new PageAction(() =>
            {
                nav.Replace(UITransformChunk0, KeyChunks[0]);
                nav.Replace(UITransformChunk1, KeyChunks[1]);
                nav.Replace(UITransformChunk2, KeyChunks[2]);
                nav.Replace(UITransformChunk3, KeyChunks[3]);
                nav.Replace(UITransformChunk4, KeyChunks[keyIs32Byte ? 4 : 0]);
                nav.Replace(UITransformChunk5, KeyChunks[keyIs32Byte ? 5 : 1]);
                nav.Replace(UITransformChunk6, KeyChunks[keyIs32Byte ? 6 : 2]);
                nav.Replace(UITransformChunk7, KeyChunks[keyIs32Byte ? 7 : 3]);
            }, nav.Undo);
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                nav.Replace(UITransformLittleEndian0, KeyLittleEndian[0]);
                nav.Replace(UITransformLittleEndian1, KeyLittleEndian[1]);
                nav.Replace(UITransformLittleEndian2, KeyLittleEndian[2]);
                nav.Replace(UITransformLittleEndian3, KeyLittleEndian[3]);
                nav.Replace(UITransformLittleEndian4, KeyLittleEndian[keyIs32Byte ? 4 : 0]);
                nav.Replace(UITransformLittleEndian5, KeyLittleEndian[keyIs32Byte ? 5 : 1]);
                nav.Replace(UITransformLittleEndian6, KeyLittleEndian[keyIs32Byte ? 6 : 2]);
                nav.Replace(UITransformLittleEndian7, KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            }, nav.Undo);
            PageAction[] copyKeyToStateActions = CopyActions(
                new Border[] { UITransformLittleEndian0Cell, UITransformLittleEndian1Cell, UITransformLittleEndian2Cell, UITransformLittleEndian3Cell, UITransformLittleEndian4Cell, UITransformLittleEndian5Cell, UITransformLittleEndian6Cell, UITransformLittleEndian7Cell },
                new Border[] { UIState4Cell, UIState5Cell, UIState6Cell, UIState7Cell, UIState8Cell, UIState9Cell, UIState10Cell, UIState11Cell });
            p.AddAction(keyStepDescriptionAction);
            p.AddAction(keyInputAction);
            p.AddAction(keyChunksAction);
            p.AddAction(keyLittleEndianAction);
            p.AddAction(copyKeyToStateActions);
            #endregion
            #region iv
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                string desc = string.Format(
                    "The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", InputIV.Length
                ) + "First, we add the IV to the state. ";
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunk0, UITransformChunk1, UITransformChunk2, UITransformChunk3, UITransformChunk4, UITransformChunk5, UITransformChunk6, UITransformChunk7);
                nav.Clear(UITransformLittleEndian0, UITransformLittleEndian1, UITransformLittleEndian2, UITransformLittleEndian3, UITransformLittleEndian4, UITransformLittleEndian5, UITransformLittleEndian6, UITransformLittleEndian7);
            }, nav.Undo);
            PageAction ivInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.Replace(UITransformInput, HexInputIV);
            }, nav.Undo);
            PageAction ivChunksAction = new PageAction(() =>
            {
                nav.Replace(UITransformChunk0, IVChunks[0]);
                nav.Replace(UITransformChunk1, IVChunks[1]);
                if(!versionIsDJB) nav.Replace(UITransformChunk2, IVChunks[2]);
            }, nav.Undo);
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                nav.Replace(UITransformLittleEndian0, IVLittleEndian[0]);
                nav.Replace(UITransformLittleEndian1, IVLittleEndian[1]);
                if(!versionIsDJB) nav.Replace(UITransformLittleEndian2, IVLittleEndian[2]);
            }, nav.Undo);
            PageAction[] copyIVToStateActions = versionIsDJB ? CopyActions(new Border[] { UITransformLittleEndian0Cell, UITransformLittleEndian1Cell }, new Border[] { UIState14Cell, UIState15Cell })
                : CopyActions(new Border[] { UITransformLittleEndian0Cell, UITransformLittleEndian1Cell, UITransformLittleEndian2Cell }, new Border[] { UIState13Cell, UIState14Cell, UIState15Cell });
            p.AddAction(ivStepDescriptionAction);
            p.AddAction(ivInputAction);
            p.AddAction(ivChunksAction);
            p.AddAction(ivLittleEndianAction);
            p.AddAction(copyIVToStateActions);
            #endregion
            #region counter
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                string desc = "And then the counter. Since this is our first keystream block, we set the counter to 0. ";
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunk0, UITransformChunk1, UITransformChunk2, UITransformChunk3);
                nav.Clear(UITransformLittleEndian0, UITransformLittleEndian1, UITransformLittleEndian2, UITransformLittleEndian3);
            }, nav.Undo);
            PageAction counterInputAction = new PageAction(() =>
            {
                nav.UnboldLast(UIStateMatrixStepDescription);
                nav.Replace(UITransformInput, HexInitialCounter);
            }, nav.Undo);
            PageAction counterChunksAction = new PageAction(() =>
            {
                nav.Replace(UITransformChunk0, InitialCounterChunks[0]);
                if(versionIsDJB)
                {
                    nav.Replace(UITransformChunk1, InitialCounterChunks[1]);
                }
            }, nav.Undo);
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                nav.Replace(UITransformLittleEndian0, InitialCounterLittleEndian[0]);
                if (versionIsDJB)
                {
                    nav.Replace(UITransformLittleEndian1, InitialCounterLittleEndian[1]);
                }
            }, nav.Undo);
            PageAction[] copyCounterToStateActions = versionIsDJB ? CopyActions(new Border[] { UITransformLittleEndian0Cell, UITransformLittleEndian1Cell }, new Border[] { UIState12Cell, UIState13Cell }) :
                CopyActions(new Border[] { UITransformLittleEndian0Cell }, new Border[] { UIState12Cell });
            p.AddAction(counterStepDescriptionAction);
            p.AddAction(counterInputAction);
            p.AddAction(counterChunksAction);
            p.AddAction(counterLittleEndianAction);
            p.AddAction(copyCounterToStateActions);
            PageAction nextPageDesc = new PageAction(() =>
            {
                string desc = "On the next page, we will use this initialized state matrix to generate the first keystream block.";
                nav.AddBold(UIStateMatrixStepDescription, desc);
                nav.Clear(UITransformInput);
                nav.Clear(UITransformChunk0, UITransformChunk1, UITransformChunk2, UITransformChunk3);
                nav.Clear(UITransformLittleEndian0, UITransformLittleEndian1, UITransformLittleEndian2, UITransformLittleEndian3);
            }, nav.Undo);
            p.AddAction(nextPageDesc);
            #endregion
            return p;          
        }
    }
}