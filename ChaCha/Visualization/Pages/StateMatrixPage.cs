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
        private int Bit(byte b, int bitIndex)
        {
            return (b & (1 << bitIndex)) != 0 ? 1 : 0;
        }
        private int Bit(byte[] bytes, int bitIndex)
        {
            Debug.Assert(0 <= bitIndex, $"bitindex ({bitIndex}) was lower than zero.");
            Debug.Assert(bitIndex < 256, $"bitIndex ({bitIndex}) was higher than 255.");
            int byteIndex = bitIndex / 8;
            int bitIndexInByte = 7 - (bitIndex % 8);
            return Bit(bytes[byteIndex], bitIndexInByte);
        }
        #region KeyBits
        // KeyBits for binding to the diffusion buttons.
        // Generated with following python script.
        // The zero-th bit is the most significant bit since it belongs to the byte the most to the left in the visualization.
        // I have chosen it this way since the byte array is in the same order as seen in the visualization.
        // (The first byte of InputKey is the first byte seen in the visualization thus the most significant byte.)
        /*
        ```python
        for i in range(256):print("public int KeyBit{}".format(i))
        print("{")
        print("    get")
        print("    {")
        print("        return Bit(InputKey, {});".format(i))
        print("    }")
        print("}")
        ```
        */
        // Can this already be considered meta-programming?
        public int KeyBit0
        {
            get
            {
                return Bit(InputKey, 0);
            }
        }
        public int KeyBit1
        {
            get
            {
                return Bit(InputKey, 1);
            }
        }
        public int KeyBit2
        {
            get
            {
                return Bit(InputKey, 2);
            }
        }
        public int KeyBit3
        {
            get
            {
                return Bit(InputKey, 3);
            }
        }
        public int KeyBit4
        {
            get
            {
                return Bit(InputKey, 4);
            }
        }
        public int KeyBit5
        {
            get
            {
                return Bit(InputKey, 5);
            }
        }
        public int KeyBit6
        {
            get
            {
                return Bit(InputKey, 6);
            }
        }
        public int KeyBit7
        {
            get
            {
                return Bit(InputKey, 7);
            }
        }
        public int KeyBit8
        {
            get
            {
                return Bit(InputKey, 8);
            }
        }
        public int KeyBit9
        {
            get
            {
                return Bit(InputKey, 9);
            }
        }
        public int KeyBit10
        {
            get
            {
                return Bit(InputKey, 10);
            }
        }
        public int KeyBit11
        {
            get
            {
                return Bit(InputKey, 11);
            }
        }
        public int KeyBit12
        {
            get
            {
                return Bit(InputKey, 12);
            }
        }
        public int KeyBit13
        {
            get
            {
                return Bit(InputKey, 13);
            }
        }
        public int KeyBit14
        {
            get
            {
                return Bit(InputKey, 14);
            }
        }
        public int KeyBit15
        {
            get
            {
                return Bit(InputKey, 15);
            }
        }
        public int KeyBit16
        {
            get
            {
                return Bit(InputKey, 16);
            }
        }
        public int KeyBit17
        {
            get
            {
                return Bit(InputKey, 17);
            }
        }
        public int KeyBit18
        {
            get
            {
                return Bit(InputKey, 18);
            }
        }
        public int KeyBit19
        {
            get
            {
                return Bit(InputKey, 19);
            }
        }
        public int KeyBit20
        {
            get
            {
                return Bit(InputKey, 20);
            }
        }
        public int KeyBit21
        {
            get
            {
                return Bit(InputKey, 21);
            }
        }
        public int KeyBit22
        {
            get
            {
                return Bit(InputKey, 22);
            }
        }
        public int KeyBit23
        {
            get
            {
                return Bit(InputKey, 23);
            }
        }
        public int KeyBit24
        {
            get
            {
                return Bit(InputKey, 24);
            }
        }
        public int KeyBit25
        {
            get
            {
                return Bit(InputKey, 25);
            }
        }
        public int KeyBit26
        {
            get
            {
                return Bit(InputKey, 26);
            }
        }
        public int KeyBit27
        {
            get
            {
                return Bit(InputKey, 27);
            }
        }
        public int KeyBit28
        {
            get
            {
                return Bit(InputKey, 28);
            }
        }
        public int KeyBit29
        {
            get
            {
                return Bit(InputKey, 29);
            }
        }
        public int KeyBit30
        {
            get
            {
                return Bit(InputKey, 30);
            }
        }
        public int KeyBit31
        {
            get
            {
                return Bit(InputKey, 31);
            }
        }
        public int KeyBit32
        {
            get
            {
                return Bit(InputKey, 32);
            }
        }
        public int KeyBit33
        {
            get
            {
                return Bit(InputKey, 33);
            }
        }
        public int KeyBit34
        {
            get
            {
                return Bit(InputKey, 34);
            }
        }
        public int KeyBit35
        {
            get
            {
                return Bit(InputKey, 35);
            }
        }
        public int KeyBit36
        {
            get
            {
                return Bit(InputKey, 36);
            }
        }
        public int KeyBit37
        {
            get
            {
                return Bit(InputKey, 37);
            }
        }
        public int KeyBit38
        {
            get
            {
                return Bit(InputKey, 38);
            }
        }
        public int KeyBit39
        {
            get
            {
                return Bit(InputKey, 39);
            }
        }
        public int KeyBit40
        {
            get
            {
                return Bit(InputKey, 40);
            }
        }
        public int KeyBit41
        {
            get
            {
                return Bit(InputKey, 41);
            }
        }
        public int KeyBit42
        {
            get
            {
                return Bit(InputKey, 42);
            }
        }
        public int KeyBit43
        {
            get
            {
                return Bit(InputKey, 43);
            }
        }
        public int KeyBit44
        {
            get
            {
                return Bit(InputKey, 44);
            }
        }
        public int KeyBit45
        {
            get
            {
                return Bit(InputKey, 45);
            }
        }
        public int KeyBit46
        {
            get
            {
                return Bit(InputKey, 46);
            }
        }
        public int KeyBit47
        {
            get
            {
                return Bit(InputKey, 47);
            }
        }
        public int KeyBit48
        {
            get
            {
                return Bit(InputKey, 48);
            }
        }
        public int KeyBit49
        {
            get
            {
                return Bit(InputKey, 49);
            }
        }
        public int KeyBit50
        {
            get
            {
                return Bit(InputKey, 50);
            }
        }
        public int KeyBit51
        {
            get
            {
                return Bit(InputKey, 51);
            }
        }
        public int KeyBit52
        {
            get
            {
                return Bit(InputKey, 52);
            }
        }
        public int KeyBit53
        {
            get
            {
                return Bit(InputKey, 53);
            }
        }
        public int KeyBit54
        {
            get
            {
                return Bit(InputKey, 54);
            }
        }
        public int KeyBit55
        {
            get
            {
                return Bit(InputKey, 55);
            }
        }
        public int KeyBit56
        {
            get
            {
                return Bit(InputKey, 56);
            }
        }
        public int KeyBit57
        {
            get
            {
                return Bit(InputKey, 57);
            }
        }
        public int KeyBit58
        {
            get
            {
                return Bit(InputKey, 58);
            }
        }
        public int KeyBit59
        {
            get
            {
                return Bit(InputKey, 59);
            }
        }
        public int KeyBit60
        {
            get
            {
                return Bit(InputKey, 60);
            }
        }
        public int KeyBit61
        {
            get
            {
                return Bit(InputKey, 61);
            }
        }
        public int KeyBit62
        {
            get
            {
                return Bit(InputKey, 62);
            }
        }
        public int KeyBit63
        {
            get
            {
                return Bit(InputKey, 63);
            }
        }
        public int KeyBit64
        {
            get
            {
                return Bit(InputKey, 64);
            }
        }
        public int KeyBit65
        {
            get
            {
                return Bit(InputKey, 65);
            }
        }
        public int KeyBit66
        {
            get
            {
                return Bit(InputKey, 66);
            }
        }
        public int KeyBit67
        {
            get
            {
                return Bit(InputKey, 67);
            }
        }
        public int KeyBit68
        {
            get
            {
                return Bit(InputKey, 68);
            }
        }
        public int KeyBit69
        {
            get
            {
                return Bit(InputKey, 69);
            }
        }
        public int KeyBit70
        {
            get
            {
                return Bit(InputKey, 70);
            }
        }
        public int KeyBit71
        {
            get
            {
                return Bit(InputKey, 71);
            }
        }
        public int KeyBit72
        {
            get
            {
                return Bit(InputKey, 72);
            }
        }
        public int KeyBit73
        {
            get
            {
                return Bit(InputKey, 73);
            }
        }
        public int KeyBit74
        {
            get
            {
                return Bit(InputKey, 74);
            }
        }
        public int KeyBit75
        {
            get
            {
                return Bit(InputKey, 75);
            }
        }
        public int KeyBit76
        {
            get
            {
                return Bit(InputKey, 76);
            }
        }
        public int KeyBit77
        {
            get
            {
                return Bit(InputKey, 77);
            }
        }
        public int KeyBit78
        {
            get
            {
                return Bit(InputKey, 78);
            }
        }
        public int KeyBit79
        {
            get
            {
                return Bit(InputKey, 79);
            }
        }
        public int KeyBit80
        {
            get
            {
                return Bit(InputKey, 80);
            }
        }
        public int KeyBit81
        {
            get
            {
                return Bit(InputKey, 81);
            }
        }
        public int KeyBit82
        {
            get
            {
                return Bit(InputKey, 82);
            }
        }
        public int KeyBit83
        {
            get
            {
                return Bit(InputKey, 83);
            }
        }
        public int KeyBit84
        {
            get
            {
                return Bit(InputKey, 84);
            }
        }
        public int KeyBit85
        {
            get
            {
                return Bit(InputKey, 85);
            }
        }
        public int KeyBit86
        {
            get
            {
                return Bit(InputKey, 86);
            }
        }
        public int KeyBit87
        {
            get
            {
                return Bit(InputKey, 87);
            }
        }
        public int KeyBit88
        {
            get
            {
                return Bit(InputKey, 88);
            }
        }
        public int KeyBit89
        {
            get
            {
                return Bit(InputKey, 89);
            }
        }
        public int KeyBit90
        {
            get
            {
                return Bit(InputKey, 90);
            }
        }
        public int KeyBit91
        {
            get
            {
                return Bit(InputKey, 91);
            }
        }
        public int KeyBit92
        {
            get
            {
                return Bit(InputKey, 92);
            }
        }
        public int KeyBit93
        {
            get
            {
                return Bit(InputKey, 93);
            }
        }
        public int KeyBit94
        {
            get
            {
                return Bit(InputKey, 94);
            }
        }
        public int KeyBit95
        {
            get
            {
                return Bit(InputKey, 95);
            }
        }
        public int KeyBit96
        {
            get
            {
                return Bit(InputKey, 96);
            }
        }
        public int KeyBit97
        {
            get
            {
                return Bit(InputKey, 97);
            }
        }
        public int KeyBit98
        {
            get
            {
                return Bit(InputKey, 98);
            }
        }
        public int KeyBit99
        {
            get
            {
                return Bit(InputKey, 99);
            }
        }
        public int KeyBit100
        {
            get
            {
                return Bit(InputKey, 100);
            }
        }
        public int KeyBit101
        {
            get
            {
                return Bit(InputKey, 101);
            }
        }
        public int KeyBit102
        {
            get
            {
                return Bit(InputKey, 102);
            }
        }
        public int KeyBit103
        {
            get
            {
                return Bit(InputKey, 103);
            }
        }
        public int KeyBit104
        {
            get
            {
                return Bit(InputKey, 104);
            }
        }
        public int KeyBit105
        {
            get
            {
                return Bit(InputKey, 105);
            }
        }
        public int KeyBit106
        {
            get
            {
                return Bit(InputKey, 106);
            }
        }
        public int KeyBit107
        {
            get
            {
                return Bit(InputKey, 107);
            }
        }
        public int KeyBit108
        {
            get
            {
                return Bit(InputKey, 108);
            }
        }
        public int KeyBit109
        {
            get
            {
                return Bit(InputKey, 109);
            }
        }
        public int KeyBit110
        {
            get
            {
                return Bit(InputKey, 110);
            }
        }
        public int KeyBit111
        {
            get
            {
                return Bit(InputKey, 111);
            }
        }
        public int KeyBit112
        {
            get
            {
                return Bit(InputKey, 112);
            }
        }
        public int KeyBit113
        {
            get
            {
                return Bit(InputKey, 113);
            }
        }
        public int KeyBit114
        {
            get
            {
                return Bit(InputKey, 114);
            }
        }
        public int KeyBit115
        {
            get
            {
                return Bit(InputKey, 115);
            }
        }
        public int KeyBit116
        {
            get
            {
                return Bit(InputKey, 116);
            }
        }
        public int KeyBit117
        {
            get
            {
                return Bit(InputKey, 117);
            }
        }
        public int KeyBit118
        {
            get
            {
                return Bit(InputKey, 118);
            }
        }
        public int KeyBit119
        {
            get
            {
                return Bit(InputKey, 119);
            }
        }
        public int KeyBit120
        {
            get
            {
                return Bit(InputKey, 120);
            }
        }
        public int KeyBit121
        {
            get
            {
                return Bit(InputKey, 121);
            }
        }
        public int KeyBit122
        {
            get
            {
                return Bit(InputKey, 122);
            }
        }
        public int KeyBit123
        {
            get
            {
                return Bit(InputKey, 123);
            }
        }
        public int KeyBit124
        {
            get
            {
                return Bit(InputKey, 124);
            }
        }
        public int KeyBit125
        {
            get
            {
                return Bit(InputKey, 125);
            }
        }
        public int KeyBit126
        {
            get
            {
                return Bit(InputKey, 126);
            }
        }
        public int KeyBit127
        {
            get
            {
                return Bit(InputKey, 127);
            }
        }
        public int KeyBit128
        {
            get
            {
                return Bit(InputKey, 128);
            }
        }
        public int KeyBit129
        {
            get
            {
                return Bit(InputKey, 129);
            }
        }
        public int KeyBit130
        {
            get
            {
                return Bit(InputKey, 130);
            }
        }
        public int KeyBit131
        {
            get
            {
                return Bit(InputKey, 131);
            }
        }
        public int KeyBit132
        {
            get
            {
                return Bit(InputKey, 132);
            }
        }
        public int KeyBit133
        {
            get
            {
                return Bit(InputKey, 133);
            }
        }
        public int KeyBit134
        {
            get
            {
                return Bit(InputKey, 134);
            }
        }
        public int KeyBit135
        {
            get
            {
                return Bit(InputKey, 135);
            }
        }
        public int KeyBit136
        {
            get
            {
                return Bit(InputKey, 136);
            }
        }
        public int KeyBit137
        {
            get
            {
                return Bit(InputKey, 137);
            }
        }
        public int KeyBit138
        {
            get
            {
                return Bit(InputKey, 138);
            }
        }
        public int KeyBit139
        {
            get
            {
                return Bit(InputKey, 139);
            }
        }
        public int KeyBit140
        {
            get
            {
                return Bit(InputKey, 140);
            }
        }
        public int KeyBit141
        {
            get
            {
                return Bit(InputKey, 141);
            }
        }
        public int KeyBit142
        {
            get
            {
                return Bit(InputKey, 142);
            }
        }
        public int KeyBit143
        {
            get
            {
                return Bit(InputKey, 143);
            }
        }
        public int KeyBit144
        {
            get
            {
                return Bit(InputKey, 144);
            }
        }
        public int KeyBit145
        {
            get
            {
                return Bit(InputKey, 145);
            }
        }
        public int KeyBit146
        {
            get
            {
                return Bit(InputKey, 146);
            }
        }
        public int KeyBit147
        {
            get
            {
                return Bit(InputKey, 147);
            }
        }
        public int KeyBit148
        {
            get
            {
                return Bit(InputKey, 148);
            }
        }
        public int KeyBit149
        {
            get
            {
                return Bit(InputKey, 149);
            }
        }
        public int KeyBit150
        {
            get
            {
                return Bit(InputKey, 150);
            }
        }
        public int KeyBit151
        {
            get
            {
                return Bit(InputKey, 151);
            }
        }
        public int KeyBit152
        {
            get
            {
                return Bit(InputKey, 152);
            }
        }
        public int KeyBit153
        {
            get
            {
                return Bit(InputKey, 153);
            }
        }
        public int KeyBit154
        {
            get
            {
                return Bit(InputKey, 154);
            }
        }
        public int KeyBit155
        {
            get
            {
                return Bit(InputKey, 155);
            }
        }
        public int KeyBit156
        {
            get
            {
                return Bit(InputKey, 156);
            }
        }
        public int KeyBit157
        {
            get
            {
                return Bit(InputKey, 157);
            }
        }
        public int KeyBit158
        {
            get
            {
                return Bit(InputKey, 158);
            }
        }
        public int KeyBit159
        {
            get
            {
                return Bit(InputKey, 159);
            }
        }
        public int KeyBit160
        {
            get
            {
                return Bit(InputKey, 160);
            }
        }
        public int KeyBit161
        {
            get
            {
                return Bit(InputKey, 161);
            }
        }
        public int KeyBit162
        {
            get
            {
                return Bit(InputKey, 162);
            }
        }
        public int KeyBit163
        {
            get
            {
                return Bit(InputKey, 163);
            }
        }
        public int KeyBit164
        {
            get
            {
                return Bit(InputKey, 164);
            }
        }
        public int KeyBit165
        {
            get
            {
                return Bit(InputKey, 165);
            }
        }
        public int KeyBit166
        {
            get
            {
                return Bit(InputKey, 166);
            }
        }
        public int KeyBit167
        {
            get
            {
                return Bit(InputKey, 167);
            }
        }
        public int KeyBit168
        {
            get
            {
                return Bit(InputKey, 168);
            }
        }
        public int KeyBit169
        {
            get
            {
                return Bit(InputKey, 169);
            }
        }
        public int KeyBit170
        {
            get
            {
                return Bit(InputKey, 170);
            }
        }
        public int KeyBit171
        {
            get
            {
                return Bit(InputKey, 171);
            }
        }
        public int KeyBit172
        {
            get
            {
                return Bit(InputKey, 172);
            }
        }
        public int KeyBit173
        {
            get
            {
                return Bit(InputKey, 173);
            }
        }
        public int KeyBit174
        {
            get
            {
                return Bit(InputKey, 174);
            }
        }
        public int KeyBit175
        {
            get
            {
                return Bit(InputKey, 175);
            }
        }
        public int KeyBit176
        {
            get
            {
                return Bit(InputKey, 176);
            }
        }
        public int KeyBit177
        {
            get
            {
                return Bit(InputKey, 177);
            }
        }
        public int KeyBit178
        {
            get
            {
                return Bit(InputKey, 178);
            }
        }
        public int KeyBit179
        {
            get
            {
                return Bit(InputKey, 179);
            }
        }
        public int KeyBit180
        {
            get
            {
                return Bit(InputKey, 180);
            }
        }
        public int KeyBit181
        {
            get
            {
                return Bit(InputKey, 181);
            }
        }
        public int KeyBit182
        {
            get
            {
                return Bit(InputKey, 182);
            }
        }
        public int KeyBit183
        {
            get
            {
                return Bit(InputKey, 183);
            }
        }
        public int KeyBit184
        {
            get
            {
                return Bit(InputKey, 184);
            }
        }
        public int KeyBit185
        {
            get
            {
                return Bit(InputKey, 185);
            }
        }
        public int KeyBit186
        {
            get
            {
                return Bit(InputKey, 186);
            }
        }
        public int KeyBit187
        {
            get
            {
                return Bit(InputKey, 187);
            }
        }
        public int KeyBit188
        {
            get
            {
                return Bit(InputKey, 188);
            }
        }
        public int KeyBit189
        {
            get
            {
                return Bit(InputKey, 189);
            }
        }
        public int KeyBit190
        {
            get
            {
                return Bit(InputKey, 190);
            }
        }
        public int KeyBit191
        {
            get
            {
                return Bit(InputKey, 191);
            }
        }
        public int KeyBit192
        {
            get
            {
                return Bit(InputKey, 192);
            }
        }
        public int KeyBit193
        {
            get
            {
                return Bit(InputKey, 193);
            }
        }
        public int KeyBit194
        {
            get
            {
                return Bit(InputKey, 194);
            }
        }
        public int KeyBit195
        {
            get
            {
                return Bit(InputKey, 195);
            }
        }
        public int KeyBit196
        {
            get
            {
                return Bit(InputKey, 196);
            }
        }
        public int KeyBit197
        {
            get
            {
                return Bit(InputKey, 197);
            }
        }
        public int KeyBit198
        {
            get
            {
                return Bit(InputKey, 198);
            }
        }
        public int KeyBit199
        {
            get
            {
                return Bit(InputKey, 199);
            }
        }
        public int KeyBit200
        {
            get
            {
                return Bit(InputKey, 200);
            }
        }
        public int KeyBit201
        {
            get
            {
                return Bit(InputKey, 201);
            }
        }
        public int KeyBit202
        {
            get
            {
                return Bit(InputKey, 202);
            }
        }
        public int KeyBit203
        {
            get
            {
                return Bit(InputKey, 203);
            }
        }
        public int KeyBit204
        {
            get
            {
                return Bit(InputKey, 204);
            }
        }
        public int KeyBit205
        {
            get
            {
                return Bit(InputKey, 205);
            }
        }
        public int KeyBit206
        {
            get
            {
                return Bit(InputKey, 206);
            }
        }
        public int KeyBit207
        {
            get
            {
                return Bit(InputKey, 207);
            }
        }
        public int KeyBit208
        {
            get
            {
                return Bit(InputKey, 208);
            }
        }
        public int KeyBit209
        {
            get
            {
                return Bit(InputKey, 209);
            }
        }
        public int KeyBit210
        {
            get
            {
                return Bit(InputKey, 210);
            }
        }
        public int KeyBit211
        {
            get
            {
                return Bit(InputKey, 211);
            }
        }
        public int KeyBit212
        {
            get
            {
                return Bit(InputKey, 212);
            }
        }
        public int KeyBit213
        {
            get
            {
                return Bit(InputKey, 213);
            }
        }
        public int KeyBit214
        {
            get
            {
                return Bit(InputKey, 214);
            }
        }
        public int KeyBit215
        {
            get
            {
                return Bit(InputKey, 215);
            }
        }
        public int KeyBit216
        {
            get
            {
                return Bit(InputKey, 216);
            }
        }
        public int KeyBit217
        {
            get
            {
                return Bit(InputKey, 217);
            }
        }
        public int KeyBit218
        {
            get
            {
                return Bit(InputKey, 218);
            }
        }
        public int KeyBit219
        {
            get
            {
                return Bit(InputKey, 219);
            }
        }
        public int KeyBit220
        {
            get
            {
                return Bit(InputKey, 220);
            }
        }
        public int KeyBit221
        {
            get
            {
                return Bit(InputKey, 221);
            }
        }
        public int KeyBit222
        {
            get
            {
                return Bit(InputKey, 222);
            }
        }
        public int KeyBit223
        {
            get
            {
                return Bit(InputKey, 223);
            }
        }
        public int KeyBit224
        {
            get
            {
                return Bit(InputKey, 224);
            }
        }
        public int KeyBit225
        {
            get
            {
                return Bit(InputKey, 225);
            }
        }
        public int KeyBit226
        {
            get
            {
                return Bit(InputKey, 226);
            }
        }
        public int KeyBit227
        {
            get
            {
                return Bit(InputKey, 227);
            }
        }
        public int KeyBit228
        {
            get
            {
                return Bit(InputKey, 228);
            }
        }
        public int KeyBit229
        {
            get
            {
                return Bit(InputKey, 229);
            }
        }
        public int KeyBit230
        {
            get
            {
                return Bit(InputKey, 230);
            }
        }
        public int KeyBit231
        {
            get
            {
                return Bit(InputKey, 231);
            }
        }
        public int KeyBit232
        {
            get
            {
                return Bit(InputKey, 232);
            }
        }
        public int KeyBit233
        {
            get
            {
                return Bit(InputKey, 233);
            }
        }
        public int KeyBit234
        {
            get
            {
                return Bit(InputKey, 234);
            }
        }
        public int KeyBit235
        {
            get
            {
                return Bit(InputKey, 235);
            }
        }
        public int KeyBit236
        {
            get
            {
                return Bit(InputKey, 236);
            }
        }
        public int KeyBit237
        {
            get
            {
                return Bit(InputKey, 237);
            }
        }
        public int KeyBit238
        {
            get
            {
                return Bit(InputKey, 238);
            }
        }
        public int KeyBit239
        {
            get
            {
                return Bit(InputKey, 239);
            }
        }
        public int KeyBit240
        {
            get
            {
                return Bit(InputKey, 240);
            }
        }
        public int KeyBit241
        {
            get
            {
                return Bit(InputKey, 241);
            }
        }
        public int KeyBit242
        {
            get
            {
                return Bit(InputKey, 242);
            }
        }
        public int KeyBit243
        {
            get
            {
                return Bit(InputKey, 243);
            }
        }
        public int KeyBit244
        {
            get
            {
                return Bit(InputKey, 244);
            }
        }
        public int KeyBit245
        {
            get
            {
                return Bit(InputKey, 245);
            }
        }
        public int KeyBit246
        {
            get
            {
                return Bit(InputKey, 246);
            }
        }
        public int KeyBit247
        {
            get
            {
                return Bit(InputKey, 247);
            }
        }
        public int KeyBit248
        {
            get
            {
                return Bit(InputKey, 248);
            }
        }
        public int KeyBit249
        {
            get
            {
                return Bit(InputKey, 249);
            }
        }
        public int KeyBit250
        {
            get
            {
                return Bit(InputKey, 250);
            }
        }
        public int KeyBit251
        {
            get
            {
                return Bit(InputKey, 251);
            }
        }
        public int KeyBit252
        {
            get
            {
                return Bit(InputKey, 252);
            }
        }
        public int KeyBit253
        {
            get
            {
                return Bit(InputKey, 253);
            }
        }
        public int KeyBit254
        {
            get
            {
                return Bit(InputKey, 254);
            }
        }
        public int KeyBit255
        {
            get
            {
                return Bit(InputKey, 255);
            }
        }
        #endregion

        public int DKeyNibble(int nibbleIndex)
        {
            Debug.Assert(0 <= nibbleIndex, $"nibbleIndex ({nibbleIndex}) was lower than zero.");
            Debug.Assert(nibbleIndex < 64, $"nibbleIndex ({nibbleIndex}) was higher than 63.");
            // most significant bit
            int msb = Bit(InputKey, nibbleIndex * 4);
            int a = Bit(InputKey, (nibbleIndex * 4) + 1);
            int b = Bit(InputKey, (nibbleIndex * 4) + 2);
            // least significant bit
            int lsb = Bit(InputKey, (nibbleIndex * 4) + 3);
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
        private TextBlock diffusionKey;
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
            diffusionKey = pres.DiffusionKey;
            InitToggleDiffusionButton();
            InitDiffusionKey();
            InitDiffusionGridLayout();
            InitDiffusionFlipBitButtons();
        }
        private void ToggleDiffusion(object sender, RoutedEventArgs e)
        {
            Visibility set = diffusionGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            diffusionGrid.Visibility = set;
            diffusionKey.Visibility = set;
        }
        private void InitToggleDiffusionButton()
        {
            toggleShowDiffusion.Click += ToggleDiffusion;
        }
        private TextBlock CreateDiffusionKeyNibbleTextBox(int nibbleIndex)
        {
            TextBlock tb = new TextBlock();
            tb.SetBinding(TextBlock.TextProperty, new Binding($"DKeyNibbleHex{nibbleIndex}"));
            Trigger markNibble = new Trigger()
            {
                Property = TextBlock.IsMouseOverProperty,
                Value = true
            };
            Setter setter = new Setter() { Property = TextBlock.ForegroundProperty, Value = Brushes.Red };
            markNibble.Setters.Add(setter);
            Style s = new Style();
            s.Triggers.Add(markNibble);
            tb.Style = s;
            return tb;
        }
        private void InitDiffusionKey()
        {
            for (int i = 0; i < (keyIs32Byte ? 32 : 16) * 2; ++i)
            {
                TextBlock nibbleBox = CreateDiffusionKeyNibbleTextBox(i);
                diffusionKey.Inlines.Add(nibbleBox);
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
        private Button CreateDiffusionButton(int bitIndex)
        {
            // Bit indices start at 0 on the most significant bit which is in the string representation in big endian notation.
            // This means we start counting from zero at the left but the zero-th bit is - maybe a bit unintuitively - the most significant bit.
            Button b = new Button() { Height = 16, FontSize = 10 };
            b.SetBinding(Button.ContentProperty, new Binding($"KeyBit{bitIndex}"));
            b.Margin = new Thickness(bitIndex % 4 == 0 ? 3 : 0, 0, 0, 3);
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