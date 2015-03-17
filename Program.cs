﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;





namespace YTurnAround
{
    
    
    public class Champ{
        public string name {get; set;}
        public string skill {get; set;}
        public string dspname {get; set;}
        public int range {get; set;}
        public int delay {get; set;}
        public int direction {get; set;}

    }


    class Turn
    {

        public static Orbwalking.Orbwalker Orbwalker;

        public static Menu config;
        private static Obj_AI_Hero player;
        public static Spell TryndW, ShacoE, CassR;
        public static Champ Tryndamere, Shaco, Cassiopeia;
        public static Vector3 pos;
        public int direct = 0;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }




        static void Game_OnGameLoad(EventArgs args)
        {

            player = ObjectManager.Player;
            config = new Menu("YTurnAround", "YTurnAround", true);
            config.AddItem(new MenuItem(Tryndamere.dspname, Tryndamere.dspname)).SetValue(true);
            config.AddItem(new MenuItem(Shaco.dspname, Shaco.dspname)).SetValue(true);
            config.AddItem(new MenuItem(Cassiopeia.dspname, Cassiopeia.dspname)).SetValue(true);


            Tryndamere = new Champ
            {
                name = "Tryndamere",
                skill = "MockingShout",
                dspname = "Trynda W",
                range = 850,
                delay = 10,
                direction = 1,
            };

            Shaco = new Champ
            {
                name = "Shaco",
                skill = "TwoShivPoison",
                dspname = "Shaco E",
                range = 625,
                delay = 100,
                direction = 1,
            };

            Cassiopeia = new Champ
            {
                name = "Cassiopeia",
                skill = "PetrifyingGaze",
                dspname = "Cassio R",
                range = 750,
                delay = 50,
                direction = 0,
            };



            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }


        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {

            if ((spell.SData.Name == Tryndamere.skill 
               || spell.SData.Name == Shaco.skill 
               || spell.SData.Name == Cassiopeia.skill)
               & (spell.Target == player))
            {
                int direct = 0;
                if (sender.BaseSkinName == "Tryndamere")
                {
                    int delay = Tryndamere.delay;
                    direct = Tryndamere.direction;
                } if (sender.BaseSkinName == "Shaco")
                {
                    int delay = Shaco.delay;
                    direct = Shaco.direction;
                } if (sender.BaseSkinName == "Cassiopeia")
                {
                    int delay = Cassiopeia.delay;
                    direct = Cassiopeia.direction;
                }



                pos = new Vector3(player.Position.X + (direct * sender.Position.X) / 4
                                 , player.Position.Y + (direct * sender.Position.Y) / 4
                                 , 0);

                player.IssueOrder(GameObjectOrder.MoveTo, pos);




            }


            
        }
    }
}