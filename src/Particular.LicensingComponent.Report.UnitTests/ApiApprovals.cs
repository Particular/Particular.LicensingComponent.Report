namespace ToolsSite.UnitTests.LicensingComponent;

using NUnit.Framework;
using Particular.Approvals;
using Particular.LicensingComponent.Report;
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{
    [Test]
    public void Approve()
    {
        var publicApi = typeof(Report).Assembly.GeneratePublicApi(new ApiGeneratorOptions
        {
            ExcludeAttributes = new[] { "System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute" }
        });
        try
        {
            Approver.Verify(publicApi);
        }
        catch (Exception)
        {
            Console.WriteLine(publicApi);
            throw;
        }
    }
}