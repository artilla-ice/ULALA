
namespace ULALA.Domain.Contracts.Data
{
    public interface IDomainRepositoryFactory
	{
		T CreateRepository<T>(params object[] ctx) where T : IDataRepository;
	}
}
