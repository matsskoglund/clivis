using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clivis.Models.Nibe;
using System.IO;
using Clivis.Models;
using Stubbery;

namespace ClivisTests
{
    public class NibeUnitTests
    {
        private NibeUnit nibeUnit;
        private string codeFilePath = "code.txt";

        public NibeUnitTests()
        {
            nibeUnit = new NibeUnit() { clientId = "12345", code = null, passWord = "qwert", redirect_uri = "http://somuri", secret = "mysecret", userName = "myusername@user.se" };
            nibeUnit.CodeFilePath = codeFilePath;
        }
        [Fact]
        public void NibeUnit_code_setter_CodeIsWrittenToFile()
            {
            if (File.Exists(codeFilePath))
                File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted");
            
            nibeUnit.code = "newcode";
            Assert.True(File.Exists(codeFilePath));
            string fileContent = File.ReadAllText(codeFilePath);

            // Assert the file contents
            Assert.Equal("newcode", fileContent);

            // Assert that the variable is set in unit
            Assert.Equal("newcode", nibeUnit.code);

            // Cleanup
            File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted " + new FileInfo(codeFilePath).FullName);

        }
        [Fact]
        public void NibeUnit_code_setter_CodeIsNullAndNotWrittenToFile()
        {
            if (File.Exists(codeFilePath))
                File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted");

            // Set code to null
            nibeUnit.code = null;

            // No file should have been created
            Assert.False(File.Exists(codeFilePath));
            
            // Assert that the variable is null
            Assert.Equal(null, nibeUnit.code);
        }

        [Fact]
        public void NibeUnit_code_setter_code_Is_Not_Null_And_CodeFilePath_is_null_throws_exception()
        {
            string saveCodePath = nibeUnit.CodeFilePath;

            nibeUnit.CodeFilePath = null;

            // Throws Exception
            Exception ex = Assert.Throws<InvalidOperationException>(() => nibeUnit.code = "newcode");

            Assert.Equal("The code file is nor set", ex.Message);
            // Restore
            nibeUnit.CodeFilePath = saveCodePath;
        }


        [Fact]
        public void NibeUnit_code_getter_CodeIsNull_Returned()
        {
            NibeUnit unit = new NibeUnit();

        // Assert that the variable is null
        Assert.Equal(null, unit.code);
        }

        [Fact]
        public void NibeUnit_code_getter_CodeIsNotNullAndFileExist_Code_From_File_Returned()
        {
            // Create a file with unique content
            DateTime codestamp = DateTime.Now;
            File.WriteAllText(codeFilePath, codestamp.ToString());

            // Read code
            string readCode = nibeUnit.code;

            
            // Assert that the code is correct
            Assert.Equal(codestamp.ToString(), nibeUnit.code);
        }


        [Fact]
        public void NibeUnit_Call_Init_When_code_Is_NULL_Throws_Exception()
        {
            // Create a file with unique content
            //DateTime codestamp = DateTime.Now;
            //File.WriteAllText(codeFilePath, codestamp.ToString());

            //AppKeyConfig configs = new AppKeyConfig();
            Assert.Throws<Exception>(() => nibeUnit.init(null));           
        }

        [Fact]
        public void NibeUnit_Call_Init_When_code_Is_NotNull()
        {
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Get(
                "/oauth/token",
                (request, args) =>
                {
                    return "{ \"status\": 500 }";
                }
                );
            apiNibeStub.Start();
            // Create a file with unique content
            DateTime codestamp = DateTime.Now;
            File.WriteAllText(codeFilePath, codestamp.ToString());

            AppKeyConfig configs = new AppKeyConfig();

            //nibeUnit.init(configs);

            //Assert.Throws<Exception>(() => );

        }

    }
}
