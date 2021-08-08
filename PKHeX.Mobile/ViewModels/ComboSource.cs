using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.ViewModels
{
    public class ComboSource
    {
        public IReadOnlyList<ComboItem> SpeciesList { get; } = GameInfo.FilteredSources.Species;
        public IReadOnlyList<ComboItem> MoveList { get; } = GameInfo.FilteredSources.Moves;
        public IReadOnlyList<ComboItem> ItemList { get; } = GameInfo.FilteredSources.Items;
        public IReadOnlyList<ComboItem> NatureList { get; } = GameInfo.FilteredSources.Natures;
        public IReadOnlyList<ComboItem> Versions { get; } = GameInfo.FilteredSources.Games;
        public IReadOnlyList<ComboItem> Balls { get; } = GameInfo.FilteredSources.Balls;
    }
}
