Tax Framework

This plugin provides a base for integrating external tax providers into the ICalculateCartPipeline. The following pipelines have been added:

ICallExternalServiceToCalculateTaxPipeline
This pipeline can be used to extend the service that is used to calculate tax on cart. Input: TaxableCartModel Output: TaxBreakdownModel

IPopulateNexusAddressPipeline
This pipeline is used to provide the plugin with a list of addresses that the shop has "Nexus" in. This can be configured in most tax providers, but it added to allow greater flexibility for internation calculations
