namespace Shared.SpaceWrapping {
    public class Unwrapper3D : Unwrapper {
        protected override void Unwrap(ulong i, out int x, out int y, out int z) => Wrapper.Unwrap(i, out x, out y, out z);
        protected override ulong Wrap(int x, int y, int z) => Wrapper.Wrap(x, y, z);
    }
}