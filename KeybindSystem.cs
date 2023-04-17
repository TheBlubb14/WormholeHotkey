using Terraria.ModLoader;

namespace WormholeHotkey
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind TeleportKeybind { get; private set; }
        public static ModKeybind HomeKeybind { get; private set; }

        public override void Load()
        {
            TeleportKeybind = KeybindLoader.RegisterKeybind(Mod, "Wormhole", "P");
            HomeKeybind = KeybindLoader.RegisterKeybind(Mod, "MagicMirror", "H");
        }

        public override void Unload()
        {
            TeleportKeybind = null;
            HomeKeybind = null;
        }
    }
}