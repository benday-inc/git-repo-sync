
[TestClass]
public class GitRepoSyncConfigurationFixture
{
    [TestInitialize]
    public void OnTestInitialize()
    {
        _SystemUnderTest = null;
    }

    private GitRepoSyncConfiguration? _SystemUnderTest;

    private GitRepoSyncConfiguration SystemUnderTest
    {
        get
        {
            if (_SystemUnderTest == null)
            {
                _SystemUnderTest = new GitRepoSyncConfiguration();
            }

            return _SystemUnderTest;
        }
    }    
}