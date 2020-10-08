using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {
        private byte[] _diffusionKey;
        public byte[] DiffusionKey {
            get {
                return _diffusionKey;
            }
            set {
                _diffusionKey = value;
                for (int i = 0; i < 256; ++i)
                {
                    OnPropertyChanged($"DKeyBit{i}");
                }
                for (int i = 0; i < 64; ++i)
                {
                    OnPropertyChanged($"DKeyNibble{i}");
                }
            }
        }
        private int Bit(byte b, int bitIndex)
        {
            return (b & (1 << bitIndex)) != 0 ? 1 : 0;
        }
        private int Bit(byte[] bytes, int bitIndex)
        {
            Debug.Assert(0 <= bitIndex, $"bitindex ({bitIndex}) was lower than zero.");
            Debug.Assert(bitIndex < 256, $"bitIndex ({bitIndex}) was higher than 255.");
            (int byteIndex, int bitIndexInByte) = GetByteArrayIndices(bitIndex);
            return Bit(bytes[byteIndex], bitIndexInByte);
        }
        private (int, int) GetByteArrayIndices(int bitIndex)
        {
            int byteIndex = bitIndex / 8;
            int bitIndexInByte = 7 - (bitIndex % 8);
            return (byteIndex, bitIndexInByte);
        }
        public void SetDKeyBit(int bitIndex)
        {
            Debug.Assert(0 <= bitIndex, $"bitindex ({bitIndex}) was lower than zero.");
            Debug.Assert(bitIndex < 256, $"bitIndex ({bitIndex}) was higher than 255.");
            (int byteIndex, int bitIndexInByte) = GetByteArrayIndices(bitIndex);
            DiffusionKey[byteIndex] = (byte)(DiffusionKey[byteIndex] | (1 << bitIndexInByte));
        }
        public void UnsetDKeyBit(int bitIndex)
        {
            Debug.Assert(0 <= bitIndex, $"bitindex ({bitIndex}) was lower than zero.");
            Debug.Assert(bitIndex < 256, $"bitIndex ({bitIndex}) was higher than 255.");
            (int byteIndex, int bitIndexInByte) = GetByteArrayIndices(bitIndex);
            DiffusionKey[byteIndex] = (byte)(DiffusionKey[byteIndex] & ~(1 << bitIndexInByte));
        }
        public int DKeyBit(int bitIndex)
        {
            return Bit(DiffusionKey, bitIndex);
        }
        #region Diffusion Key Bits
        // KeyBits for binding to the diffusion buttons.
        // Generated with a python script (... Can this already be considered meta-programming?)
        // The zero-th bit is the most significant bit since it belongs to the byte the most to the left in the visualization.
        // I have chosen it this way since the byte array is in the same order as seen in the visualization.
        // (The first byte of InputKey is the first byte seen in the visualization thus the most significant byte.)
        // 
        public int DKeyBit0
        {
            get
            {
                return DKeyBit(0);
            }
        }
        public int DKeyBit1
        {
            get
            {
                return DKeyBit(1);
            }
        }
        public int DKeyBit2
        {
            get
            {
                return DKeyBit(2);
            }
        }
        public int DKeyBit3
        {
            get
            {
                return DKeyBit(3);
            }
        }
        public int DKeyBit4
        {
            get
            {
                return DKeyBit(4);
            }
        }
        public int DKeyBit5
        {
            get
            {
                return DKeyBit(5);
            }
        }
        public int DKeyBit6
        {
            get
            {
                return DKeyBit(6);
            }
        }
        public int DKeyBit7
        {
            get
            {
                return DKeyBit(7);
            }
        }
        public int DKeyBit8
        {
            get
            {
                return DKeyBit(8);
            }
        }
        public int DKeyBit9
        {
            get
            {
                return DKeyBit(9);
            }
        }
        public int DKeyBit10
        {
            get
            {
                return DKeyBit(10);
            }
        }
        public int DKeyBit11
        {
            get
            {
                return DKeyBit(11);
            }
        }
        public int DKeyBit12
        {
            get
            {
                return DKeyBit(12);
            }
        }
        public int DKeyBit13
        {
            get
            {
                return DKeyBit(13);
            }
        }
        public int DKeyBit14
        {
            get
            {
                return DKeyBit(14);
            }
        }
        public int DKeyBit15
        {
            get
            {
                return DKeyBit(15);
            }
        }
        public int DKeyBit16
        {
            get
            {
                return DKeyBit(16);
            }
        }
        public int DKeyBit17
        {
            get
            {
                return DKeyBit(17);
            }
        }
        public int DKeyBit18
        {
            get
            {
                return DKeyBit(18);
            }
        }
        public int DKeyBit19
        {
            get
            {
                return DKeyBit(19);
            }
        }
        public int DKeyBit20
        {
            get
            {
                return DKeyBit(20);
            }
        }
        public int DKeyBit21
        {
            get
            {
                return DKeyBit(21);
            }
        }
        public int DKeyBit22
        {
            get
            {
                return DKeyBit(22);
            }
        }
        public int DKeyBit23
        {
            get
            {
                return DKeyBit(23);
            }
        }
        public int DKeyBit24
        {
            get
            {
                return DKeyBit(24);
            }
        }
        public int DKeyBit25
        {
            get
            {
                return DKeyBit(25);
            }
        }
        public int DKeyBit26
        {
            get
            {
                return DKeyBit(26);
            }
        }
        public int DKeyBit27
        {
            get
            {
                return DKeyBit(27);
            }
        }
        public int DKeyBit28
        {
            get
            {
                return DKeyBit(28);
            }
        }
        public int DKeyBit29
        {
            get
            {
                return DKeyBit(29);
            }
        }
        public int DKeyBit30
        {
            get
            {
                return DKeyBit(30);
            }
        }
        public int DKeyBit31
        {
            get
            {
                return DKeyBit(31);
            }
        }
        public int DKeyBit32
        {
            get
            {
                return DKeyBit(32);
            }
        }
        public int DKeyBit33
        {
            get
            {
                return DKeyBit(33);
            }
        }
        public int DKeyBit34
        {
            get
            {
                return DKeyBit(34);
            }
        }
        public int DKeyBit35
        {
            get
            {
                return DKeyBit(35);
            }
        }
        public int DKeyBit36
        {
            get
            {
                return DKeyBit(36);
            }
        }
        public int DKeyBit37
        {
            get
            {
                return DKeyBit(37);
            }
        }
        public int DKeyBit38
        {
            get
            {
                return DKeyBit(38);
            }
        }
        public int DKeyBit39
        {
            get
            {
                return DKeyBit(39);
            }
        }
        public int DKeyBit40
        {
            get
            {
                return DKeyBit(40);
            }
        }
        public int DKeyBit41
        {
            get
            {
                return DKeyBit(41);
            }
        }
        public int DKeyBit42
        {
            get
            {
                return DKeyBit(42);
            }
        }
        public int DKeyBit43
        {
            get
            {
                return DKeyBit(43);
            }
        }
        public int DKeyBit44
        {
            get
            {
                return DKeyBit(44);
            }
        }
        public int DKeyBit45
        {
            get
            {
                return DKeyBit(45);
            }
        }
        public int DKeyBit46
        {
            get
            {
                return DKeyBit(46);
            }
        }
        public int DKeyBit47
        {
            get
            {
                return DKeyBit(47);
            }
        }
        public int DKeyBit48
        {
            get
            {
                return DKeyBit(48);
            }
        }
        public int DKeyBit49
        {
            get
            {
                return DKeyBit(49);
            }
        }
        public int DKeyBit50
        {
            get
            {
                return DKeyBit(50);
            }
        }
        public int DKeyBit51
        {
            get
            {
                return DKeyBit(51);
            }
        }
        public int DKeyBit52
        {
            get
            {
                return DKeyBit(52);
            }
        }
        public int DKeyBit53
        {
            get
            {
                return DKeyBit(53);
            }
        }
        public int DKeyBit54
        {
            get
            {
                return DKeyBit(54);
            }
        }
        public int DKeyBit55
        {
            get
            {
                return DKeyBit(55);
            }
        }
        public int DKeyBit56
        {
            get
            {
                return DKeyBit(56);
            }
        }
        public int DKeyBit57
        {
            get
            {
                return DKeyBit(57);
            }
        }
        public int DKeyBit58
        {
            get
            {
                return DKeyBit(58);
            }
        }
        public int DKeyBit59
        {
            get
            {
                return DKeyBit(59);
            }
        }
        public int DKeyBit60
        {
            get
            {
                return DKeyBit(60);
            }
        }
        public int DKeyBit61
        {
            get
            {
                return DKeyBit(61);
            }
        }
        public int DKeyBit62
        {
            get
            {
                return DKeyBit(62);
            }
        }
        public int DKeyBit63
        {
            get
            {
                return DKeyBit(63);
            }
        }
        public int DKeyBit64
        {
            get
            {
                return DKeyBit(64);
            }
        }
        public int DKeyBit65
        {
            get
            {
                return DKeyBit(65);
            }
        }
        public int DKeyBit66
        {
            get
            {
                return DKeyBit(66);
            }
        }
        public int DKeyBit67
        {
            get
            {
                return DKeyBit(67);
            }
        }
        public int DKeyBit68
        {
            get
            {
                return DKeyBit(68);
            }
        }
        public int DKeyBit69
        {
            get
            {
                return DKeyBit(69);
            }
        }
        public int DKeyBit70
        {
            get
            {
                return DKeyBit(70);
            }
        }
        public int DKeyBit71
        {
            get
            {
                return DKeyBit(71);
            }
        }
        public int DKeyBit72
        {
            get
            {
                return DKeyBit(72);
            }
        }
        public int DKeyBit73
        {
            get
            {
                return DKeyBit(73);
            }
        }
        public int DKeyBit74
        {
            get
            {
                return DKeyBit(74);
            }
        }
        public int DKeyBit75
        {
            get
            {
                return DKeyBit(75);
            }
        }
        public int DKeyBit76
        {
            get
            {
                return DKeyBit(76);
            }
        }
        public int DKeyBit77
        {
            get
            {
                return DKeyBit(77);
            }
        }
        public int DKeyBit78
        {
            get
            {
                return DKeyBit(78);
            }
        }
        public int DKeyBit79
        {
            get
            {
                return DKeyBit(79);
            }
        }
        public int DKeyBit80
        {
            get
            {
                return DKeyBit(80);
            }
        }
        public int DKeyBit81
        {
            get
            {
                return DKeyBit(81);
            }
        }
        public int DKeyBit82
        {
            get
            {
                return DKeyBit(82);
            }
        }
        public int DKeyBit83
        {
            get
            {
                return DKeyBit(83);
            }
        }
        public int DKeyBit84
        {
            get
            {
                return DKeyBit(84);
            }
        }
        public int DKeyBit85
        {
            get
            {
                return DKeyBit(85);
            }
        }
        public int DKeyBit86
        {
            get
            {
                return DKeyBit(86);
            }
        }
        public int DKeyBit87
        {
            get
            {
                return DKeyBit(87);
            }
        }
        public int DKeyBit88
        {
            get
            {
                return DKeyBit(88);
            }
        }
        public int DKeyBit89
        {
            get
            {
                return DKeyBit(89);
            }
        }
        public int DKeyBit90
        {
            get
            {
                return DKeyBit(90);
            }
        }
        public int DKeyBit91
        {
            get
            {
                return DKeyBit(91);
            }
        }
        public int DKeyBit92
        {
            get
            {
                return DKeyBit(92);
            }
        }
        public int DKeyBit93
        {
            get
            {
                return DKeyBit(93);
            }
        }
        public int DKeyBit94
        {
            get
            {
                return DKeyBit(94);
            }
        }
        public int DKeyBit95
        {
            get
            {
                return DKeyBit(95);
            }
        }
        public int DKeyBit96
        {
            get
            {
                return DKeyBit(96);
            }
        }
        public int DKeyBit97
        {
            get
            {
                return DKeyBit(97);
            }
        }
        public int DKeyBit98
        {
            get
            {
                return DKeyBit(98);
            }
        }
        public int DKeyBit99
        {
            get
            {
                return DKeyBit(99);
            }
        }
        public int DKeyBit100
        {
            get
            {
                return DKeyBit(100);
            }
        }
        public int DKeyBit101
        {
            get
            {
                return DKeyBit(101);
            }
        }
        public int DKeyBit102
        {
            get
            {
                return DKeyBit(102);
            }
        }
        public int DKeyBit103
        {
            get
            {
                return DKeyBit(103);
            }
        }
        public int DKeyBit104
        {
            get
            {
                return DKeyBit(104);
            }
        }
        public int DKeyBit105
        {
            get
            {
                return DKeyBit(105);
            }
        }
        public int DKeyBit106
        {
            get
            {
                return DKeyBit(106);
            }
        }
        public int DKeyBit107
        {
            get
            {
                return DKeyBit(107);
            }
        }
        public int DKeyBit108
        {
            get
            {
                return DKeyBit(108);
            }
        }
        public int DKeyBit109
        {
            get
            {
                return DKeyBit(109);
            }
        }
        public int DKeyBit110
        {
            get
            {
                return DKeyBit(110);
            }
        }
        public int DKeyBit111
        {
            get
            {
                return DKeyBit(111);
            }
        }
        public int DKeyBit112
        {
            get
            {
                return DKeyBit(112);
            }
        }
        public int DKeyBit113
        {
            get
            {
                return DKeyBit(113);
            }
        }
        public int DKeyBit114
        {
            get
            {
                return DKeyBit(114);
            }
        }
        public int DKeyBit115
        {
            get
            {
                return DKeyBit(115);
            }
        }
        public int DKeyBit116
        {
            get
            {
                return DKeyBit(116);
            }
        }
        public int DKeyBit117
        {
            get
            {
                return DKeyBit(117);
            }
        }
        public int DKeyBit118
        {
            get
            {
                return DKeyBit(118);
            }
        }
        public int DKeyBit119
        {
            get
            {
                return DKeyBit(119);
            }
        }
        public int DKeyBit120
        {
            get
            {
                return DKeyBit(120);
            }
        }
        public int DKeyBit121
        {
            get
            {
                return DKeyBit(121);
            }
        }
        public int DKeyBit122
        {
            get
            {
                return DKeyBit(122);
            }
        }
        public int DKeyBit123
        {
            get
            {
                return DKeyBit(123);
            }
        }
        public int DKeyBit124
        {
            get
            {
                return DKeyBit(124);
            }
        }
        public int DKeyBit125
        {
            get
            {
                return DKeyBit(125);
            }
        }
        public int DKeyBit126
        {
            get
            {
                return DKeyBit(126);
            }
        }
        public int DKeyBit127
        {
            get
            {
                return DKeyBit(127);
            }
        }
        public int DKeyBit128
        {
            get
            {
                return DKeyBit(128);
            }
        }
        public int DKeyBit129
        {
            get
            {
                return DKeyBit(129);
            }
        }
        public int DKeyBit130
        {
            get
            {
                return DKeyBit(130);
            }
        }
        public int DKeyBit131
        {
            get
            {
                return DKeyBit(131);
            }
        }
        public int DKeyBit132
        {
            get
            {
                return DKeyBit(132);
            }
        }
        public int DKeyBit133
        {
            get
            {
                return DKeyBit(133);
            }
        }
        public int DKeyBit134
        {
            get
            {
                return DKeyBit(134);
            }
        }
        public int DKeyBit135
        {
            get
            {
                return DKeyBit(135);
            }
        }
        public int DKeyBit136
        {
            get
            {
                return DKeyBit(136);
            }
        }
        public int DKeyBit137
        {
            get
            {
                return DKeyBit(137);
            }
        }
        public int DKeyBit138
        {
            get
            {
                return DKeyBit(138);
            }
        }
        public int DKeyBit139
        {
            get
            {
                return DKeyBit(139);
            }
        }
        public int DKeyBit140
        {
            get
            {
                return DKeyBit(140);
            }
        }
        public int DKeyBit141
        {
            get
            {
                return DKeyBit(141);
            }
        }
        public int DKeyBit142
        {
            get
            {
                return DKeyBit(142);
            }
        }
        public int DKeyBit143
        {
            get
            {
                return DKeyBit(143);
            }
        }
        public int DKeyBit144
        {
            get
            {
                return DKeyBit(144);
            }
        }
        public int DKeyBit145
        {
            get
            {
                return DKeyBit(145);
            }
        }
        public int DKeyBit146
        {
            get
            {
                return DKeyBit(146);
            }
        }
        public int DKeyBit147
        {
            get
            {
                return DKeyBit(147);
            }
        }
        public int DKeyBit148
        {
            get
            {
                return DKeyBit(148);
            }
        }
        public int DKeyBit149
        {
            get
            {
                return DKeyBit(149);
            }
        }
        public int DKeyBit150
        {
            get
            {
                return DKeyBit(150);
            }
        }
        public int DKeyBit151
        {
            get
            {
                return DKeyBit(151);
            }
        }
        public int DKeyBit152
        {
            get
            {
                return DKeyBit(152);
            }
        }
        public int DKeyBit153
        {
            get
            {
                return DKeyBit(153);
            }
        }
        public int DKeyBit154
        {
            get
            {
                return DKeyBit(154);
            }
        }
        public int DKeyBit155
        {
            get
            {
                return DKeyBit(155);
            }
        }
        public int DKeyBit156
        {
            get
            {
                return DKeyBit(156);
            }
        }
        public int DKeyBit157
        {
            get
            {
                return DKeyBit(157);
            }
        }
        public int DKeyBit158
        {
            get
            {
                return DKeyBit(158);
            }
        }
        public int DKeyBit159
        {
            get
            {
                return DKeyBit(159);
            }
        }
        public int DKeyBit160
        {
            get
            {
                return DKeyBit(160);
            }
        }
        public int DKeyBit161
        {
            get
            {
                return DKeyBit(161);
            }
        }
        public int DKeyBit162
        {
            get
            {
                return DKeyBit(162);
            }
        }
        public int DKeyBit163
        {
            get
            {
                return DKeyBit(163);
            }
        }
        public int DKeyBit164
        {
            get
            {
                return DKeyBit(164);
            }
        }
        public int DKeyBit165
        {
            get
            {
                return DKeyBit(165);
            }
        }
        public int DKeyBit166
        {
            get
            {
                return DKeyBit(166);
            }
        }
        public int DKeyBit167
        {
            get
            {
                return DKeyBit(167);
            }
        }
        public int DKeyBit168
        {
            get
            {
                return DKeyBit(168);
            }
        }
        public int DKeyBit169
        {
            get
            {
                return DKeyBit(169);
            }
        }
        public int DKeyBit170
        {
            get
            {
                return DKeyBit(170);
            }
        }
        public int DKeyBit171
        {
            get
            {
                return DKeyBit(171);
            }
        }
        public int DKeyBit172
        {
            get
            {
                return DKeyBit(172);
            }
        }
        public int DKeyBit173
        {
            get
            {
                return DKeyBit(173);
            }
        }
        public int DKeyBit174
        {
            get
            {
                return DKeyBit(174);
            }
        }
        public int DKeyBit175
        {
            get
            {
                return DKeyBit(175);
            }
        }
        public int DKeyBit176
        {
            get
            {
                return DKeyBit(176);
            }
        }
        public int DKeyBit177
        {
            get
            {
                return DKeyBit(177);
            }
        }
        public int DKeyBit178
        {
            get
            {
                return DKeyBit(178);
            }
        }
        public int DKeyBit179
        {
            get
            {
                return DKeyBit(179);
            }
        }
        public int DKeyBit180
        {
            get
            {
                return DKeyBit(180);
            }
        }
        public int DKeyBit181
        {
            get
            {
                return DKeyBit(181);
            }
        }
        public int DKeyBit182
        {
            get
            {
                return DKeyBit(182);
            }
        }
        public int DKeyBit183
        {
            get
            {
                return DKeyBit(183);
            }
        }
        public int DKeyBit184
        {
            get
            {
                return DKeyBit(184);
            }
        }
        public int DKeyBit185
        {
            get
            {
                return DKeyBit(185);
            }
        }
        public int DKeyBit186
        {
            get
            {
                return DKeyBit(186);
            }
        }
        public int DKeyBit187
        {
            get
            {
                return DKeyBit(187);
            }
        }
        public int DKeyBit188
        {
            get
            {
                return DKeyBit(188);
            }
        }
        public int DKeyBit189
        {
            get
            {
                return DKeyBit(189);
            }
        }
        public int DKeyBit190
        {
            get
            {
                return DKeyBit(190);
            }
        }
        public int DKeyBit191
        {
            get
            {
                return DKeyBit(191);
            }
        }
        public int DKeyBit192
        {
            get
            {
                return DKeyBit(192);
            }
        }
        public int DKeyBit193
        {
            get
            {
                return DKeyBit(193);
            }
        }
        public int DKeyBit194
        {
            get
            {
                return DKeyBit(194);
            }
        }
        public int DKeyBit195
        {
            get
            {
                return DKeyBit(195);
            }
        }
        public int DKeyBit196
        {
            get
            {
                return DKeyBit(196);
            }
        }
        public int DKeyBit197
        {
            get
            {
                return DKeyBit(197);
            }
        }
        public int DKeyBit198
        {
            get
            {
                return DKeyBit(198);
            }
        }
        public int DKeyBit199
        {
            get
            {
                return DKeyBit(199);
            }
        }
        public int DKeyBit200
        {
            get
            {
                return DKeyBit(200);
            }
        }
        public int DKeyBit201
        {
            get
            {
                return DKeyBit(201);
            }
        }
        public int DKeyBit202
        {
            get
            {
                return DKeyBit(202);
            }
        }
        public int DKeyBit203
        {
            get
            {
                return DKeyBit(203);
            }
        }
        public int DKeyBit204
        {
            get
            {
                return DKeyBit(204);
            }
        }
        public int DKeyBit205
        {
            get
            {
                return DKeyBit(205);
            }
        }
        public int DKeyBit206
        {
            get
            {
                return DKeyBit(206);
            }
        }
        public int DKeyBit207
        {
            get
            {
                return DKeyBit(207);
            }
        }
        public int DKeyBit208
        {
            get
            {
                return DKeyBit(208);
            }
        }
        public int DKeyBit209
        {
            get
            {
                return DKeyBit(209);
            }
        }
        public int DKeyBit210
        {
            get
            {
                return DKeyBit(210);
            }
        }
        public int DKeyBit211
        {
            get
            {
                return DKeyBit(211);
            }
        }
        public int DKeyBit212
        {
            get
            {
                return DKeyBit(212);
            }
        }
        public int DKeyBit213
        {
            get
            {
                return DKeyBit(213);
            }
        }
        public int DKeyBit214
        {
            get
            {
                return DKeyBit(214);
            }
        }
        public int DKeyBit215
        {
            get
            {
                return DKeyBit(215);
            }
        }
        public int DKeyBit216
        {
            get
            {
                return DKeyBit(216);
            }
        }
        public int DKeyBit217
        {
            get
            {
                return DKeyBit(217);
            }
        }
        public int DKeyBit218
        {
            get
            {
                return DKeyBit(218);
            }
        }
        public int DKeyBit219
        {
            get
            {
                return DKeyBit(219);
            }
        }
        public int DKeyBit220
        {
            get
            {
                return DKeyBit(220);
            }
        }
        public int DKeyBit221
        {
            get
            {
                return DKeyBit(221);
            }
        }
        public int DKeyBit222
        {
            get
            {
                return DKeyBit(222);
            }
        }
        public int DKeyBit223
        {
            get
            {
                return DKeyBit(223);
            }
        }
        public int DKeyBit224
        {
            get
            {
                return DKeyBit(224);
            }
        }
        public int DKeyBit225
        {
            get
            {
                return DKeyBit(225);
            }
        }
        public int DKeyBit226
        {
            get
            {
                return DKeyBit(226);
            }
        }
        public int DKeyBit227
        {
            get
            {
                return DKeyBit(227);
            }
        }
        public int DKeyBit228
        {
            get
            {
                return DKeyBit(228);
            }
        }
        public int DKeyBit229
        {
            get
            {
                return DKeyBit(229);
            }
        }
        public int DKeyBit230
        {
            get
            {
                return DKeyBit(230);
            }
        }
        public int DKeyBit231
        {
            get
            {
                return DKeyBit(231);
            }
        }
        public int DKeyBit232
        {
            get
            {
                return DKeyBit(232);
            }
        }
        public int DKeyBit233
        {
            get
            {
                return DKeyBit(233);
            }
        }
        public int DKeyBit234
        {
            get
            {
                return DKeyBit(234);
            }
        }
        public int DKeyBit235
        {
            get
            {
                return DKeyBit(235);
            }
        }
        public int DKeyBit236
        {
            get
            {
                return DKeyBit(236);
            }
        }
        public int DKeyBit237
        {
            get
            {
                return DKeyBit(237);
            }
        }
        public int DKeyBit238
        {
            get
            {
                return DKeyBit(238);
            }
        }
        public int DKeyBit239
        {
            get
            {
                return DKeyBit(239);
            }
        }
        public int DKeyBit240
        {
            get
            {
                return DKeyBit(240);
            }
        }
        public int DKeyBit241
        {
            get
            {
                return DKeyBit(241);
            }
        }
        public int DKeyBit242
        {
            get
            {
                return DKeyBit(242);
            }
        }
        public int DKeyBit243
        {
            get
            {
                return DKeyBit(243);
            }
        }
        public int DKeyBit244
        {
            get
            {
                return DKeyBit(244);
            }
        }
        public int DKeyBit245
        {
            get
            {
                return DKeyBit(245);
            }
        }
        public int DKeyBit246
        {
            get
            {
                return DKeyBit(246);
            }
        }
        public int DKeyBit247
        {
            get
            {
                return DKeyBit(247);
            }
        }
        public int DKeyBit248
        {
            get
            {
                return DKeyBit(248);
            }
        }
        public int DKeyBit249
        {
            get
            {
                return DKeyBit(249);
            }
        }
        public int DKeyBit250
        {
            get
            {
                return DKeyBit(250);
            }
        }
        public int DKeyBit251
        {
            get
            {
                return DKeyBit(251);
            }
        }
        public int DKeyBit252
        {
            get
            {
                return DKeyBit(252);
            }
        }
        public int DKeyBit253
        {
            get
            {
                return DKeyBit(253);
            }
        }
        public int DKeyBit254
        {
            get
            {
                return DKeyBit(254);
            }
        }
        public int DKeyBit255
        {
            get
            {
                return DKeyBit(255);
            }
        }
        #endregion

        public int DKeyNibble(int nibbleIndex)
        {
            Debug.Assert(0 <= nibbleIndex, $"nibbleIndex ({nibbleIndex}) was lower than zero.");
            Debug.Assert(nibbleIndex < 64, $"nibbleIndex ({nibbleIndex}) was higher than 63.");
            // most significant bit
            int msb = Bit(DiffusionKey, nibbleIndex * 4);
            int a = Bit(DiffusionKey, (nibbleIndex * 4) + 1);
            int b = Bit(DiffusionKey, (nibbleIndex * 4) + 2);
            // least significant bit
            int lsb = Bit(DiffusionKey, (nibbleIndex * 4) + 3);
            return (msb << 3) + (a << 2) + (b << 1) + lsb;
        }
        public string DKeyNibbleHex(int nibbleIndex)
        {
            return DKeyNibble(nibbleIndex).ToString("X");
        }
        #region Diffusion Key Nibbles
        public string DKeyNibbleHex0
        {
            get
            {
                return DKeyNibbleHex(0);
            }
        }
        public string DKeyNibbleHex1
        {
            get
            {
                return DKeyNibbleHex(1);
            }
        }
        public string DKeyNibbleHex2
        {
            get
            {
                return DKeyNibbleHex(2);
            }
        }
        public string DKeyNibbleHex3
        {
            get
            {
                return DKeyNibbleHex(3);
            }
        }
        public string DKeyNibbleHex4
        {
            get
            {
                return DKeyNibbleHex(4);
            }
        }
        public string DKeyNibbleHex5
        {
            get
            {
                return DKeyNibbleHex(5);
            }
        }
        public string DKeyNibbleHex6
        {
            get
            {
                return DKeyNibbleHex(6);
            }
        }
        public string DKeyNibbleHex7
        {
            get
            {
                return DKeyNibbleHex(7);
            }
        }
        public string DKeyNibbleHex8
        {
            get
            {
                return DKeyNibbleHex(8);
            }
        }
        public string DKeyNibbleHex9
        {
            get
            {
                return DKeyNibbleHex(9);
            }
        }
        public string DKeyNibbleHex10
        {
            get
            {
                return DKeyNibbleHex(10);
            }
        }
        public string DKeyNibbleHex11
        {
            get
            {
                return DKeyNibbleHex(11);
            }
        }
        public string DKeyNibbleHex12
        {
            get
            {
                return DKeyNibbleHex(12);
            }
        }
        public string DKeyNibbleHex13
        {
            get
            {
                return DKeyNibbleHex(13);
            }
        }
        public string DKeyNibbleHex14
        {
            get
            {
                return DKeyNibbleHex(14);
            }
        }
        public string DKeyNibbleHex15
        {
            get
            {
                return DKeyNibbleHex(15);
            }
        }
        public string DKeyNibbleHex16
        {
            get
            {
                return DKeyNibbleHex(16);
            }
        }
        public string DKeyNibbleHex17
        {
            get
            {
                return DKeyNibbleHex(17);
            }
        }
        public string DKeyNibbleHex18
        {
            get
            {
                return DKeyNibbleHex(18);
            }
        }
        public string DKeyNibbleHex19
        {
            get
            {
                return DKeyNibbleHex(19);
            }
        }
        public string DKeyNibbleHex20
        {
            get
            {
                return DKeyNibbleHex(20);
            }
        }
        public string DKeyNibbleHex21
        {
            get
            {
                return DKeyNibbleHex(21);
            }
        }
        public string DKeyNibbleHex22
        {
            get
            {
                return DKeyNibbleHex(22);
            }
        }
        public string DKeyNibbleHex23
        {
            get
            {
                return DKeyNibbleHex(23);
            }
        }
        public string DKeyNibbleHex24
        {
            get
            {
                return DKeyNibbleHex(24);
            }
        }
        public string DKeyNibbleHex25
        {
            get
            {
                return DKeyNibbleHex(25);
            }
        }
        public string DKeyNibbleHex26
        {
            get
            {
                return DKeyNibbleHex(26);
            }
        }
        public string DKeyNibbleHex27
        {
            get
            {
                return DKeyNibbleHex(27);
            }
        }
        public string DKeyNibbleHex28
        {
            get
            {
                return DKeyNibbleHex(28);
            }
        }
        public string DKeyNibbleHex29
        {
            get
            {
                return DKeyNibbleHex(29);
            }
        }
        public string DKeyNibbleHex30
        {
            get
            {
                return DKeyNibbleHex(30);
            }
        }
        public string DKeyNibbleHex31
        {
            get
            {
                return DKeyNibbleHex(31);
            }
        }
        public string DKeyNibbleHex32
        {
            get
            {
                return DKeyNibbleHex(32);
            }
        }
        public string DKeyNibbleHex33
        {
            get
            {
                return DKeyNibbleHex(33);
            }
        }
        public string DKeyNibbleHex34
        {
            get
            {
                return DKeyNibbleHex(34);
            }
        }
        public string DKeyNibbleHex35
        {
            get
            {
                return DKeyNibbleHex(35);
            }
        }
        public string DKeyNibbleHex36
        {
            get
            {
                return DKeyNibbleHex(36);
            }
        }
        public string DKeyNibbleHex37
        {
            get
            {
                return DKeyNibbleHex(37);
            }
        }
        public string DKeyNibbleHex38
        {
            get
            {
                return DKeyNibbleHex(38);
            }
        }
        public string DKeyNibbleHex39
        {
            get
            {
                return DKeyNibbleHex(39);
            }
        }
        public string DKeyNibbleHex40
        {
            get
            {
                return DKeyNibbleHex(40);
            }
        }
        public string DKeyNibbleHex41
        {
            get
            {
                return DKeyNibbleHex(41);
            }
        }
        public string DKeyNibbleHex42
        {
            get
            {
                return DKeyNibbleHex(42);
            }
        }
        public string DKeyNibbleHex43
        {
            get
            {
                return DKeyNibbleHex(43);
            }
        }
        public string DKeyNibbleHex44
        {
            get
            {
                return DKeyNibbleHex(44);
            }
        }
        public string DKeyNibbleHex45
        {
            get
            {
                return DKeyNibbleHex(45);
            }
        }
        public string DKeyNibbleHex46
        {
            get
            {
                return DKeyNibbleHex(46);
            }
        }
        public string DKeyNibbleHex47
        {
            get
            {
                return DKeyNibbleHex(47);
            }
        }
        public string DKeyNibbleHex48
        {
            get
            {
                return DKeyNibbleHex(48);
            }
        }
        public string DKeyNibbleHex49
        {
            get
            {
                return DKeyNibbleHex(49);
            }
        }
        public string DKeyNibbleHex50
        {
            get
            {
                return DKeyNibbleHex(50);
            }
        }
        public string DKeyNibbleHex51
        {
            get
            {
                return DKeyNibbleHex(51);
            }
        }
        public string DKeyNibbleHex52
        {
            get
            {
                return DKeyNibbleHex(52);
            }
        }
        public string DKeyNibbleHex53
        {
            get
            {
                return DKeyNibbleHex(53);
            }
        }
        public string DKeyNibbleHex54
        {
            get
            {
                return DKeyNibbleHex(54);
            }
        }
        public string DKeyNibbleHex55
        {
            get
            {
                return DKeyNibbleHex(55);
            }
        }
        public string DKeyNibbleHex56
        {
            get
            {
                return DKeyNibbleHex(56);
            }
        }
        public string DKeyNibbleHex57
        {
            get
            {
                return DKeyNibbleHex(57);
            }
        }
        public string DKeyNibbleHex58
        {
            get
            {
                return DKeyNibbleHex(58);
            }
        }
        public string DKeyNibbleHex59
        {
            get
            {
                return DKeyNibbleHex(59);
            }
        }
        public string DKeyNibbleHex60
        {
            get
            {
                return DKeyNibbleHex(60);
            }
        }
        public string DKeyNibbleHex61
        {
            get
            {
                return DKeyNibbleHex(61);
            }
        }
        public string DKeyNibbleHex62
        {
            get
            {
                return DKeyNibbleHex(62);
            }
        }
        public string DKeyNibbleHex63
        {
            get
            {
                return DKeyNibbleHex(63);
            }
        }

        #endregion
    }
    class StateMatrixPage : Page
    {
        private ChaChaPresentation pres;
        private TextBlock diffusionKeyTextBlock;
        private Button toggleShowDiffusion;
        private Grid diffusionGrid;

        private List<string> descriptions = new List<string>();
        private bool versionIsDJB;
        private bool keyIs32Byte;
        public StateMatrixPage(ContentControl pageElement, ChaChaPresentation pres_) : base(pageElement)
        {
            pres = pres_;
            versionIsDJB = pres.Version == ChaCha.Version.DJB;
            keyIs32Byte = pres.InputKey.Length == 32;
            descriptions.Add("The 512-bit (128-byte) ChaCha state can be interpreted as a 4x4 matrix, where each entry consists of 4 bytes interpreted as little-endian. The first 16 bytes consist of the constants. ");
            descriptions.Add("The next 32 bytes consist of the key. If the key consists of only 16 bytes, it is concatenated with itself. ");
            descriptions.Add($"The next { pres.InitialCounter.Length} bytes consist of the counter.Since this is our first keystream block, we set the counter to zero.The counter is special since we first reverse all bytes before applying the transformations but since the counter is zero, this does not matter for the first block. ");
            descriptions.Add($"The last {pres.InputIV.Length} bytes consist of the initialization vector. ");
            descriptions.Add("On the next page, we will use this initialized state matrix to generate the first keystream block.");

            Init();
        }

        private void Init()
        {
            AddConstantsActions();
            AddKeyActions();
            AddCounterActions();
            AddIVActions();
            PageAction nextPageDesc = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[4]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInputIV);
                ReplaceTransformChunkIV();
                ReplaceTransformLittleEndianIV();
            });
            AddAction(nextPageDesc);
            InitDiffusion();
        }

        private void InitDiffusion()
        {
            toggleShowDiffusion = pres.ToggleShowDiffusion;
            diffusionGrid = pres.DiffusionGrid;
            diffusionKeyTextBlock = pres.DiffusionKeyTextBlock;
            pres.DiffusionKey = (byte[])pres.InputKey.Clone();
            InitToggleDiffusionButton();
            InitDiffusionKey();
            InitDiffusionGridLayout();
            InitDiffusionFlipBitButtons();
        }
        private void ToggleDiffusion(object sender, RoutedEventArgs e)
        {
            Visibility set = diffusionGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            diffusionGrid.Visibility = set;
            diffusionKeyTextBlock.Visibility = set;
        }
        private void InitToggleDiffusionButton()
        {
            toggleShowDiffusion.Click += ToggleDiffusion;
        }
        private TextBlock CreateDiffusionKeyNibbleTextBox(int nibbleIndex)
        {
            TextBlock tb = new TextBlock();
            tb.SetBinding(TextBlock.TextProperty, new Binding($"DKeyNibbleHex{nibbleIndex}"));

            Setter markNibble = new Setter() { Property = TextBlock.ForegroundProperty, Value = Brushes.Red };
            Trigger onHover = new Trigger()
            {
                Property = TextBlock.IsMouseOverProperty,
                Value = true
            };
            onHover.Setters.Add(markNibble);

            DataTrigger onButtonHover = new DataTrigger() {
                Binding = new Binding() { ElementName = $"DKeyBit{nibbleIndex}Button", Path = new PropertyPath("IsMouseOver") },
                Value = true
            };
            onButtonHover.Setters.Add(markNibble);

            Style s = new Style();
            s.Triggers.Add(onHover);
            s.Triggers.Add(onButtonHover);
            tb.Style = s;
            return tb;
        }
        private void InitDiffusionKey()
        {
            for (int i = 0; i < (keyIs32Byte ? 32 : 16) * 2; ++i)
            {
                TextBlock nibbleBox = CreateDiffusionKeyNibbleTextBox(i);
                diffusionKeyTextBlock.Inlines.Add(nibbleBox);
            }
        }
        private void InitDiffusionGridLayout()
        {
            for (int i = 0; i < 64; ++i)
            {
                diffusionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            diffusionGrid.RowDefinitions.Add(new RowDefinition());
            diffusionGrid.RowDefinitions.Add(new RowDefinition());
            if (keyIs32Byte)
            {
                diffusionGrid.RowDefinitions.Add(new RowDefinition());
                diffusionGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
        private void FlipDiffusionBit(int bitIndex)
        {
            int bit = pres.DKeyBit(bitIndex);
            if(bit == 1)
            {
                pres.UnsetDKeyBit(bitIndex);
            }
            else
            {
                pres.SetDKeyBit(bitIndex);
            }
        }
        private Button CreateDiffusionButton(int bitIndex)
        {
            // Bit indices start at 0 on the most significant bit which is in the string representation in big endian notation.
            // This means we start counting from zero at the left but the zero-th bit is - maybe a bit unintuitively - the most significant bit.
            Button b = new Button() { Height = 16, FontSize = 10 };
            b.SetBinding(Button.ContentProperty, new Binding($"DKeyBit{bitIndex}"));
            b.Margin = new Thickness(bitIndex % 4 == 0 ? 3 : 0, 0, 0, 3);
            b.Name = $"DKeyBit{bitIndex}Button";
            b.Click += (obj, e) => FlipDiffusionBit(bitIndex);
            return b;
        }
        private void InitDiffusionFlipBitButtons()
        {
            for (int i = 0; i < (keyIs32Byte ? 256 : 128); ++i)
            {
                Button b = CreateDiffusionButton(i);
                Grid.SetRow(b, i / 64);
                Grid.SetColumn(b, i % 64);
                diffusionGrid.Children.Add(b);
            }
        }
        private void AddBoldToDescription(string descToAdd)
        {
            pres.Nav.AddBold(pres.UIStateMatrixStepDescription, descToAdd);
        }
        private void ClearDescription()
        {
            pres.Nav.Clear(pres.UIStateMatrixStepDescription);
        }
        private void UnboldLastFromDescription()
        {
            pres.Nav.UnboldLast(pres.UIStateMatrixStepDescription);
        }
        private void MakeLastBoldInDescription()
        {
            pres.Nav.MakeBoldLast(pres.UIStateMatrixStepDescription);
        }
        private void RemoveLastFromDescription()
        {
            pres.Nav.RemoveLast(pres.UIStateMatrixStepDescription);
        }
        private void ReplaceTransformInput(string input)
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
        private void ClearTransformInput()
        {
            pres.Nav.Clear(pres.UITransformInput, pres.UITransformInput2);
        }
        private void ReplaceTransformChunk(params string[] chunk)
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
        private void ClearTransformChunk()
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
        private void ReplaceTransformLittleEndian(params string[] le)
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
        private void ClearTransformLittleEndian()
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
        private void ReplaceTransformChunkCounter()
        {
            if (versionIsDJB)
            {
                ReplaceTransformChunk(pres.InitialCounterChunks[0], pres.InitialCounterChunks[1]);
            }
            else
            {
                ReplaceTransformChunk(pres.InitialCounterChunks[0]);
            }
        }
        private void ReplaceTransformLittleEndianCounter()
        {
            if (versionIsDJB)
            {
                ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0], pres.InitialCounterLittleEndian[1]);
            }
            else
            {
                ReplaceTransformLittleEndian(pres.InitialCounterLittleEndian[0]);
            }
        }
        private void ReplaceTransformChunkIV()
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
        private void ReplaceTransformLittleEndianIV()
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
        private PageAction[] InputAction(Border copyFrom, string text = null)
        {
            if (text == null)
            {
                text = ((TextBox)copyFrom.Child).Text;
            }
            PageAction[] copyActions = pres.Nav.CopyActions(new Border[] { copyFrom }, new Border[] { pres.UITransformInputCell }, new string[] { "" });
            if (text.Length > 32)
            {
                copyActions[1].AddToExec(() => {
                    pres.Nav.SetCopyBackground(pres.UITransformInputCell2);
                    ReplaceTransformInput(text);
                });
                copyActions[1].AddToUndo(() =>
                {
                    pres.Nav.UnsetBackground(pres.UITransformInputCell2);
                    ClearTransformInput();
                });
                copyActions[2].AddToExec(() => { pres.Nav.UnsetBackground(pres.UITransformInputCell2); });
                copyActions[2].AddToUndo(() => { pres.Nav.SetCopyBackground(pres.UITransformInputCell2); });
            }
            return copyActions;
        }
        private PageAction[] KeyInputAction()
        {
            Border inputKeyCell = pres.UIInputKeyCell;
            string text = pres.UIInputKey.Text;
            PageAction[] copyActions = InputAction(inputKeyCell, text);
            if (pres.InputKey.Length == 16)
            {
                copyActions[1].AddToExec(() =>
                {
                    pres.Nav.SetCopyBackground(pres.UITransformInputCell2);
                    ReplaceTransformInput($"{text}{text}");
                });
                copyActions[1].AddToUndo(() =>
                {
                    pres.Nav.UnsetBackground(pres.UITransformInputCell2);
                    ClearTransformInput();
                });
                copyActions[2].AddToExec(() => { pres.Nav.UnsetBackground(pres.UITransformInputCell2); });
                copyActions[2].AddToUndo(() => { pres.Nav.SetCopyBackground(pres.UITransformInputCell2); });
            }
            return copyActions;
        }

        private void AddConstantsActions()
        {
            PageAction constantsStepDescriptionAction = new PageAction(() =>
            {
                AddBoldToDescription(descriptions[0]);
            }, () =>
            {
                ClearDescription();
            });
            PageAction[] constantsInputAction = InputAction(pres.UIConstantsCell);
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
            AddAction(constantsStepDescriptionAction);
            AddAction(constantsInputAction);
            AddAction(constantsChunksAction);
            AddAction(constantsLittleEndianAction);
            AddAction(copyConstantsToStateActions);
        }
        private void AddKeyActions()
        {
            PageAction keyStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[1]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexConstants);
                ReplaceTransformChunk(pres.ConstantsChunks[0], pres.ConstantsChunks[1], pres.ConstantsChunks[2], pres.ConstantsChunks[3]);
                ReplaceTransformLittleEndian(pres.ConstantsLittleEndian[0], pres.ConstantsLittleEndian[1], pres.ConstantsLittleEndian[2], pres.ConstantsLittleEndian[3]);
            });
            PageAction[] keyInputAction = KeyInputAction();
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
            AddAction(keyStepDescriptionAction);
            AddAction(keyInputAction);
            AddAction(keyChunksAction);
            AddAction(keyLittleEndianAction);
            AddAction(copyKeyToStateActions);
        }
        private void AddCounterActions()
        {
            PageAction counterStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[2]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInputKey);
                ReplaceTransformChunk(
                    pres.KeyChunks[0], pres.KeyChunks[1], pres.KeyChunks[2], pres.KeyChunks[3],
                    pres.KeyChunks[keyIs32Byte ? 4 : 0], pres.KeyChunks[keyIs32Byte ? 5 : 1], pres.KeyChunks[keyIs32Byte ? 6 : 2], pres.KeyChunks[keyIs32Byte ? 7 : 3]);
                ReplaceTransformLittleEndian(
                    pres.KeyLittleEndian[0], pres.KeyLittleEndian[1], pres.KeyLittleEndian[2], pres.KeyLittleEndian[3],
                    pres.KeyLittleEndian[keyIs32Byte ? 4 : 0], pres.KeyLittleEndian[keyIs32Byte ? 5 : 1], pres.KeyLittleEndian[keyIs32Byte ? 6 : 2], pres.KeyLittleEndian[keyIs32Byte ? 7 : 3]);
            });
            PageAction counterInputAction = new PageAction(() =>
            {
                ReplaceTransformInput(pres.HexInitialCounter);
            }, () =>
            {
                ClearTransformInput();
            });
                
            PageAction counterChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkCounter();
            }, () =>
            {
                ClearTransformChunk();
            });
                
            PageAction counterLittleEndianAction = new PageAction(() =>
            {
                ReplaceTransformLittleEndianCounter();
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
            AddAction(counterStepDescriptionAction);
            AddAction(counterInputAction);
            AddAction(counterChunksAction);
            AddAction(counterLittleEndianAction);
            AddAction(copyCounterToStateActions);
        }
        private void AddIVActions()
        {
            PageAction ivStepDescriptionAction = new PageAction(() =>
            {
                UnboldLastFromDescription();
                AddBoldToDescription(descriptions[3]);
                ClearTransformInput();
                ClearTransformChunk();
                ClearTransformLittleEndian();
            }, () =>
            {
                RemoveLastFromDescription();
                MakeLastBoldInDescription();
                ReplaceTransformInput(pres.HexInitialCounter);
                ReplaceTransformChunkCounter();
                ReplaceTransformLittleEndianCounter();
            });
            PageAction[] ivInputAction = InputAction(pres.UIInputIVCell);
            PageAction ivChunksAction = new PageAction(() =>
            {
                ReplaceTransformChunkIV();
            }, () =>
            {
                ClearTransformChunk();
            });
                
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
            AddAction(ivStepDescriptionAction);
            AddAction(ivInputAction);
            AddAction(ivChunksAction);
            AddAction(ivLittleEndianAction);
            AddAction(copyIVToStateActions);
        }
    }

    partial class Page
    {
        public static StateMatrixPage StateMatrixPage(ChaChaPresentation pres)
        {
            // using static function as factory to hide the name assigned to the KeystreamBlockGenPage ContentControl element in the XAML code
            return new StateMatrixPage(pres.UIStateMatrixPage, pres);
        }
    }
}