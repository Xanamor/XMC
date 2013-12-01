#region Header

//    XMC, a Minecraft SMP server.
//    Copyright (C) 2011 XMC. All rights reserved.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion Header

namespace XMC
{
    using System;
    using System.Collections.Generic;

    #region Enumerations

    public enum Rank
    {
        Banned = -1,
        Guest,
        Janitor,
        Operator,
        Admin
    }

    #endregion Enumerations

    public static class RankInfo
    {
        #region Fields

        private static Dictionary<Rank, string> colors;

        #endregion Fields

        #region Constructors

        static RankInfo()
        {
            colors = new Dictionary<Rank, string>();
            colors[Rank.Banned] = Color.Red;
            colors[Rank.Guest] = Color.White;
            colors[Rank.Janitor] = Color.Green;
            colors[Rank.Operator] = Color.Yellow;
            colors[Rank.Admin] = Color.Blue;
        }

        #endregion Constructors

        #region Methods

        public static bool IsAdmin(Rank rank)
        {
            return (rank == Rank.Admin);
        }

        public static bool IsJanitor(Rank rank)
        {
            return (rank >= Rank.Janitor);
        }

        public static bool IsOperator(Rank rank)
        {
            return (rank >= Rank.Operator);
        }

        public static string RankColor(Rank rank)
        {
            if (colors.ContainsKey(rank)) {
                return colors[rank];
            } else {
                return Color.Black;
            }
        }

        public static string RankTitle(Rank rank)
        {
            int TitleLevel = (int)rank;

            if (TitleLevel == -1)
                return Color.Red + "<Banned>";
            if (TitleLevel == 0)
                return Color.White + "<Guest>";
            if (TitleLevel == 1)
                return  Color.Green + "<Janitor>";
            if (TitleLevel == 2)
                return Color.Yellow + "<Operator>";
            if (TitleLevel == 3)
                return Color.Blue + "<Administrator>";
            return "#RANK_NOT_FOUND#";
        }

        #endregion Methods
    }
}