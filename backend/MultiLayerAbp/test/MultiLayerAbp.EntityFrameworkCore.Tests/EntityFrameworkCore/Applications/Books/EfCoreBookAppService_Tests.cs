using MultiLayerAbp.Books;
using Xunit;

namespace MultiLayerAbp.EntityFrameworkCore.Applications.Books;

[Collection(MultiLayerAbpTestConsts.CollectionDefinitionName)]
public class EfCoreBookAppService_Tests : BookAppService_Tests<MultiLayerAbpEntityFrameworkCoreTestModule>
{

}