using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes;
using Hazel;
using UnityEngine;

namespace HarryPotter.CustomOption
{
	public static class Rpc
    {
        public static void SendRpc(CustomOption optionn = null)
        {
            List<CustomOption> options;
            if (optionn != null)
                options = new List<CustomOption> {optionn};
            else
                options = CustomOption.AllOptions;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)Packets.SyncCustomSettings, SendOption.Reliable);
            foreach (var option in options)
            {
                if (writer.Position > 1000) {
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)Packets.SyncCustomSettings, SendOption.Reliable);
                }
                writer.Write(option.ID);
                if (option.Type == CustomOptionType.Toggle) writer.Write((bool) option.Value);
                else if (option.Type == CustomOptionType.Number) writer.Write((float) option.Value);
                else if (option.Type == CustomOptionType.String) writer.Write((int) option.Value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ReceiveRpc(MessageReader reader)
        {
            while (reader.BytesRemaining > 0)
            {
                var id = reader.ReadInt32();
                var customOption =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.ID == id); // Works but may need to change to gameObject.name check
                var type = customOption?.Type;
                object value = null;
                if (type == CustomOptionType.Toggle) value = reader.ReadBoolean();
                else if (type == CustomOptionType.Number) value = reader.ReadSingle();
                else if (type == CustomOptionType.String) value = reader.ReadInt32();

                customOption?.Set(value);

                var panels = GameObject.FindObjectsOfType<ViewSettingsInfoPanel>();
                foreach (var panel in panels) {
                    if (panel.titleText.text == customOption.Name)
                    {
                        panel.SetInfo(StringNames.ImpostorsCategory, customOption.ToString(), 61);
                        panel.titleText.text = customOption.Name;
                    }
                }
            }
        }
    }
}
