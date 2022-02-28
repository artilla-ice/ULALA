namespace ULALA.Domain.Contracts
{
    public enum RepositoryInitializeStrategy
	{
		DropAlways,
		DropIfModelChanged,
		Migrate,
	}

	public interface IRepositoryInitializer
	{
		void SetInitializeStrategy(RepositoryInitializeStrategy strategy);
        void Initialize(bool force);
		void SeedSystemData(string xmlData);
	}
}
