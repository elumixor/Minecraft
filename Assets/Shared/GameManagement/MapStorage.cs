using System.Collections.Generic;
using Shared.Blocks;

namespace Shared.GameManagement {
    // todo: use dictionary internally, but implement IDictionary, etc.
    // todo: use custom structure for values as well
    public class MapStorage : SortedDictionary<long, SortedDictionary<int, BlockType>> { }
}