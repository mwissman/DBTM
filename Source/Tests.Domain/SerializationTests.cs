using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class SerializationTests
    {
        private string EXPECTED_FORMAT_WITH_1_VERSION =
            @"<?xml version=""1.0""?>
<Database xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Name=""{0}"">
  <Version Id=""{1}"" Created=""{5}"" IsBaseline=""false"">
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{2}</Description>
      <UpgradeSQL>{3}</UpgradeSQL>
      <RollbackSQL>{4}</RollbackSQL>
    </PreDeploymentSqlStatement>
  </Version>
</Database>";

        private string EXPECTED_FORMAT_WITH_MULTIPLE_STATEMENTS_IN_A_VERSION =
            @"<?xml version=""1.0""?>
<Database xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Name=""{0}"">
  <Version Id=""{1}"" Created=""{8}"" IsBaseline=""false"">
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{2}</Description>
      <UpgradeSQL>{3}</UpgradeSQL>
      <RollbackSQL>{4}</RollbackSQL>
    </PreDeploymentSqlStatement>
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{5}</Description>
      <UpgradeSQL>{6}</UpgradeSQL>
      <RollbackSQL>{7}</RollbackSQL>
    </PreDeploymentSqlStatement>
  </Version>
</Database>";

        private string EXPECTED_FORMAT_WITH_MULTIPLE_VERSIONS =
              @"<?xml version=""1.0""?>
<Database xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Name=""{0}"">
  <Version Id=""{1}"" Created=""{12}"" IsBaseline=""false"">
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{2}</Description>
      <UpgradeSQL>{3}</UpgradeSQL>
      <RollbackSQL>{4}</RollbackSQL>
    </PreDeploymentSqlStatement>
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{5}</Description>
      <UpgradeSQL>{6}</UpgradeSQL>
      <RollbackSQL>{7}</RollbackSQL>
    </PreDeploymentSqlStatement>
  </Version>
  <Version Id=""{8}"" Created=""{13}"" IsBaseline=""false"">
    <PreDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{9}</Description>
      <UpgradeSQL>{10}</UpgradeSQL>
      <RollbackSQL>{11}</RollbackSQL>
    </PreDeploymentSqlStatement>
    <PostDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{14}</Description>
      <UpgradeSQL>{15}</UpgradeSQL>
      <RollbackSQL>{16}</RollbackSQL>
    </PostDeploymentSqlStatement>
    <PostDeploymentSqlStatement Id=""00000000-0000-0000-0000-000000000000"">
      <Description>{17}</Description>
      <UpgradeSQL>{18}</UpgradeSQL>
      <RollbackSQL>{19}</RollbackSQL>
    </PostDeploymentSqlStatement>
  </Version>
</Database>";

        private string _dbName = "name";

        private DateTime _now = new DateTime(DateTime.Year,
                                             DateTime.Month,
                                             DateTime.Day,
                                             DateTime.Hour,
                                             DateTime.Minute,
                                             DateTime.Minute);

        private static DateTime DateTime = DateTime.Now.AddHours(4);

        [Test]
        public void DatabaseIsSerializedCorrectlyWithOneVersionInOneStatement()
        {
            string statement = "blah";
            string rollbackStatement = "blahblahblah";
            var description = "description";

            int versionId = 1;

            Database objToSerialize = new Database(_dbName);

            SqlStatementCollection sqlStatements = new SqlStatementCollection
                                                    {
                                                        new SqlStatement(description, statement, rollbackStatement){Id = Guid.Empty}
                                                    };

            DatabaseVersion databaseVersion = new DatabaseVersion(versionId, _now) { PreDeploymentStatements = (sqlStatements) };

            objToSerialize.Versions = new DatabaseVersionCollection { databaseVersion };

            XmlSerializer serializer = new XmlSerializer(typeof(Database));


            string expectedSerializedXml = string.Format(EXPECTED_FORMAT_WITH_1_VERSION, _dbName, versionId, description, statement, rollbackStatement, _now.ToString("s"));

            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, objToSerialize);

            memoryStream.Position = 0;

            StreamReader reader = new StreamReader(memoryStream);
            var actual = reader.ReadToEnd();

            Assert.AreEqual(RemoveAllNamespaces(expectedSerializedXml), RemoveAllNamespaces(actual));
        }

        [Test]
        [Explicit]
        public void Deserialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            //serializer.UnknownElement += new XmlElementEventHandler(serializer_UnknownElement);
            object databaseObj = serializer.Deserialize(
                new StreamReader(@"C:\Work\CorpSystems\root\hive\trunk\DatabaseFiles\KioskManagement.dbschema"));


            Database database = databaseObj as Database;
            Assert.AreEqual(14, database.Versions.First().PreDeploymentStatements.Count);

        }

        [Test]
        public void DatabaseIsSerializedCorrectlyWithOneVersionAndMultipleStatements()
        {
            string statement = "blah";
            string statement2 = "blah2";
            string rollbackStatement = "blahblah";
            string rollbackStatement2 = "blahblah2";
            int versionId = 1;

            Database objToSerialize = new Database(_dbName);

            var description = "description";
            var description2 = "description2";
            SqlStatementCollection sqlStatements = new SqlStatementCollection
                                                    {
                                                        new SqlStatement(description, statement, rollbackStatement){Id = Guid.Empty},
                                                        new SqlStatement(description2,statement2, rollbackStatement2){Id = Guid.Empty}
                                                    };

            objToSerialize.Versions = new DatabaseVersionCollection { new DatabaseVersion(versionId, _now) { PreDeploymentStatements = sqlStatements } };

            XmlSerializer serializer = new XmlSerializer(typeof(Database));

            string expectedSerializedXml = string.Format(EXPECTED_FORMAT_WITH_MULTIPLE_STATEMENTS_IN_A_VERSION, _dbName,
                                                         versionId, description, statement, rollbackStatement, description2, statement2, rollbackStatement2, _now.ToString("s"));

            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, objToSerialize);

            memoryStream.Position = 0;

            StreamReader reader = new StreamReader(memoryStream);
            var actual = reader.ReadToEnd();

            Assert.AreEqual(RemoveAllNamespaces(expectedSerializedXml), RemoveAllNamespaces(actual));
        }

        [Test]
        public void DatabaseIsSerializedCorrectlyWithMultipleVersionsAndMixedStatements()
        {
            string statement = "blah";
            string statement2 = "blah2";
            string statement3 = "blah3";
            string statement6 = "blah6";
            string statement7 = "blah7";

            string rollbackStatement = "blahblah";
            string rollbackStatement2 = "blahblah2";
            string rollbackStatement3 = "blahblah3";
            string rollbackStatement6 = "blahblah6";
            string rollbackStatement7 = "blahblah7";

            int versionId = 1;
            int versionId2 = 2;

            Database objToSerialize = new Database(_dbName);

            var description = "description";
            var description2 = "description2";
            var description3 = "description3";
            var description6 = "description6";
            var description7 = "description7";

            SqlStatementCollection sqlStatements1 = new SqlStatementCollection
                                                    {
                                                        new SqlStatement(description, statement, rollbackStatement){Id = Guid.Empty},
                                                        new SqlStatement(description2,statement2, rollbackStatement2){Id = Guid.Empty}
                                                    };
            SqlStatementCollection sqlStatements2 = new SqlStatementCollection
                                                    {
                                                        new SqlStatement(description3, statement3, rollbackStatement3){Id = Guid.Empty}
                                                    };

            SqlStatementCollection sqlStatements4 = new SqlStatementCollection()
                                                        {
                                                            new SqlStatement(description6,statement6,rollbackStatement6){Id = Guid.Empty},
                                                            new SqlStatement(description7,statement7,rollbackStatement7){Id = Guid.Empty},
                                                        };

            objToSerialize.Versions = new DatabaseVersionCollection
                                          {
                                              new DatabaseVersion(versionId, _now) {PreDeploymentStatements = sqlStatements1},
                                              new DatabaseVersion(versionId2, _now)
                                                  {
                                                      PreDeploymentStatements = sqlStatements2,
                                                      PostDeploymentStatements = sqlStatements4
                                                  }
                                          };

            XmlSerializer serializer = new XmlSerializer(typeof(Database));

            string expectedSerializedXml = string.Format(EXPECTED_FORMAT_WITH_MULTIPLE_VERSIONS, _dbName,
                                                         versionId,
                                                         description,
                                                         statement,
                                                         rollbackStatement,
                                                         description2,
                                                         statement2,
                                                         rollbackStatement2,
                                                         versionId2,
                                                         description3,
                                                         statement3,
                                                         rollbackStatement3,
                                                         _now.ToString("s"),
                                                         _now.ToString("s"),
                                                         description6,
                                                         statement6,
                                                         rollbackStatement6,
                                                         description7,
                                                         statement7,
                                                         rollbackStatement7
                                                         );

            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, objToSerialize);

            memoryStream.Position = 0;

            StreamReader reader = new StreamReader(memoryStream);
            var actual = reader.ReadToEnd();

            Assert.AreEqual(RemoveAllNamespaces(expectedSerializedXml), RemoveAllNamespaces(actual));
        }

        //Implemented based on interface, not part of algorithm
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
    }


}