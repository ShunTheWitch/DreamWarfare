﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;

namespace taranchuk_ablativehealth
{
    public class taranchuk_ablativehealthMod : Mod
    {
        public taranchuk_ablativehealthMod(ModContentPack pack) : base(pack)
        {
            new Harmony("taranchuk_ablativehealthMod").PatchAll();
        }
    }


}
