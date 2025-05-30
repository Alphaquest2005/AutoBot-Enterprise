// <copyright file="PexAssemblyInfo.cs">Copyright ©  2014</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "NUnit3")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("WaterNut.Business.Services")]
[assembly: PexInstrumentAssembly("TrackableEntities.Common")]
[assembly: PexInstrumentAssembly("TrackableEntities.Client")]
[assembly: PexInstrumentAssembly("Core.Common.UI")]
[assembly: PexInstrumentAssembly("TrackableEntities.EF.6")]
[assembly: PexInstrumentAssembly("AsycudaWorld421")]
[assembly: PexInstrumentAssembly("DataLayer")]
[assembly: PexInstrumentAssembly("System.Linq.Dynamic")]
[assembly: PexInstrumentAssembly("Tesseract")]
[assembly: PexInstrumentAssembly("ParallelExtensionsExtras")]
[assembly: PexInstrumentAssembly("Omu.ValueInjecter")]
[assembly: PexInstrumentAssembly("pdf-ocr")]
[assembly: PexInstrumentAssembly("MoreLinq")]
[assembly: PexInstrumentAssembly("UglyToad.PdfPig.DocumentLayoutAnalysis")]
[assembly: PexInstrumentAssembly("Microsoft.CSharp")]
[assembly: PexInstrumentAssembly("WaterNut.Business.Entities")]
[assembly: PexInstrumentAssembly("System.ComponentModel.Composition")]
[assembly: PexInstrumentAssembly("WaterNut.Data")]
[assembly: PexInstrumentAssembly("Core.Common")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("System.ServiceModel")]
[assembly: PexInstrumentAssembly("Core.Common.Contracts")]
[assembly: PexInstrumentAssembly("UglyToad.PdfPig")]
[assembly: PexInstrumentAssembly("WaterNut.Interfaces")]
[assembly: PexInstrumentAssembly("Core.Common.Data")]
[assembly: PexInstrumentAssembly("System.Transactions")]
[assembly: PexInstrumentAssembly("EntityFramework")]
[assembly: PexInstrumentAssembly("System.Data")]
[assembly: PexInstrumentAssembly("EmailDownloader")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "TrackableEntities.Common")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "TrackableEntities.Client")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Core.Common.UI")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "TrackableEntities.EF.6")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "AsycudaWorld421")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "DataLayer")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Linq.Dynamic")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Tesseract")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "ParallelExtensionsExtras")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Omu.ValueInjecter")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "pdf-ocr")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "MoreLinq")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "UglyToad.PdfPig.DocumentLayoutAnalysis")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.CSharp")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "WaterNut.Business.Entities")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.ComponentModel.Composition")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "WaterNut.Data")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Core.Common")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.ServiceModel")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Core.Common.Contracts")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "UglyToad.PdfPig")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "WaterNut.Interfaces")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Core.Common.Data")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Transactions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "EntityFramework")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Data")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "EmailDownloader")]

