using System.Collections.Generic;
using PKHeX.Core;
using PKHeX.Mobile.Logic;

namespace PKHeX.ViewModels
{
    public class ComboSource
    {
        public IReadOnlyList<ComboObject> SpeciesList { get; } = GameInfo.FilteredSources.Species.Convert();
        public IReadOnlyList<ComboObject> MoveList { get; } = GameInfo.FilteredSources.Moves.Convert();
        public IReadOnlyList<ComboObject> ItemList { get; } = GameInfo.FilteredSources.Items.Convert();
        public IReadOnlyList<ComboObject> NatureList { get; } = GameInfo.FilteredSources.Natures.Convert();
        public IReadOnlyList<ComboObject> Versions { get; } = GameInfo.FilteredSources.Games.Convert();
        public IReadOnlyList<ComboObject> Balls { get; } = GameInfo.FilteredSources.Balls.Convert();
    }
}