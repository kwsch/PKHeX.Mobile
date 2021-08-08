using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing
{
    public static class SpriteUtil
    {
        public static ISpriteBuilder<SKBitmap> Spriter { get; set; } = new SpriteBuilder();

        public static SKBitmap GetBallSprite(int ball)
        {
            string resource = SpriteName.GetResourceStringBall(ball);
            return SpriteBuilder.LoadBitmapResource("ball." + resource) ?? SpriteBuilder.LoadBitmapResource("ball._ball4"); // Poké Ball (default)
        }

        public static SKBitmap GetSprite(int species, int form, int gender, uint formarg, int item, bool isegg, bool shiny, int generation = -1, bool isBoxBGRed = false)
        {
            return Spriter.GetSprite(species, form, gender, formarg, item, isegg, shiny, generation, isBoxBGRed);
        }

        public static SKBitmap GetRibbonSprite(string name)
        {
            var resource = name.Replace("CountG3", "G3").ToLower();
            return SpriteBuilder.LoadBitmapResource("ribbon." + resource);
        }

        public static SKBitmap GetRibbonSprite(string name, int max, int value)
        {
            var resource = GetRibbonSpriteName(name, max, value);
            return SpriteBuilder.LoadBitmapResource("ribbon." + resource);
        }

        private static string GetRibbonSpriteName(string name, int max, int value)
        {
            if (max != 4) // Memory
            {
                var sprite = name.ToLower();
                if (max == value)
                    return sprite + "2";
                return sprite;
            }

            // Count ribbons
            string n = name.Replace("Count", string.Empty).ToLower();
            return value switch
            {
                2 => (n + "super"),
                3 => (n + "hyper"),
                4 => (n + "master"),
                _ => n
            };
        }

        public static SKBitmap GetTypeSprite(int type, int generation = PKX.Generation)
        {
            if (generation <= 2)
                type = (int)((MoveType)type).GetMoveTypeGeneration(generation);
            return SpriteBuilder.LoadBitmapResource($"type.type_icon_{type:00}");
        }

        private static SKBitmap GetSprite(MysteryGift gift)
        {
            if (gift.Empty)
                return null;

            var img = GetBaseImage(gift);
            if (gift.GiftUsed)
                img = ImageUtil.ChangeOpacity(img, 0.3);
            return img;
        }

        private static SKBitmap GetBaseImage(MysteryGift gift)
        {
            if (gift.IsEgg && gift.Species == 490) // Manaphy Egg
                return SpriteBuilder.LoadBitmapResource("pkm.490_e");
            if (gift.IsPokémon)
                return GetSprite(gift.Species, gift.Form, gift.Gender, 0, gift.HeldItem, gift.IsEgg, gift.IsShiny, gift.Generation);
            if (gift.IsItem)
            {
                int item = gift.ItemID;
                if (Legal.ZCrystalDictionary.TryGetValue(item, out int value))
                    item = value;
                return SpriteBuilder.LoadBitmapResource($"item.item_{item}") ?? SpriteBuilder.LoadBitmapResource("bag.Bag_Key");
            }
            return SpriteBuilder.LoadBitmapResource("pkm.unknown");
        }

        private static SKBitmap GetSprite(PKM pk, bool isBoxBGRed = false)
        {
            var formarg = pk is IFormArgument f ? f.FormArgument : 0;
            var img = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, pk.SpriteItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
            if (pk is IShadowPKM s && s.Purification > 0)
            {
                const int Lugia = 249;
                if (pk.Species == Lugia) // show XD shadow sprite
                    img = Spriter.GetSprite(SpriteBuilder.LoadBitmapResource("pkm.249x"), Lugia, pk.HeldItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
                GetSpriteGlow(img, 75, 0, 130, out var pixels, true);
                var glowImg = img.Copy();
                ImageUtil.SetBitmapData(glowImg, pixels);
                img = ImageUtil.LayerImage(glowImg, img, 0, 0);
            }
            return img;
        }

        private static SKBitmap GetWallpaper(SaveFile sav, int box)
        {
            string s = BoxWallpaper.GetWallpaperResourceName(sav.Version, sav.GetBoxWallpaper(box));
            var folder = s[8..];
            s = $"box.{folder}.{s}";
            return SpriteBuilder.LoadBitmapResource(s) ?? SpriteBuilder.LoadBitmapResource("box.xy.box_wp16xy");
        }

        private static SKBitmap GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        {
            if (!pk.Valid)
                return null;

            bool inBox = (uint)slot < MaxSlotCount;
            bool empty = pk.Species == 0;
            var sprite = empty ? null : pk.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(sav.Version, sav.GetBoxWallpaper(box)));

            if (!empty && flagIllegal)
            {
                var la = new LegalityAnalysis(pk, sav.Personal, box != -1 ? SlotOrigin.Box : SlotOrigin.Party);
                if (!la.Valid)
                    sprite = ImageUtil.LayerImage(sprite, SpriteBuilder.LoadBitmapResource("warn"), 0, FlagIllegalShiftY);
            }
            if (inBox) // in box
            {
                var flags = sav.GetSlotFlags(box, slot);
                if (flags.HasFlagFast(StorageSlotFlag.Locked))
                    sprite = ImageUtil.LayerImage(sprite, SpriteBuilder.LoadBitmapResource("misc.locked"), SlotLockShiftX, 0);
                int team = flags.IsBattleTeam();
                if (team >= 0)
                    sprite = ImageUtil.LayerImage(sprite, SpriteBuilder.LoadBitmapResource("misc.team"), SlotTeamShiftX, 0);
                int party = flags.IsParty();
                if (party >= 0)
                    sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
                if (flags.HasFlagFast(StorageSlotFlag.Starter))
                    sprite = ImageUtil.LayerImage(sprite, SpriteBuilder.LoadBitmapResource("misc.starter"), 0, 0);
            }

            return sprite;
        }

        private const int MaxSlotCount = 30; // slots in a box
        private const int SpriteWidth = 68;
        private const int SpriteHeight = 56;
        private const int PartyMarkShiftX = SpriteWidth - 16;
        private const int SlotLockShiftX = SpriteWidth - 14;
        private const int SlotTeamShiftX = SpriteWidth - 19;
        private const int FlagIllegalShiftY = SpriteHeight - 16;

        private static readonly SKBitmap[] PartyMarks =
        {
            SpriteBuilder.LoadBitmapResource("misc.party1"),
            SpriteBuilder.LoadBitmapResource("misc.party2"),
            SpriteBuilder.LoadBitmapResource("misc.party3"),
            SpriteBuilder.LoadBitmapResource("misc.party4"),
            SpriteBuilder.LoadBitmapResource("misc.party5"),
            SpriteBuilder.LoadBitmapResource("misc.party6"),
        };

        public static void GetSpriteGlow(PKM pk, byte[] bgr, out byte[] pixels, out SKBitmap baseSprite, bool forceHollow = false)
        {
            GetSpriteGlow(pk, bgr[0], bgr[1], bgr[2], out pixels, out baseSprite, forceHollow);
        }

        public static void GetSpriteGlow(PKM pk, byte blue, byte green, byte red, out byte[] pixels, out SKBitmap baseSprite, bool forceHollow = false)
        {
            bool egg = pk.IsEgg;
            var formarg = pk is IFormArgument f ? f.FormArgument : 0;
            baseSprite = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, 0, egg, false, pk.Format);
            GetSpriteGlow(baseSprite, blue, green, red, out pixels, forceHollow || egg);
        }

        public static void GetSpriteGlow(SKBitmap baseSprite, byte blue, byte green, byte red, out byte[] pixels, bool forceHollow = false)
        {
            pixels = ImageUtil.GetBitmapData(baseSprite);
            if (!forceHollow)
            {
                ImageUtil.GlowEdges(pixels, blue, green, red, baseSprite.Width);
                return;
            }

            // If the SKBitmap has any transparency, any derived background will bleed into it.
            // Need to undo any transparency values if any present.
            // Remove opaque pixels from original SKBitmap, leaving only the glow effect pixels.
            var original = (byte[])pixels.Clone();
            ImageUtil.SetAllUsedPixelsOpaque(pixels);
            ImageUtil.GlowEdges(pixels, blue, green, red, baseSprite.Width);
            ImageUtil.RemovePixels(pixels, original);
        }

        // Extension Methods
        public static SKBitmap WallpaperSKBitmap(this SaveFile sav, int box) => GetWallpaper(sav, box);
        public static SKBitmap Sprite(this MysteryGift gift) => GetSprite(gift);
        public static SKBitmap Sprite(this PKM pk, bool isBoxBGRed = false) => GetSprite(pk, isBoxBGRed);

        public static SKBitmap Sprite(this PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
            => GetSprite(pk, sav, box, slot, flagIllegal);
    }
}