using FluentAssertions;
using NJsonSchema;

namespace JsonSchemaStudy
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("valid-01.json")]
        [InlineData("valid-02.json")]
        [InlineData("valid-03.json")]
        [InlineData("valid-04.json")]
        [InlineData("valid-05.json")]
        [InlineData("valid-06.json")]
        [InlineData("valid-07.json")]
        [InlineData("valid-08.json")]
        [InlineData("valid-09.json")]
        public async Task Valid(string jsonFile)
        {
            // �X�L�[�}�t�@�C����ǂݍ���
            var schema = await JsonSchema.FromFileAsync("ridw-image-import-unit-schema-v1.0.0.json");

            // JSON�t�@�C���̓��e��ǂݍ���
            var jsonData = await File.ReadAllTextAsync(jsonFile);

            // JSON�̃o���f�[�V����
            var errors = schema.Validate(jsonData);

            // �G���[���Ȃ����Ƃ��m�F
            errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("error-01.json")]
        [InlineData("error-02.json")]
        [InlineData("error-03.json")]
        [InlineData("error-04.json")]
        [InlineData("error-05.json")]
        [InlineData("error-06.json")]
        [InlineData("error-07.json")]
        [InlineData("error-08.json")]
        [InlineData("error-09.json")]
        public async Task Error(string jsonFile)
        {
            // �X�L�[�}�t�@�C����ǂݍ���
            var schema = await JsonSchema.FromFileAsync("ridw-image-import-unit-schema-v1.0.0.json");

            // JSON�t�@�C���̓��e��ǂݍ���
            var jsonData = await File.ReadAllTextAsync(jsonFile);

            // JSON�̃o���f�[�V����
            var errors = schema.Validate(jsonData);

            // �G���[���Ȃ����Ƃ��m�F
            errors.Should().NotBeEmpty();
        }

    }
}