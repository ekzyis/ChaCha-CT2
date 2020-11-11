﻿using Cryptool.Plugins.ChaCha.Helper;
using Cryptool.Plugins.ChaCha.Util;
using System;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaCha.ViewModel.Components
{
    internal static class Diffusion
    {
        /// <summary>
        /// Set the Document of the RichTextBox with the diffusion value as hex string; using byte array for comparison; marking differences red.
        /// </summary>
        public static void InitDiffusionValue(RichTextBox rtb, byte[] diffusion, byte[] primary)
        {
            string dHex = Formatter.HexString(diffusion);
            string pHex = Formatter.HexString(primary);
            InitDiffusionValue(rtb, dHex, pHex);
        }

        /// <summary>
        /// Set the Document of the RichTextBox with the diffusion value as hex string; using byte array for comparison; marking differences red.
        /// </summary>
        public static void InitDiffusionValue(RichTextBox rtb, uint diffusion, uint primary)
        {
            string dHex = Formatter.HexString(diffusion);
            string pHex = Formatter.HexString(primary);
            InitDiffusionValue(rtb, dHex, pHex);
        }

        /// <summary>
        /// Set the document of the RichTextBox with the diffusion value as hex string; using strings for comparison; marking differences red.
        /// </summary>
        public static void InitDiffusionValue(RichTextBox rtb, string dHex, string pHex)
        {
            if (dHex.Length != pHex.Length) throw new ArgumentException("Diffusion value must be of same length as primary value.");
            if (dHex.Length % 2 != 0) throw new ArgumentException("Length must be even");
            if ((Paragraph)rtb.Document.Blocks.LastBlock == null)
            {
                rtb.Document.Blocks.Add(new Paragraph());
            }
            else ((Paragraph)rtb.Document.Blocks.LastBlock).Inlines.Clear();
            for (int i = 0; i < dHex.Length; i += 2)
            {
                char dChar1 = dHex[i];
                char dChar2 = dHex[i + 1];
                char pChar1 = pHex[i];
                char pChar2 = pHex[i + 1];
                ((Paragraph)rtb.Document.Blocks.LastBlock).Inlines.Add(RedIfDifferent(dChar1, pChar1));
                ((Paragraph)rtb.Document.Blocks.LastBlock).Inlines.Add(RedIfDifferent(dChar2, pChar2));
            }
        }

        /// <summary>
        /// Returns a Run element with the character d in red if d != v else black.
        /// </summary>
        private static Run RedIfDifferent(char d, char v)
        {
            return new Run() { Text = d.ToString(), Foreground = d != v ? Brushes.Red : Brushes.Black };
        }

        /// <summary>
        /// Set the Document of the RichTextBox with the diffusion value; marking differences red.
        /// Version is used to determine counter size.
        /// </summary>
        public static void InitDiffusionValue(RichTextBox rtb, BigInteger diffusion, BigInteger primary, Version version)
        {
            if (version.CounterBits == 64)
            {
                byte[] diffusionBytes = ByteUtil.GetBytesBE((ulong)diffusion);
                byte[] primaryBytes = ByteUtil.GetBytesBE((ulong)primary);
                InitDiffusionValue(rtb, diffusionBytes, primaryBytes);
            }
            else
            {
                byte[] diffusionBytes = ByteUtil.GetBytesBE((uint)diffusion);
                byte[] primaryBytes = ByteUtil.GetBytesBE((uint)primary);
                InitDiffusionValue(rtb, diffusionBytes, primaryBytes);
            }
        }
    }
}