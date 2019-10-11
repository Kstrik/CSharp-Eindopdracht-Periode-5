using Battleship_Server.Net;
using Networking.Battleship;
using Networking.Battleship.GameLogic;
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

        private void SubmitBoats(List<(int indexX, int indexY, int direction)> boatsData, Player player)
        {
            GamePlayer gamePlayer = this.players.Where(p => p.Player == player).First();
            if(gamePlayer.HasPlacedBoats)
            {
                this.battleshipServer.Transmit(new Message(Message.ID.SUBMIT_BOATS, Message.State.ERROR, Encoding.UTF8.GetBytes("Already placed boats!")), player.GetConnection());
                return;
            }
                
            List<GridObject> gridobjects = new List<GridObject>();

            if (boatsData.Count != 5)
            {
                this.battleshipServer.Transmit(new Message(Message.ID.SUBMIT_BOATS, Message.State.ERROR, Encoding.UTF8.GetBytes("Invalid boat data!")), player.GetConnection());
                return;
            }

            int[] sizes = new int[5] { 5, 4, 3, 3, 2 };
            int index = 0;
            foreach((int indexX, int indexY, int direction) boatData in boatsData)
            {
                GridObject gridObject = new GridObject(sizes[index], boatData.indexX, boatData.indexY, (GridObject.Direction)boatData.direction);
                if (!gamePlayer.GetGrid().CheckGridObjectPlacement(gridObject))
                {
                    this.battleshipServer.Transmit(new Message(Message.ID.SUBMIT_BOATS, Message.State.ERROR, Encoding.UTF8.GetBytes("Invalid boat data!")), player.GetConnection());
                    return;
                }
                else
                    gridobjects.Add(gridObject);
                index++;
            }

            foreach (GridObject gridObject in gridobjects)
                gamePlayer.GetGrid().PlaceGridObject(gridObject);

            gamePlayer.HasPlacedBoats = true;

            if (this.players[0].HasPlacedBoats && this.players[1].HasPlacedBoats)
            {
                this.session.Broadcast(new Message(Message.ID.START_MATCH, Message.State.OK, null));
                this.state = GameState.GAME;
            }
        }

        private bool SubmitMove(int indexX, int indexY)
        {
            GamePlayer gamePlayer = this.players[this.currentTurnIndex];
            GamePlayer enemyPlayer = this.players[(this.currentTurnIndex == 0) ? 1 : 0];

            if(enemyPlayer.GetGrid().EvaluateMove(indexX, indexY))
            {
                List<byte> bytes = new List<byte>();
                bytes.AddRange(new byte[2] { (byte)indexX, (byte)indexY});

                if (enemyPlayer.GetGrid().ExecuteMove(indexX, indexY))
                {
                    bytes.Add(1);
                    bytes.AddRange(Encoding.UTF8.GetBytes(gamePlayer.Player.GetUsername()));

                    this.session.Broadcast(new Message(Message.ID.SUBMIT_MOVE, Message.State.OK, bytes.ToArray()));
                    enemyPlayer.HitPoints--;

                    if(enemyPlayer.HitPoints == 0)
                    {
                        this.session.Broadcast(new Message(Message.ID.END_GAME, Message.State.OK, Encoding.UTF8.GetBytes(gamePlayer.Player.GetUsername() + " won the game!")));
                        gamePlayer.Player.IsReady = false;
                        enemyPlayer.Player.IsReady = false;
                        return false;
                    }
                }
                else
                {
                    bytes.Add(0);
                    bytes.AddRange(Encoding.UTF8.GetBytes(gamePlayer.Player.GetUsername()));

                    this.currentTurnIndex = (this.currentTurnIndex == 0) ? 1 : 0;
                    this.session.Broadcast(new Message(Message.ID.SUBMIT_MOVE, Message.State.OK, bytes.ToArray()));
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
                            if(content.Count == 0 || content.Count % 5 != 0)
                                this.battleshipServer.Transmit(new Message(Message.ID.SUBMIT_BOATS, Message.State.ERROR, Encoding.UTF8.GetBytes("Invalid boat data!")), player.GetConnection());
                            else
                            {
                                List<(int indexX, int indexY, int direction)> boatsData = new List<(int indexX, int indexY, int direction)>();
                                for (int i = 0; i < content.Count; i += 3)
                                    boatsData.Add((indexX: content[i], indexY: content[i + 1], direction: content[i + 2]));

                                SubmitBoats(boatsData, player);
                            }
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

        public string GetEndGameText()
        {
            if (players[0].HitPoints < players[1].HitPoints)
                return players[0].Player.GetUsername() + " won the game!";
            else if(players[0].HitPoints == players[1].HitPoints)
                return "The game was tied!";
            return players[1].Player.GetUsername() + " won the game!";
        }
    }
}
