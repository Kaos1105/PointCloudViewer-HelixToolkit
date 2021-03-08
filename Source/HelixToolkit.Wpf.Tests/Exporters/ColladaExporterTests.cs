﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColladaExporterTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Schema;
    using HelixToolkit.Wpf;
    using NUnit.Framework;
    using System;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ColladaExporterTests : ExporterTests
    {
        [SetUp]
        public void SetUp()
        {
            var dir = Path.GetDirectoryName(typeof(ColladaExporterTests).Assembly.Location);
            Directory.SetCurrentDirectory(dir);
        }

        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.dae";

            try
            {
                var e = new ColladaExporter();
                using (var stream = File.Create(path))
                {
                    this.ExportSimpleModel(e, stream);
                }

                var result = this.Validate(path);
                Assert.IsNull(result, result);
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        private string Validate(string path)
        {
            var sc = new XmlSchemaSet();
            string dir = @"..\..\..\..\Schemas\Collada\";
            sc.Add("http://www.collada.org/2008/03/COLLADASchema", dir + "collada_schema_1_5.xsd");
            return this.Validate(path, sc);
        }
    }
}