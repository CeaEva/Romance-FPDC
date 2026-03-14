using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace Resources
{
    [GlobalClass]
    public partial class AutoMapStepped : Resource
    {
        [Export] public Array<Vector2I> RevealedCells { get; set; } = new();

        private readonly HashSet<Vector2I> _revealedLookup = new();
        private bool _lookupInitialized;

        public bool IsRevealed(Vector2I cell)
        {
            EnsureLookup();
            return _revealedLookup.Contains(cell);
        }

        public bool MarkRevealed(Vector2I cell)
        {
            EnsureLookup();
            if (!_revealedLookup.Add(cell))
                return false;

            RevealedCells.Add(cell);
            return true;
        }

        public void RebuildLookup()
        {
            _revealedLookup.Clear();

            for (int i = 0; i < RevealedCells.Count; i++)
                _revealedLookup.Add(RevealedCells[i]);

            _lookupInitialized = true;
        }

        public void ClearRevealed()
        {
            RevealedCells.Clear();
            _revealedLookup.Clear();
            _lookupInitialized = true;
        }

        private void EnsureLookup()
        {
            if (_lookupInitialized)
                return;

            RebuildLookup();
        }
    }
}
