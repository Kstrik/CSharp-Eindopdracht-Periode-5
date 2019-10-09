using Battleship_Server.Net;
using Networking.Battleship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Server.GameLogic
{
    public class Game
    {
        public enum GameState
        {
            SETUP,
            GAME
        }

        private GameState state;

        private GamePlayer[] players;
        private int currentTurnIndex;

        private Session session;
        private BattleshipServer battleshipServer;

        public Game(GamePlayer player1, GamePlayer player2, Session session, BattleshipServer battleshipServer)
        {
            this.state = GameState.SETUP;
            this.players = new GamePlayer[2] { player1, player2 };
            this.currentTurnIndex = 0;
            this.session = session;
            this.battleshipServer = battleshipServer;
        }

        private bool SubmitMove(int indexX, int indexY)
        {
            GamePlayer gamePlayer = this.players[this.currentTurnIndex];
            GamePlayer enemyPlayer = this.players[(this.currentTurnIndex == 0) ? 1 : 0];

            if(enemyPlayer.GetGrid().EvaluateMove(indexX, indexY))
            {
                if(enemyPlayer.GetGrid().ExecuteMove(indexX, indexY))
                {
                    this.session.Broadcast(new Message(Message.ID.SUBMIT_MOVE, Message.State.OK, new byte[3] { (byte)indexX, (byte)indexY, 1 }));
                    enemyPlayer.HitPoints--;

                    if(enemyPlayer.HitPoints == 0)
                    {
                        this.session.Broadcast(new Message(Message.ID.END_GAME, Message.State.OK, Encoding.UTF8.GetBytes(gamePlayer.Player.GetUsername())));
                        return false;
                    }
                }
                else
                {
                    this.currentTurnIndex = (this.currentTurnIndex == 0) ? 1 : 0;
                    this.session.Broadcast(new Message(Message.ID.SUBMIT_MOVE, Message.State.OK, new byte[3] { (byte)indexX, (byte)indexY, 0 }));
                }
            }
            else
                this.battleshipServer.Transmit(new Message(Message.ID.SUBMIT_MOVE, Message.State.ERROR, Encoding.UTF8.GetBytes("Move is already done!")), gamePlayer.Player.GetConnection());

            return true;
        }

        public bool HandleMessage(Message message, Player player)
        {
            List<byte> content = new List<byte>(message.GetContent());

            switch (message.GetId())
            {
                case Message.ID.SUBMIT_BOATS:
                    {
                        if(player.IsAuthorized && this.state == GameState.SETUP)
                        {

                        }
                        break;
                    }
                case Message.ID.SUBMIT_MOVE:
                    {
                        if (player.IsAuthorized && this.players[this.currentTurnIndex].Player == player && this.state == GameState.GAME)
                            return SubmitMove(content[0], content[1]);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return true;
        }

        public GameState GetState()
        {
            return this.state;
        }
    }
}
