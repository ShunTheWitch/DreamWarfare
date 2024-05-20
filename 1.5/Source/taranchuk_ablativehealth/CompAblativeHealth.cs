using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace taranchuk_ablativehealth
{
    public class CompProperties_AblativeHealth : CompProperties
    {
        public float maxHealth;

        public Color hostile = PawnNameColorUtility.ColorBaseHostile;

        public Color nonHostile = PawnNameColorUtility.ColorBaseNeutral;

        public Vector2 barSize = new Vector2(75f, 15f);

        public Vector2 offset;

        public float margin = 3f;

        public bool showBarSelectedOnly;

        public bool showBar;

        public Dictionary<DamageDef, float> damageMultipliers;

        public CompProperties_AblativeHealth()
        {
            this.compClass = typeof(CompAblativeHealth);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    public class CompAblativeHealth : ThingComp
    {
        public float curHealth;
        public CompProperties_AblativeHealth Props => base.props as CompProperties_AblativeHealth;
        public override void PostPostMake()
        {
            base.PostPostMake();
            curHealth = Props.maxHealth;
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = true;
            if (Props.damageMultipliers != null && Props.damageMultipliers.TryGetValue(dinfo.Def, out var mult)) 
            {
                dinfo.SetAmount(mult);
            }
            curHealth = Mathf.Max(0, curHealth - dinfo.Amount);
            if (curHealth <=  0)
            {
                parent.Kill(null);
            }
        }

        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();
            if (Props.showBar)
            {
                if (Props.showBarSelectedOnly && Find.Selector.SelectedObjects.Contains(parent) is false)
                {
                    return;
                }
                var pos = parent.DrawPos.MapToUIPosition();
                var healthBar = new Rect(pos.x + Props.offset.x, pos.y + Props.offset.y, Props.barSize.x, Props.barSize.y);
                DrawBar(healthBar, curHealth / Props.maxHealth, GetBarTexture());
            }
        }

        public void DrawBar(Rect rect, float fillPercent, Texture2D fillTex)
        {
            GUI.DrawTexture(rect, BaseContent.BlackTex);
            rect = rect.ContractedBy(Props.margin);
            GUI.DrawTexture(rect, BaseContent.BlackTex);
            rect.width *= fillPercent;
            GUI.DrawTexture(rect, fillTex);
        }

        public static Dictionary<Color, Texture2D> bars = new Dictionary<Color, Texture2D>();

        public Texture2D GetBarTexture()
        {
            if (parent.HostileTo(Faction.OfPlayer))
            {
                if (!bars.TryGetValue(Props.hostile, out var texture))
                {
                    bars[Props.hostile] = texture = SolidColorMaterials.NewSolidColorTexture(Props.hostile);
                }
                return texture;
            }
            else
            {
                if (!bars.TryGetValue(Props.nonHostile, out var texture))
                {
                    bars[Props.nonHostile] = texture = SolidColorMaterials.NewSolidColorTexture(Props.nonHostile);
                }
                return texture;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref curHealth, "curHealth");
        }
    }
}
