using MultiLayerAbp.Samples;
using Xunit;

namespace MultiLayerAbp.EntityFrameworkCore.Applications;

[Collection(MultiLayerAbpTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MultiLayerAbpEntityFrameworkCoreTestModule>
{

}
