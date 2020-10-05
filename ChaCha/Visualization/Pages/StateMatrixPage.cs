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
            string STATE_MATRIX_DESCRIPTION_1 = "The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. The first 16 bytes consist of the constants. ";
            string STATE_MATRIX_DESCRIPTION_2 = "The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ";
            string STATE_MATRIX_DESCRIPTION_3 = string.Format(
                    "The last 16 bytes consist of the counter and the IV (in this order). Since the IV may vary between 8 and 12 bytes, the counter may vary between 8 and 4 bytes. You have chosen a {0}-byte IV. ", pres.InputIV.Length
                ) + "First, we add the IV to the state. ";
            string STATE_MATRIX_DESCRIPTION_4 = "And then the counter. Since this is our first keystream block, we set the counter to 0. ";
            string STATE_MATRIX_DESCRIPTION_5 = "On the next page, we will use this initialized state matrix to generate the first keystream block.";
            void AddBoldToDescription(string descToAdd)
            {
                pres.Nav.AddBold(pres.UIStateMatrixStepDescription, descToAdd);
            }
            void ClearDescription()
            {
                pres.Nav.Clear(pres.UIStateMatrixStepDescription);
            }
            void UnboldLastFromDescription()
            {
                pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
            }
            void MakeLastBoldInDescription()
            {
                pres.Nav.MakeBoldLast(pres.UIStateMatrixStepDescription);
            }
            void RemoveLastFromDescription()
            {
                pres.Nav.RemoveLast(pres.UIStateMatrixStepDescription);
            }
            void ReplaceTransformInput(string input)
            {
                if (input.Length > 32)
                {
                    pres.Nav.Replace(pres.UITransformInput, input.Substring(0, 32));
                    pres.Nav.Replace(pres.UITransformInput2, input.Substring(32));
                }
                else
                {
                    pres.Nav.Replace(pres.UITransformInput, input);
                }
            }
            void ClearTransformInput()
            {
                pres.Nav.Clear(pres.UITransformInput, pres.UITransformInput2);
            }
            void ReplaceTransformChunk(params string[] chunk)
            {
                if (chunk.Length == 2)
                {
                    // use borders in center to center text
                    pres.Nav.Replace(pres.UITransformChunk1, chunk[0]);
                    pres.Nav.Replace(pres.UITransformChunk2, chunk[1]);
                }
                else
                {
                    for (int i = 0; i < chunk.Length; ++i)
                    {
                        pres.Nav.Replace((TextBox)pres.FindName($"UITransformChunk{i}"), chunk[i]);
                    }
                }
            }
            void ClearTransformChunk()
            {
                pres.Nav.Clear(pres.UITransformChunk0);
                pres.Nav.Clear(pres.UITransformChunk1);
                pres.Nav.Clear(pres.UITransformChunk2);
                pres.Nav.Clear(pres.UITransformChunk3);
                pres.Nav.Clear(pres.UITransformChunk4);
                pres.Nav.Clear(pres.UITransformChunk5);
                pres.Nav.Clear(pres.UITransformChunk6);
                pres.Nav.Clear(pres.UITransformChunk7);
            }
            void ReplaceTransformLittleEndian(params string[] le)
            {
                // TODO create another grid with 3 rows to center IV in IETF version and add branch for le.Length == 3 (IV IETF) and le.Length == 1 (counter IETF) here
                if (le.Length == 2)
                {
                    // use borders in center to center text
                    pres.Nav.Replace(pres.UITransformLittleEndian1, le[0]);
                    pres.Nav.Replace(pres.UITransformLittleEndian2, le[1]);
                }
                else
                {
                    for (int i = 0; i < le.Length; ++i)
                    {
                        pres.Nav.Replace((TextBox)pres.FindName($"UITransformLittleEndian{i}"), le[i]);
                    }
                }
            }
            void ClearTransformLittleEndian()
            {
                pres.Nav.Clear(pres.UITransformLittleEndian0);
                pres.Nav.Clear(pres.UITransformLittleEndian1);
                pres.Nav.Clear(pres.UITransformLittleEndian2);
                pres.Nav.Clear(pres.UITransformLittleEndian3);
                pres.Nav.Clear(pres.UITransformLittleEndian4);
                pres.Nav.Clear(pres.UITransformLittleEndian5);
                pres.Nav.Clear(pres.UITransformLittleEndian6);
                pres.Nav.Clear(pres.UITransformLittleEndian7);
            }
            #region constants
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(STATE_MATRIX_DESCRIPTION_1);
            }, () =>
            {
                ClearDescription();
            });
            PageAction constantsInputAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                ReplaceTransformInput(pres.HexConstants);
            }, () =>
            {
                MakeLastBoldInDescription();
                ClearTransformInput();
            });
            PageAction constantsChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction constantsLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyConstantsToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell },
                new Border[] { pres.UIState0Cell, pres.UIState1Cell, pres.UIState2Cell, pres.UIState3Cell },
                new string[] { "", "", "", "" });
            page.AddAction(constantsStepDescriptionAction);
            page.AddAction(constantsInputAction);
            page.AddAction(constantsChunksAction);
            page.AddAction(constantsLittleEndianAction);
            page.AddAction(copyConstantsToStateActions);
            #endregion
            #region key
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(STATE_MATRIX_DESCRIPTION_2);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                ReplaceTransformInput(pres.HexConstants);
                ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
                ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
            });
            PageAction keyInputAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                ReplaceTransformInput(pres.HexInputKey);
            }, () =>
            {
                MakeLastBoldInDescription();
                ClearTransformInput();
            });
            PageAction keyChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunk(
                    pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction keyLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndian(
                    pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                    pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyKeyToStateActions = pres.Nav.CopyActions(
                new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell, pres.UITransformLittleEndian3Cell, pres.UITransformLittleEndian4Cell, pres.UITransformLittleEndian5Cell, pres.UITransformLittleEndian6Cell, pres.UITransformLittleEndian7Cell },
                new Border[] { pres.UIState4Cell, pres.UIState5Cell, pres.UIState6Cell, pres.UIState7Cell, pres.UIState8Cell, pres.UIState9Cell, pres.UIState10Cell, pres.UIState11Cell },
                new string[] { "", "", "", "", "", "", "", "" });
            page.AddAction(keyStepDescriptionAction);
            page.AddAction(keyInputAction);
            page.AddAction(keyChunksAction);
            page.AddAction(keyLittleEndianAction);
            page.AddAction(copyKeyToStateActions);
            #endregion
            #region iv
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(STATE_MATRIX_DESCRIPTION_3);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                ReplaceTransformInput(pres.HexInputKey);
                ReplaceTransformChunk(pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
                ReplaceTransformLittleEndian(pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                    pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            });
            PageAction ivInputAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                ReplaceTransformInput(pres.HexInputIV);
            }, () =>
            {
                MakeLastBoldInDescription();
                ClearTransformInput();
            });
            void ReplaceTransformChunkIV()
            {
                if (versionIsDJB)
                {
                    ReplaceTransformChunk(pres.IVChunks[0], pres.IVChunks[1]);
                }
                else
                {
                    ReplaceTransformChunk(pres.IVChunks[0], pres.IVChunks[1], pres.IVChunks[2]);
                }
            }
            PageAction ivChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkIV();
            }, () =>
            {
                ClearTransformChunk();
            });
            void ReplaceTransformLittleEndianIV()
            {
                if (versionIsDJB)
                {
                    ReplaceTransformLittleEndian(pres.IVLittleEndian[0], pres.IVLittleEndian[1]);
                }
                else
                {
                    ReplaceTransformLittleEndian(pres.IVLittleEndian[0], pres.IVLittleEndian[1], pres.IVLittleEndian[2]);
                }
            }
            PageAction ivLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianIV();
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyIVToStateActions = versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState14Cell, pres.UIState15Cell },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center IV in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian0Cell, pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState13Cell, pres.UIState14Cell, pres.UIState15Cell },
                    new string[] { "", "", "" });
            page.AddAction(ivStepDescriptionAction);
            page.AddAction(ivInputAction);
            page.AddAction(ivChunksAction);
            page.AddAction(ivLittleEndianAction);
            page.AddAction(copyIVToStateActions);
            #endregion
            #region counter
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(STATE_MATRIX_DESCRIPTION_4);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                ReplaceTransformInput(pres.HexInputIV);
                ReplaceTransformChunkIV();
                ReplaceTransformLittleEndianIV();
            });
            PageAction counterInputAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                ReplaceTransformInput(pres.HexInitialCounter);
            }, () =>
            {
                MakeLastBoldInDescription();
                ClearTransformInput();
            });
            PageAction counterChunksAction = new PageAction(() =>
            {
                if (versionIsDJB)
                {
                    ReplaceTransformChunk(pres.InitialCounterChunks[0], pres.InitialCounterChunks[1]);
                }
                else
                {
                    ReplaceTransformChunk(pres.InitialCounterChunks[0]);
                }
            }, () =>
            {
                ClearTransformChunk();
            });
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                if (versionIsDJB)
                {
                    ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0], pres.InitialCounterLittleEndian[1]);
                }
                else
                {
                    ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0]);
                }
            }, () =>
            {
                ClearTransformLittleEndian();
            });
            PageAction[] copyCounterToStateActions = versionIsDJB ?
                pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian1Cell, pres.UITransformLittleEndian2Cell },
                    new Border[] { pres.UIState12Cell, pres.UIState13Cell },
                    new string[] { "", "" })
                // TODO create another grid with 3 rows to center counter in IETF version and update this code here
                : pres.Nav.CopyActions(
                    new Border[] { pres.UITransformLittleEndian0Cell },
                    new Border[] { pres.UIState12Cell },
                    new string[] { "" });
            page.AddAction(counterStepDescriptionAction);
            page.AddAction(counterInputAction);
            page.AddAction(counterChunksAction);
            page.AddAction(counterLittleEndianAction);
            page.AddAction(copyCounterToStateActions);
            PageAction nextPageDesc = new PageAction(() =>
            {
                AddBoldToDescription(STATE_MATRIX_DESCRIPTION_5);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                ReplaceTransformInput(pres.HexInitialCounter);
                if (versionIsDJB)
                {
                    ReplaceTransformChunk(pres.InitialCounterChunks[0], pres.InitialCounterChunks[1]);
                }
                else
                {
                    ReplaceTransformChunk(pres.InitialCounterChunks[0]);
                }
                if (versionIsDJB)
                {
                    ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0], pres.InitialCounterLittleEndian[1]);
                }
                else
                {
                    ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0]);
                }
            });
            page.AddAction(nextPageDesc);
            #endregion
            return page;
        }
    }
}