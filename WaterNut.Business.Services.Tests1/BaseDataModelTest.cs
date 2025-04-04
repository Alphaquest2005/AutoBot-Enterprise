// <copyright file="BaseDataModelTest.cs">Copyright ©  2014</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.Tests
{
    /// <summary>This class contains parameterized unit tests for BaseDataModel</summary>
    [PexClass(typeof(BaseDataModel))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class BaseDataModelTest
    {



    }
}
