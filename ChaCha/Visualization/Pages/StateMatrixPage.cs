using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    partial class Page
    {
        public static Page StateMatrixPage(ChaChaPresentation pres)
        {
            bool versionIsDJB = pres.Version == ChaCha.Version.DJB;
            bool keyIs32Byte = pres.InputKey.Length == 32;
            Page page = new Page(pres.UIStateMatrixPage);
            #region constants
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                string desc = "The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. The first 16 bytes consist of the constants. ";
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, desc);
            }, pres.Nav.Undo);
            PageAction constantsInputAction = new PageAction(() =>
            {
                pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
                pres.Nav.Replace(pres.UITransformInput, pres.HexConstants);
            }, pres.Nav.Undo);
            PageAction constantsChunksAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformChunk0, pres.ConstantsChunks[0]);
                pres.Nav.Replace(pres.UITransformChunk1, pres.ConstantsChunks[1]);
                pres.Nav.Replace(pres.UITransformChunk2, pres.ConstantsChunks[2]);
                pres.Nav.Replace(pres.UITransformChunk3, pres.ConstantsChunks[3]);
            }, pres.Nav.Undo);
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformLittleEndian0, pres.ConstantsLittleEndian[0]);
                pres.Nav.Replace(pres.UITransformLittleEndian1, pres.ConstantsLittleEndian[1]);
                pres.Nav.Replace(pres.UITransformLittleEndian2, pres.ConstantsLittleEndian[2]);
                pres.Nav.Replace(pres.UITransformLittleEndian3, pres.ConstantsLittleEndian[3]);
            }, pres.Nav.Undo);
            PageAction[] copyConstantsToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell },
                new Border[] { pres.UIState0Cell, pres.UIState1Cell, pres.UIState2Cell, pres.UIState3Cell });
            page.AddAction(constantsStepDescriptionAction);
            page.AddAction(constantsInputAction);
            page.AddAction(constantsChunksAction);
            page.AddAction(constantsLittleEndianAction);
            page.AddAction(copyConstantsToStateActions);
            #endregion
            #region key
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                string desc = "The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ";
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, desc);
                pres.Nav.Clear(pres.UITransformInput);
                pres.Nav.Clear(pres.UITransformChunk0, pres.UITransformChunk1, pres.UITransformChunk2, pres.UITransformChunk3);
                pres.Nav.Clear(pres.UITransformLittleEndian0, pres.UITransformLittleEndian1, pres.UITransformLittleEndian2, pres.UITransformLittleEndian3);
            }, pres.Nav.Undo);
            PageAction keyInputAction = new PageAction(() =>
            {
                pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
                pres.Nav.Replace(pres.UITransformInput, pres.HexInputKey);
            }, pres.Nav.Undo);
            PageAction keyChunksAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformChunk0, pres.KeyChunks[0]);
                pres.Nav.Replace(pres.UITransformChunk1, pres.KeyChunks[1]);
                pres.Nav.Replace(pres.UITransformChunk2, pres.KeyChunks[2]);
                pres.Nav.Replace(pres.UITransformChunk3, pres.KeyChunks[3]);
                pres.Nav.Replace(pres.UITransformChunk4, pres.KeyChunks[keyIs32Byte ? 4 : 0]);
                pres.Nav.Replace(pres.UITransformChunk5, pres.KeyChunks[keyIs32Byte ? 5 : 1]);
                pres.Nav.Replace(pres.UITransformChunk6, pres.KeyChunks[keyIs32Byte ? 6 : 2]);
                pres.Nav.Replace(pres.UITransformChunk7, pres.KeyChunks[keyIs32Byte ? 7 : 3]);
            }, pres.Nav.Undo);
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformLittleEndian0, pres.KeyLittleEndian[0]);
                pres.Nav.Replace(pres.UITransformLittleEndian1, pres.KeyLittleEndian[1]);
                pres.Nav.Replace(pres.UITransformLittleEndian2, pres.KeyLittleEndian[2]);
                pres.Nav.Replace(pres.UITransformLittleEndian3, pres.KeyLittleEndian[3]);
                pres.Nav.Replace(pres.UITransformLittleEndian4, pres.KeyLittleEndian[keyIs32Byte ? 4 : 0]);
                pres.Nav.Replace(pres.UITransformLittleEndian5, pres.KeyLittleEndian[keyIs32Byte ? 5 : 1]);
                pres.Nav.Replace(pres.UITransformLittleEndian6, pres.KeyLittleEndian[keyIs32Byte ? 6 : 2]);
                pres.Nav.Replace(pres.UITransformLittleEndian7, pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            }, pres.Nav.Undo);
            PageAction[] copyKeyToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell, pres.UITransformLittleEndian4Cell, pres.UITransformLittleEndian5Cell, pres.UITransformLittleEndian6Cell, pres.UITransformLittleEndian7Cell },
                new Border[] { pres.UIState4Cell, pres.UIState5Cell, pres.UIState6Cell, pres.UIState7Cell, pres.UIState8Cell, pres.UIState9Cell, pres.UIState10Cell, pres.UIState11Cell });
            page.AddAction(keyStepDescriptionAction);
            page.AddAction(keyInputAction);
            page.AddAction(keyChunksAction);
            page.AddAction(keyLittleEndianAction);
            page.AddAction(copyKeyToStateActions);
            #endregion
            #region iv
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                string desc = string.Format(
                    "The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", pres.InputIV.Length
                ) + "First, we add the IV to the state. ";
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, desc);
                pres.Nav.Clear(pres.UITransformInput);
                pres.Nav.Clear(pres.UITransformChunk0, pres.UITransformChunk1, pres.UITransformChunk2, pres.UITransformChunk3, pres.UITransformChunk4, pres.UITransformChunk5, pres.UITransformChunk6, pres.UITransformChunk7);
                pres.Nav.Clear(pres.UITransformLittleEndian0, pres.UITransformLittleEndian1, pres.UITransformLittleEndian2, pres.UITransformLittleEndian3, pres.UITransformLittleEndian4, pres.UITransformLittleEndian5, pres.UITransformLittleEndian6, pres.UITransformLittleEndian7);
            }, pres.Nav.Undo);
            PageAction ivInputAction = new PageAction(() =>
            {
                pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
                pres.Nav.Replace(pres.UITransformInput, pres.HexInputIV);
            }, pres.Nav.Undo);
            PageAction ivChunksAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformChunk0, pres.IVChunks[0]);
                pres.Nav.Replace(pres.UITransformChunk1, pres.IVChunks[1]);
                if(!versionIsDJB) pres.Nav.Replace(pres.UITransformChunk2, pres.IVChunks[2]);
            }, pres.Nav.Undo);
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformLittleEndian0, pres.IVLittleEndian[0]);
                pres.Nav.Replace(pres.UITransformLittleEndian1, pres.IVLittleEndian[1]);
                if(!versionIsDJB) pres.Nav.Replace(pres.UITransformLittleEndian2, pres.IVLittleEndian[2]);
            }, pres.Nav.Undo);
            PageAction[] copyIVToStateActions = versionIsDJB ? pres.Nav.CopyActions(new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell }, new Border[] { pres.UIState14Cell, pres.UIState15Cell })
                : pres.Nav.CopyActions(new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell }, new Border[] { pres.UIState13Cell, pres.UIState14Cell, pres.UIState15Cell });
            page.AddAction(ivStepDescriptionAction);
            page.AddAction(ivInputAction);
            page.AddAction(ivChunksAction);
            page.AddAction(ivLittleEndianAction);
            page.AddAction(copyIVToStateActions);
            #endregion
            #region counter
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                string desc = "And then the counter. Since this is our first keystream block, we set the counter to 0. ";
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, desc);
                pres.Nav.Clear(pres.UITransformInput);
                pres.Nav.Clear(pres.UITransformChunk0, pres.UITransformChunk1, pres.UITransformChunk2, pres.UITransformChunk3);
                pres.Nav.Clear(pres.UITransformLittleEndian0, pres.UITransformLittleEndian1, pres.UITransformLittleEndian2, pres.UITransformLittleEndian3);
            }, pres.Nav.Undo);
            PageAction counterInputAction = new PageAction(() =>
            {
                pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
                pres.Nav.Replace(pres.UITransformInput, pres.HexInitialCounter);
            }, pres.Nav.Undo);
            PageAction counterChunksAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformChunk0, pres.InitialCounterChunks[0]);
                if(versionIsDJB)
                {
                    pres.Nav.Replace(pres.UITransformChunk1, pres.InitialCounterChunks[1]);
                }
            }, pres.Nav.Undo);
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                pres.Nav.Replace(pres.UITransformLittleEndian0, pres.InitialCounterLittleEndian[0]);
                if (versionIsDJB)
                {
                    pres.Nav.Replace(pres.UITransformLittleEndian1, pres.InitialCounterLittleEndian[1]);
                }
            }, pres.Nav.Undo);
            PageAction[] copyCounterToStateActions = versionIsDJB ? pres.Nav.CopyActions(new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell }, new Border[] { pres.UIState12Cell, pres.UIState13Cell }) :
                pres.Nav.CopyActions(new Border[] { pres.UITransformLittleEndian0Cell }, new Border[] { pres.UIState12Cell });
            page.AddAction(counterStepDescriptionAction);
            page.AddAction(counterInputAction);
            page.AddAction(counterChunksAction);
            page.AddAction(counterLittleEndianAction);
            page.AddAction(copyCounterToStateActions);
            PageAction nextPageDesc = new PageAction(() =>
            {
                string desc = "On the next page, we will use this initialized state matrix to generate the first keystream block.";
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, desc);
                pres.Nav.Clear(pres.UITransformInput);
                pres.Nav.Clear(pres.UITransformChunk0, pres.UITransformChunk1, pres.UITransformChunk2, pres.UITransformChunk3);
                pres.Nav.Clear(pres.UITransformLittleEndian0, pres.UITransformLittleEndian1, pres.UITransformLittleEndian2, pres.UITransformLittleEndian3);
            }, pres.Nav.Undo);
            page.AddAction(nextPageDesc);
            #endregion
            return page;          
        }
    }
}