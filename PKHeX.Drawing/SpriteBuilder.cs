using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing
{
    public class SpriteBuilder : ISpriteBuilder<SKBitmap>
    {
        public static bool ShowEggSpriteAsItem { get; set; } = true;

        private const int ItemShiftX = 52;
        private const int ItemShiftY = 24;
        private const int ItemMaxSize = 32;
        private const int EggItemShiftX = 32;
        private const int EggItemShiftY = 26;

        private const double UnknownFormTransparency = 0.5;
        private const double ShinyTransparency = 0.7;
        private const double EggUnderLayerTransparency = 0.33;

        public void Initialize(SaveFile sav)
        {
            if (sav.Generation != 3)
                return;

            Game = sav.Version;
            if (Game == GameVersion.FRLG)
                Game = sav.Personal == PersonalTable.FR ? GameVersion.FR : GameVersion.LG;
        }

        private GameVersion Game;

        private static int GetDeoxysForm(GameVersion game)
        {
            return game switch
            {
                GameVersion.FR => 1, // Attack
                GameVersion.LG => 2, // Defense
                GameVersion.E => 3, // Speed
                _ => 0
            };
        }

        public static SKBitmap LoadBitmapResource(string resourceID) => ResourceFetch.LoadBitmapResource(resourceID);

        public static SKBitmap GetGameIcon(GameVersion game)
        {
            var img = LoadBitmapResource($"games.{(int)game}");
            return img ?? LoadBitmapResource($"games.{game}");
        }

        public SKBitmap GetSprite(int species, int form, int gender, uint formarg, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
        {
            if (species == 0)
                return LoadBitmapResource("pkm.b_0");

            if (generation == 3 && species == 386) // Deoxys, special consideration for Gen3 save files
                form = GetDeoxysForm(Game);

            var baseImage = GetBaseImage(species, form, gender, formarg, isShiny, generation);

            return GetSprite(baseImage, species, heldItem, isEgg, isShiny, generation, isBoxBGRed);
        }

        public SKBitmap GetSprite(SKBitmap baseSprite, int species, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
        {
            if (isEgg)
                baseSprite = LayerOverImageEgg(baseSprite, species, heldItem != 0);
            if (isShiny)
                baseSprite = LayerOverImageShiny(baseSprite, isBoxBGRed);
            if (heldItem > 0)
                baseSprite = LayerOverImageItem(baseSprite, heldItem, generation);
            return baseSprite;
        }

        private static SKBitmap GetBaseImage(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var img = FormInfo.IsTotemForm(species, form, generation)
                        ? GetBaseImageTotem(species, form, gender, formarg, shiny, generation)
                        : GetBaseImageDefault(species, form, gender, formarg, shiny, generation);
            return img ?? GetBaseImageFallback(species, form, gender, formarg, shiny, generation);
        }

        private static SKBitmap GetBaseImageTotem(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var baseform = FormInfo.GetTotemBaseForm(species, form);
            var baseImage = GetBaseImageDefault(species, baseform, gender, formarg, shiny, generation);
            return ImageUtil.ToGrayscale(baseImage);
        }

        private static SKBitmap GetBaseImageDefault(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var file = SpriteName.GetResourceStringSprite(species, form, gender, formarg, generation, shiny)[1..];
            if (shiny)
                return LoadBitmapResource($"pkms.b_{file}s");
            return LoadBitmapResource($"pkm.b_{file}");
        }

        private static SKBitmap GetBaseImageFallback(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            if (shiny) // try again without shiny
            {
                var img = GetBaseImageDefault(species, form, gender, formarg, false, generation);
                if (img != null)
                    return img;
            }

            // try again without form
            var baseImage = LoadBitmapResource($"pkm.b_{species}");
            if (baseImage == null) // failed again
                return LoadBitmapResource("pkm.b_0");
            return ImageUtil.LayerImage(baseImage, LoadBitmapResource("pkm.b_0"), 0, 0, UnknownFormTransparency);
        }

        private static SKBitmap LayerOverImageItem(SKBitmap baseImage, int item, int generation)
        {
            var itemimg = LoadBitmapResource($"item.item_{item}") ??
                          LoadBitmapResource("misc.helditem");
            if (generation >= 2 && generation <= 4 && 328 <= item && item <= 419) // gen2/3/4 TM
                itemimg = LoadBitmapResource("item.item_tm");

            // Redraw item in bottom right corner; since images are cropped, try to not have them at the edge
            int x = ItemShiftX + ((ItemMaxSize - itemimg.Width) / 2);
            if (x + itemimg.Width > baseImage.Width)
                x = baseImage.Width - itemimg.Width;
            int y = ItemShiftY + (ItemMaxSize - itemimg.Height);
            return ImageUtil.LayerImage(baseImage, itemimg, x, y);
        }

        private static SKBitmap LayerOverImageShiny(SKBitmap baseImage, bool isBoxBGRed)
        {
            // Add shiny star to top left of image.
            var rare = isBoxBGRed ? LoadBitmapResource("misc.rare_icon_alt") : LoadBitmapResource("misc.rare_icon");
            return ImageUtil.LayerImage(baseImage, rare, 0, 0, ShinyTransparency);
        }

        private static SKBitmap LayerOverImageEgg(SKBitmap baseImage, int species, bool hasItem)
        {
            if (ShowEggSpriteAsItem && !hasItem)
                return LayerOverImageEggAsItem(baseImage, species);
            return LayerOverImageEggTransparentSpecies(baseImage, species);
        }

        private static SKBitmap GetEggSprite(int _) => LoadBitmapResource("pkm.b_0-1");

        private static SKBitmap LayerOverImageEggTransparentSpecies(SKBitmap baseImage, int species)
        {
            // Partially transparent species.
            baseImage = ImageUtil.ChangeOpacity(baseImage, EggUnderLayerTransparency);
            // Add the egg layer over-top with full opacity.
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, 0, 0);
        }

        private static SKBitmap LayerOverImageEggAsItem(SKBitmap baseImage, int species)
        {
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, EggItemShiftX, EggItemShiftY); // similar to held item, since they can't have any
        }
    }
}
