# Particular.LicensingComponent.Report

The Particular.LicensingComponent.Report nuget package that is used by to manage the throughput report data contract, serialization and validation required for NServiceBus licensing purposes.
This package is deploted to feedz and is currently used in:

- [Endpoint Throughput Counter Tool](https://github.com/Particular/Particular.EndpointThroughputCounter)
- [Platform License Component hosted in ServiceControl](https://github.com/Particular/ServiceControl)
- Throughput Report Validator used internally by Particular to validate and process usage reports received from customers.

## Deployment

### Particular.LicensingComponent.Report nuget

To deploy a new version of the nuget package, tag the master branch with a new version number.

> [!NOTE]  
> Tagging the branch will **not** create a new Tools website deployment.

## Environment variables

### Report private key

`THROUGHPUT_REPORT_PRIVATEKEY_PEM` is required for running unit tests on Particular.LicensingComponent.Report in the `ci` Github actions workflow.

## Development
