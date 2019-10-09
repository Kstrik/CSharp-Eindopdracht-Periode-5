using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Battleship
{
    public class Message
    {
        public enum ID
        {
            REGISTER = 0x00,
            LOGIN = 0x01,
            LOGOUT = 0x02,

            GET_SESSIONS = 0x03,
            SESSIONDATA = 0x04,
            ADD_SESSION = 0x05,
            REMOVE_SESSION = 0x06,
            JOIN_SESSION = 0x07,
            LEAVE_SESSION = 0x08,

            READY = 0x09,
            UNREADY = 0xA,
            GET_PLAYERS = 0xB,
            PLAYERDATA = 0xC,
            REMOVE_PLAYER = 0xD,
            CHAT_MESSAGE = 0xE,
            START_GAME = 0xF,
            END_GAME = 0x10,
            SUBMIT_MOVE = 0x11,
            SUBMIT_BOATS = 0x12,
            NEXT_TURN = 0X13
        }

        public enum State
        {
            NONE = 0x00,
            ERROR = 0x01,
            OK = 0x02
        }

        private byte id;
        private byte state;
        private byte[] contentLength;
        private byte[] content;

        public Message(ID id, State state, byte[] content)
        {
            this.id = (byte)id;
            this.state = (byte)state;

            if (content == null)
                content = new byte[0];

            this.contentLength = new byte[4];
            contentLength[0] = (byte)content.Length;
            contentLength[1] = (byte)(content.Length >> 8);
            contentLength[2] = (byte)(content.Length >> 16);
            contentLength[3] = (byte)(content.Length >> 24);

            this.content = content;
        }

        private Message(ID id, State state, byte[] contentLength, byte[] content)
        {
            this.id = (byte)id;
            this.state = (byte)state;
            this.contentLength = contentLength;
            this.content = content;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(this.id);
            bytes.Add(this.state);
            bytes.AddRange(this.contentLength);
            bytes.AddRange(this.content);

            return bytes.ToArray();
        }

        public static Message ParseMessage(byte[] bytes)
        {
            byte[] contentLength = new byte[] { bytes[2], bytes[3], bytes[4], bytes[5] };
            byte[] content = new List<byte>(bytes).GetRange(6, bytes.Length - 6).ToArray();
            return new Message((ID)bytes[0], (State)bytes[1], contentLength, content);
        }

        public ID GetId()
        {
            return (ID)this.id;
        }

        public State GetState()
        {
            return (State)this.state;
        }

        public byte[] GetContent()
        {
            return this.content;
        }
    }
}
