using Xunit;

namespace reviews.Tests
{

    /// This is so all tests run sequentially to avoid DB conflicts
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollection
    {
        // This class is never actually instantiated
    }
}
