using Xunit;

namespace MultiLayerAbp.EntityFrameworkCore;

[CollectionDefinition(MultiLayerAbpTestConsts.CollectionDefinitionName)]
public class MultiLayerAbpEntityFrameworkCoreCollection : ICollectionFixture<MultiLayerAbpEntityFrameworkCoreFixture>
{

}
