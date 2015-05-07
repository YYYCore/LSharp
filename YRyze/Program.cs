using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;



namespace YRyze
{
    class Program
    {

        public const string ChampionName = "Ryze";





        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;



        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;


        public static bool attackNow = true;
        public static double lag = 0;



        public static Menu Config;
        private static Obj_AI_Hero Player;
        public static Orbwalking.Orbwalker Orbwalker;



        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }


        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 580f);
            E = new Spell(SpellSlot.E, 580f);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 50f, 1800f, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.20f, float.MaxValue);

            Config = new Menu("YRyze", "YRyze", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));


            Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("UseQLastHit", "Use Q In LastHit").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("QMiniManaLastHit", "Min Mana To lasthit with Q").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LastHit").AddItem(new MenuItem("UseELastHit", "Use E In LastHit").SetValue(false));
            Config.SubMenu("LastHit").AddItem(new MenuItem("EMiniManaLastHit", "Minimum Mana To Use E In LastHit").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LastHit").AddItem(new MenuItem("NoPassiveProcLastHit", "Never Use Spell In LastHit When Passive Will Proc").SetValue(true));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActiveT", "Harass (toggle)").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "Use W in LaneClear").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "Use E in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("EMiniManaLaneClear", "Minimum Mana To Use E In LaneClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("NoPassiveProcLaneClear", "Never Use Spell In LaneClear When Passive Will Proc").SetValue(false));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("UseEJungleClear", "Use E In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("EMiniManaLaneClear", "Minimum Mana To Use E In LaneClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("NoPassiveProcLaneClear", "Never Use Spell In LaneClear When Passive Will Proc").SetValue(false));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseAACombo", "AA Usage In Combo").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQEGC", "Auto Q On Gapclosers").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoWEGC", "Auto W On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQPS", "Auto Q On ProcessSpell").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQKS", "Auto Q KS ").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoPotion", "Use Auto Potion").SetValue(true));


            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WERange", "W/E range").SetValue(true));

            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (Player.IsDead) return;

            if (Config.Item("AutoQKS").GetValue<bool>())
            {
                Killsteal();
            }


            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }



        }



        private static void Killsteal()
        {
            List<Obj_AI_Hero> enemies = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
            foreach (var enemy in enemies)
            {
                if (Q.IsReady() && Player.Mana >= QMANA && enemy.Health < Q.GetDamage(enemy) && Player.Distance(enemy.ServerPosition) < Q.Range && enemy.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(enemy, HitChance.High, true);
                    return;
                }
            }
        }


        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            throw new NotImplementedException();
        }


        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("AutoWGC").GetValue<bool>() && W.IsReady() && Player.Mana >= WMANA && Player.Distance(gapcloser.Sender.ServerPosition) < W.Range)
            {
                W.Cast(gapcloser.Sender, true);
            }

            if (Config.Item("AutoQGC").GetValue<bool>() && Q.IsReady() && Player.Mana >= QMANA && Player.Distance(gapcloser.Sender.ServerPosition) < Q.Range)
            {
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High, true);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {

            if (Config.Item("AutoQPS").GetValue<bool>() && sender.IsEnemy && sender.IsValid)
            {
                if (spell.SData.Name == "ThreshQ"
                    || spell.SData.Name == "KatarinaR"
                    || spell.SData.Name == "AlZaharNetherGrasp"
                    || spell.SData.Name == "GalioIdolOfDurand"
                    || spell.SData.Name == "LuxMalicaCannon"
                    || spell.SData.Name == "MissFortuneBulletTime"
                    || spell.SData.Name == "GocketGrabMissile"
                    || spell.SData.Name == "CaitlynPiltoverPeacemaker"
                    || spell.SData.Name == "EzrealTrueshotBarrage"
                    || spell.SData.Name == "InfiniteDuress"
                    || spell.SData.Name == "VelkozR")
                {
                    Q.Cast(sender.ServerPosition, true);
                }
            }


        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("QRange").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + ObjectManager.Player.BoundingRadius, System.Drawing.Color.Green, 1, 1);
                }
                else
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + ObjectManager.Player.BoundingRadius, System.Drawing.Color.Red, 1, 1);
                }
            }
            if (Config.Item("WERange").GetValue<bool>())
            {
                if (W.IsReady() || E.IsReady())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range + ObjectManager.Player.BoundingRadius, System.Drawing.Color.Green, 1, 1);
                }
                else
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range + ObjectManager.Player.BoundingRadius, System.Drawing.Color.Red, 1, 1);
                }
            }
        }



        private static void Harass()
        {
            throw new NotImplementedException();
        }

        private static void LastHit()
        {
            throw new NotImplementedException();
        }

        private static void LaneClear()
        {
            throw new NotImplementedException();
        }

        private static void JungleClear()
        {
            throw new NotImplementedException();
        }

        public static void Combo()
        {

            var useQ = Config.Item("UseQCombo").GetValue<bool>();
            var useW = Config.Item("UseWCombo").GetValue<bool>();
            var useE = Config.Item("UseECombo").GetValue<bool>();
            var useR = Config.Item("UseRCombo").GetValue<bool>();

            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, TargetSelector.DamageType.Magical);

            if (Config.Item("AA").GetValue<bool>())
            {
                Orbwalking.Attack = true;
            }
            else if ((target.IsValidTarget() && (Player.GetAutoAttackDamage(target) > target.Health))
                      || (!Q.IsReady() && !W.IsReady() && !E.IsReady())
                      || ((Player.Mana < QMANA && Player.Mana < WMANA && Player.Mana < EMANA)))
            {
                Orbwalking.Attack = true;
            }
            else
            {
                Orbwalking.Attack = false;
            }

            if (useR && Player.CountEnemiesInRange(W.Range) > 1
                && (Q.GetDamage(target) + W.GetDamage(target) + E.GetDamage(target) < target.Health)
                && GetPassiveStacks() == 4 || Player.HasBuff("RyzePassiveCharged"))
            {
                R.Cast(true);
            }



            if (useW && W.IsReady() && WMANA < Player.Mana)
            {
                W.Cast(target);
            }

            if (useQ && Q.IsReady() && QMANA < Player.Mana)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
            }


            if (useE && E.IsReady() && EMANA < Player.Mana)
            {
                E.Cast(target);
            }

            if (useQ && Q.IsReady() && QMANA < Player.Mana)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
            }




        }

        public static int GetPassiveStacks()
        {
            var stacks = ObjectManager.Player.Buffs.FirstOrDefault(name => name.DisplayName == "RyzePassiveStack");
            if (stacks.Count > 0)
            {
                return stacks.Count;
            }
            else
            {
                return 0;
            }
        }


    }
}
