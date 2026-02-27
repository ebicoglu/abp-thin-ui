using MultiLayerAbp.Samples;
using Xunit;

namespace MultiLayerAbp.EntityFrameworkCore.Domains;

[Collection(MultiLayerAbpTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MultiLayerAbpEntityFrameworkCoreTestModule>
{

}
