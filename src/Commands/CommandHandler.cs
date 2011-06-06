#region Header

//    OpenMC, a Minecraft SMP server.
//    Copyright (C) 2011 OpenMC. All rights reserved.
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

namespace OpenMC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CommandHandler
    {
        #region Fields

        public Rank AccessRights;
        public string Command;
        public int CommandLength;
        public InventoryItem Item;
        public Player Target = null;
        public Player User;

        private short BlockID;
        private byte Count;

        #endregion Fields

        #region Constructors

        public CommandHandler(Rank accessRights, Player p)
        {
            this.AccessRights = accessRights;
            this.User = p;
        }

        #endregion Constructors

        #region Methods

        public object GetPlayerObject(string Username)
        {
            Player Target = OpenMC.Server.PlayerList.Find(delegate(Player p) { return p.Username == Username; });
                return Target;
        }

        public void Handle(string[] command)
        {
            Command = command[0];
            CommandLength = command.Length;
            try
            {

                switch (Command.ToLower())
                {
                    case "/help":
                        User.RecvServerMessage("The following commands are availible to you:");
                        User.RecvServerMessage("<> = optional :: [] = required field");
                        User.RecvServerMessage("/tp    -- teleport to a user IE: /tp [username]");
                        User.RecvServerMessage("/ban   -- ban a user; defaults to 30 mins IE: /ban [user] <time>");
                        User.RecvServerMessage("/ipban -- ban a user by ip; defaults to 30 mins IE: /ipban [ip] <time>");
                        User.RecvServerMessage("/kick  -- Disconect a user IE: /kick [username]");
                        User.RecvServerMessage("/give  -- Gives you a item IE: /item [ID] [Amount]");
                        User.RecvServerMessage("/whois -- returns player information");
                        break;
                    case "/ban":
                        if (RankInfo.IsOperator(User.AccessRights))
                        {
                            User.RecvServerMessage("Not Implemented");
                        }
                        break;
                    case "/ipban":
                        if (RankInfo.IsAdmin(User.AccessRights))
                        {
                            User.RecvServerMessage("Not Implemented");
                        }
                        break;
                    case "/kick":
                        if (RankInfo.IsJanitor(User.AccessRights))
                        {
                            if (command.Length == 2)
                            {
                                User.RecvServerMessage("Kicking " + command[1]);
                                Target = (Player)GetPlayerObject(command[1]);
                                Target.Disconnect("You have been kicked by " + User.Username);
                            }
                            else
                            {
                                User.RecvServerMessage("Wrong amount of arguments");
                            }
                        }
                        break;
                    case "/tp":
                        if (RankInfo.IsJanitor(User.AccessRights))
                        {
                            User.RecvServerMessage("Teleporting to " + command[1]);
                        }
                        break;

                    // UNDONE: Need to finish /give command
                    case "/give":
                        if (RankInfo.IsOperator(User.AccessRights))
                        {
                            if (command.Length == 2)
                            {
                                if (command[1].ToLower() == "help")
                                {
                                    User.RecvServerMessage("Showing Help for /item");
                                    User.RecvServerMessage("Usages:");
                                    User.RecvServerMessage("/item help");
                                    User.RecvServerMessage("/item [ID]");
                                    User.RecvServerMessage("/item [ID] [Count]");
                                    User.RecvServerMessage("/item [Username] [ID] [Count]");
                                }
                                else
                                {
                                    BlockID = Convert.ToInt16(command[1]);
                                    Item = new InventoryItem(BlockID);
                                    //If it is a Block, else it is a item
                                    if (Convert.ToInt32(command[1]) < 256)
                                    {
                                        //Holy shit its LISP....
                                        User.RecvServerMessage(((Block)BlockID).ToString() + " Added to inventory");
                                    }
                                    else
                                    {
                                        User.RecvServerMessage(((Item)BlockID).ToString() + " Added to inventory");
                                    }

                                    User.Inventory.AddItem(Item);

                                }
                            }
                            if (command.Length == 3)
                            {
                                BlockID = Convert.ToInt16(command[1]);
                                Count = Convert.ToByte(command[2]);
                                Item = new InventoryItem(BlockID,Count,0);
                                //If it is a Block, else it is a item
                                if (Convert.ToInt32(command[1]) < 256)
                                {
                                    //Holy shit its LISP....
                                    User.RecvServerMessage(((Block)BlockID).ToString() + "x" + Count + " Added to inventory");
                                }
                                else
                                {
                                    User.RecvServerMessage(((Item)BlockID).ToString() + "x" + Count + " Added to inventory");
                                }
                                User.Inventory.AddItem(Item);
                            }
                            if (command.Length == 4)
                            {

                            }
                        }
                        break;
                    case "/whois":
                        User.RecvServerMessage("Not Implemented");
                        break;
                    case "/whoami":
                        User.RecvServerMessage("You are " + User.Username + " who is a " + RankInfo.RankTitle(AccessRights) + Color.White + " with entity ID " + User.EntityID);
                        User.RecvServerMessage("You are connected from " + User._Conn.IPString);
                        break;
                    default:
                        User.RecvServerMessage("Type /help to see avalible commands for a Access level " +
                            AccessRights + " User");
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion Methods
    }
}