﻿namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventCommand
    {
        public int[] Ints = new int[6];
        public EventMoveRoute Route;
        public string[] Strs = new string[6];
        public EventCommandType Type;

        public EventCommand()
        {
            for (var i = 0; i < 6; i++)
            {
                Strs[i] = "";
                Ints[i] = 0;
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (EventCommandType) myBuffer.ReadInteger();
            for (var x = 0; x < 6; x++)
            {
                Strs[x] = myBuffer.ReadString();
                Ints[x] = myBuffer.ReadInteger();
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route = new EventMoveRoute();
                Route.Load(myBuffer);
            }
        }

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int) Type);
            for (var x = 0; x < 6; x++)
            {
                myBuffer.WriteString(Strs[x]);
                myBuffer.WriteInteger(Ints[x]);
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route.Save(myBuffer);
            }
        }
    }
}