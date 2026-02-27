using System.Threading.Tasks;

namespace MultiLayerAbp.Data;

public interface IMultiLayerAbpDbSchemaMigrator
{
    Task MigrateAsync();
}
